using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskCat.Common.Settings;
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
        private QueueClient queueClient;
        private string infiniToken;

        public InfiniPollingService(
            ILogger<InfiniPollingService> logger,
            IConfiguration configuration,
            RedisContext redixContext,
            ServiceBusContext serviceBusContext,
            HttpClient httpClient)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.redisContext = redixContext;
            this.serviceBusContext = serviceBusContext;
            this.httpClient = httpClient;
            orderService = new OrderService(httpClient);

            this.queueClient = this.serviceBusContext.QueueClient;
        }

        private async Task DoWork()
        {
            logger.LogInformation("Background task is working.");
            var cache = redisContext.Connection.GetDatabase();

            try
            {
                logger.LogInformation("Fetching token");
                this.infiniToken = await this.orderService.Login();
                
                logger.LogInformation("Fetching new orders");
                var newOrders = await this.orderService.GetOrders(this.infiniToken, OrderStatusCode.Ready_To_Ship);

                // TODO
                /* 1. Set a customer ID for infini in taskcat
                 * 4. Find a way so TaskCat can communicate everytime these jobs are updated. 
                 * 5. When any of these jobs are updated TaskCat will notify some message channel .
                 * 6. Add a new code block here that listens to that channel and updates the job using something like the following/
                 * */

                var defaultAddressSettings =
                    this.configuration.GetSection(DefaultAddressConfiguration).Get<ProprietorSettings>();

                // Sample update code, possibly useless here, we shouldn't use it anyway.
                foreach (var order in newOrders)
                {
                    await ProcessNewOrder(cache, defaultAddressSettings, order);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encountered");
            }
        }

        private bool shouldProcess(string cacheState)
        {
            return !(cacheState == CacheStates.POSTED || cacheState == CacheStates.CLAIMED);
        }

        private async Task ProcessNewOrder(IDatabase cache, ProprietorSettings defaultAddressSettings, PartnerModels.Infini.Order order)
        {
            // Step: Did we read this order before and if we did should we process it?
            var cachedOrderState = cache.StringGet(order.id.ToString());
            if (cachedOrderState.HasValue)
            {
                if (!shouldProcess(cachedOrderState.ToString()))
                {
                    logger.LogInformation($"Skipping order {order.id.ToString()}, it is cached as {cachedOrderState.ToString()}");
                    return;
                }
            }
            else
            {
                // Step: We didn't read this order before. Let's just mark it read before anything happens
                // We are not using this state now, may be later.
                cache.StringSet(order.id.ToString(), CacheStates.READ);
            }

            // Step: Convert the partner order to native taskcat order
            var taskcatOrder = order.ToClassifiedDeliveryOrder(defaultAddressSettings);

            logger.LogInformation($"Preparing message for order {order.id.ToString()}");
            // Step: Throw it to our message bus and update the state to posted.
            var createJobMessageBody = JsonConvert.SerializeObject(taskcatOrder);
            var createNewTaskCatJobMessage = new Message(Encoding.UTF8.GetBytes(createJobMessageBody));

            try
            {
                await this.queueClient.SendAsync(createNewTaskCatJobMessage);
                cache.StringSet(order.id.ToString(), CacheStates.POSTED);

                logger.LogInformation($"order {order.id.ToString()} is {CacheStates.POSTED}");
            }
            catch (Exception ex) when (ex is ServiceBusException || ex is InvalidOperationException)
            {
                logger.LogError($"Service bus exception encountered, skipping marking the order {CacheStates.POSTED}", ex);
            }

            //await this._orderService.UpdateOrderStatus(this._infiniToken, order.id.ToString(), OrderStatusCode.Ready_To_Ship);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Infini Background Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                await DoWork();
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }

            await queueClient.CloseAsync();
        }
    }
}
