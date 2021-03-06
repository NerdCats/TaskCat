﻿using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskCat.Common.Settings;
using TaskCat.Data.Entity;
using TaskCat.Data.Message;
using TaskCat.PartnerModels.Infini;
using TaskCat.PartnerServices.Infini;

namespace TaskCat.BackgroundJobService
{
    public class InfiniPollingService: HostedService, IDisposable
    {
        private const string DefaultAddressConfiguration = "DefaultFromAddress";
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly RedisContext redisContext;
        private readonly ServiceBusContext serviceBusContext;
        private HttpClient httpClient;
        private OrderService orderService;
        private QueueClient pushQueueClient;
        private QueueClient pullQueueClient;
        private string infiniToken;
        private IDatabase cache;

        public InfiniPollingService(
            ILogger<InfiniPollingService> logger,
            IConfiguration configuration,
            RedisContext redisContext,
            ServiceBusContext serviceBusContext,
            HttpClient httpClient)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.redisContext = redisContext;
            this.serviceBusContext = serviceBusContext;
            this.httpClient = httpClient;
            orderService = new OrderService(httpClient);

            
            this.pushQueueClient = this.serviceBusContext.PushQueueClient;
            this.pullQueueClient = this.serviceBusContext.PullQueueClient;
        }

        private async Task DoWork()
        {
            logger.LogInformation("Background task is working.");
            logger.LogInformation($"Pulling messages from {this.pullQueueClient.QueueName}");
            logger.LogInformation($"Pushing messages to {this.pushQueueClient.QueueName}");

            try
            {
                logger.LogInformation("Fetching token");
                var username = this.configuration["userid"];
                var password = this.configuration["password"];

                this.infiniToken = await this.orderService.Login(username, password);
                
                logger.LogInformation("Fetching new orders");
                var newOrders = await this.orderService.GetOrders(this.infiniToken, OrderStatusCode.Ready_To_Ship);
                logger.LogInformation($"Fetched {newOrders.Count()} new orders");
                /* TODO
                 * 5. When any of these jobs are updated TaskCat will notify some message channel .
                 * 6. Add a new code block here that listens to that channel and updates the job using something like the following/
                 * */

                var defaultAddressSettings =
                    this.configuration.GetSection(DefaultAddressConfiguration).Get<ProprietorSettings>();

                // Sample update code, possibly useless here, we shouldn't use it anyway.
                foreach (var order in newOrders)
                {
                    await ProcessOrder(cache, defaultAddressSettings, order);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered");
            }
        }

        private bool shouldProcess(string cacheState)
        {
            return (cacheState == RemoteJobStage.ERROR 
                || cacheState == RemoteJobStage.READ
                || cacheState == RemoteJobStage.UPDATE);
        }

        private async Task ProcessOrder(IDatabase cache, ProprietorSettings defaultAddressSettings, PartnerModels.Infini.Order order)
        {
            // Step: Did we read this order before and if we did should we process it?
            var cachedOrderState = cache.StringGet(order.id.ToString());
            if (cachedOrderState.HasValue)
            {
                if (!shouldProcess(cachedOrderState.ToString()))
                {
                    logger.LogDebug($"Skipping order {order.id.ToString()}, it is cached as {cachedOrderState.ToString()}");
                    return;
                }
            }
            else
            {
                // Step: We didn't read this order before. Let's just mark it read before anything happens
                // We are not using this state now, may be later.
                cache.StringSet(order.id.ToString(), RemoteJobStage.READ);
            }

            var infiniUserId = this.configuration["Infini:userid"];
            var serviceCharge = Decimal.Parse(this.configuration["ServiceCharge"]);
            // Step: Convert the partner order to native taskcat order
            var taskcatOrder = order.ToClassifiedDeliveryOrder(serviceCharge, defaultAddressSettings, infiniUserId);

            logger.LogDebug($"Preparing message for order {order.id.ToString()}");
            // Step: Throw it to our message bus and update the state to posted.
            var createJobMessageBody = JsonConvert.SerializeObject(taskcatOrder);
            var createNewTaskCatJobMessage = new Message(Encoding.UTF8.GetBytes(createJobMessageBody));

            try
            {
                await this.pushQueueClient.SendAsync(createNewTaskCatJobMessage);
                cache.StringSet(order.id.ToString(), RemoteJobStage.POSTED);

                logger.LogInformation($"order {order.id.ToString()} is {RemoteJobStage.POSTED}");
            }
            catch (Exception ex) when (ex is ServiceBusException || ex is InvalidOperationException)
            {
                logger.LogError($"Service bus exception encountered, skipping marking the order {RemoteJobStage.POSTED}", ex);
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Infini Background Service is starting.");
            InitiatePullQueueHandler();
            this.cache = redisContext.Connection.GetDatabase();

            while (!cancellationToken.IsCancellationRequested)
            {
                await DoWork();
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }

            await pushQueueClient.CloseAsync();
        }

        private void InitiatePullQueueHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                MaxAutoRenewDuration = TimeSpan.FromHours(1)
            };

            pullQueueClient.RegisterMessageHandler(ProcessTaskCatMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessTaskCatMessagesAsync(Message message, CancellationToken token)
        {
            logger.LogInformation($"Received message from TaskCat: SequenceNumber:{message.SystemProperties.SequenceNumber}");

            var messageBody = Encoding.UTF8.GetString(message.Body);
            var taskCatMessage = JsonConvert.DeserializeObject<TaskCatMessage>(messageBody);

            if (taskCatMessage.RemoteJobStage == RemoteJobStage.ERROR)
            {
                logger.LogInformation($"Order {taskCatMessage.ReferenceId} ended in {RemoteJobStage.ERROR}");
                await this.cache.StringSetAsync(taskCatMessage.ReferenceId, RemoteJobStage.ERROR);
            }
            else if (taskCatMessage.RemoteJobStage == RemoteJobStage.CREATED)
            {
                logger.LogInformation($"Order {taskCatMessage.ReferenceId} was created in TaskCat. Taskcat job ID/HRID: {taskCatMessage.JobID} => {taskCatMessage.JobHRID}");
                await this.orderService.UpdateOrderReferenceId(this.infiniToken, taskCatMessage.ReferenceId, taskCatMessage.JobHRID);
                await this.cache.StringSetAsync(taskCatMessage.ReferenceId, RemoteJobStage.CREATED);
            }
            else if (taskCatMessage.RemoteJobStage == RemoteJobStage.UPDATE)
            {
                logger.LogInformation($"Order {taskCatMessage.ReferenceId} was updated in TaskCat. Taskcat job ID/HRID: {taskCatMessage.JobID} => {taskCatMessage.JobHRID}");

                OrderStatusCode translatedStatus = StatusTranslator.TranslateStatus(taskCatMessage.RemoteJobState);
                if (translatedStatus != OrderStatusCode.Undefined)
                    await this.orderService.UpdateOrderStatus(this.infiniToken, taskCatMessage.ReferenceId, translatedStatus);

                await this.cache.StringSetAsync(taskCatMessage.ReferenceId, RemoteJobStage.UPDATE);
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            logger.LogError($"jobpull message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");

            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            logger.LogInformation("Exception context for troubleshooting:");
            logger.LogInformation($"- Endpoint: {context.Endpoint}");
            logger.LogInformation($"- Entity Path: {context.EntityPath}");
            logger.LogInformation($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
