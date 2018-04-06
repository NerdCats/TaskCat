using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using NLog;
using System;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskCat.Data.Entity;
using TaskCat.Data.Message;
using TaskCat.Data.Model.Order.Delivery;
using TaskCat.Job.Order;

namespace TaskCat.Lib.ServiceBus
{
    public class JobQueueHandler
    {
        private readonly IQueueClient pullQueueClient;
        private readonly IQueueClient pushQueueClient;
        private readonly IOrderRepository repository;
        private readonly Subject<JobActivity> activitySubject;

        private static Logger logger = LogManager.GetLogger(nameof(JobQueueHandler));

        public JobQueueHandler(
            IQueueClient pullQueueClient,
            IQueueClient pushQueueClient,
            IOrderRepository repository,
            Subject<JobActivity> activitySubject)
        {
            this.pullQueueClient = pullQueueClient ?? throw new ArgumentNullException(nameof(pullQueueClient));
            this.pushQueueClient = pushQueueClient ?? throw new ArgumentNullException(nameof(pushQueueClient));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.activitySubject = activitySubject ?? throw new ArgumentNullException(nameof(activitySubject));
        }

        public void Initiate()
        {
            logger.Info("Initiating Job Queue Handler");

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                MaxAutoRenewDuration = TimeSpan.FromHours(1)
            };

            logger.Info("Registering pull queue client");
            pullQueueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            logger.Info("Registering on job activity stream");
            activitySubject.Subscribe(OnNext, CancellationToken.None);
        }

        private void OnNext(JobActivity activity)
        {
            if (activity.Operation == JobActivityOperationNames.Update)
            {
                var jobUpdatedMessage = new TaskCatMessage()

                {
                    JobID = activity.JobId,
                    JobHRID = activity.HRID,
                    ReferenceId = activity.ReferenceId,
                    RemoteJobStage = RemoteJobStage.UPDATE,
                    RemoteJobState = activity.Value.ToString()
                };


            }
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            logger.Info($"Received jobpull message: SequenceNumber:{message.SystemProperties.SequenceNumber}");
            var messageString = Encoding.UTF8.GetString(message.Body);
            var taskcatOrder = JsonConvert.DeserializeObject<ClassifiedDeliveryOrder>(messageString);

            logger.Info($"Order from {taskcatOrder.UserId} converted, ready to order");

            var jobCreatedMessage = new TaskCatMessage();

            try
            {
                var newJob = await PostNewOrder(taskcatOrder);

                logger.Info($"New job created. Remote Order = {taskcatOrder.ReferenceOrderId} => TaskCat Job {newJob.Id} => HRID {newJob.HRID}");
                jobCreatedMessage = new TaskCatMessage
                {
                    ReferenceId = taskcatOrder.ReferenceOrderId,
                    JobHRID = newJob.HRID,
                    JobID = newJob.Id,
                    RemoteJobStage = RemoteJobStage.CREATED
                };                
            }
            catch (Exception ex) // Catching global exception is a bad thing, fix this
            {
                logger.Error(ex);
                logger.Error($"Error processing remote Order = {taskcatOrder.ReferenceOrderId}");

                jobCreatedMessage = new TaskCatMessage
                {
                    ReferenceId = taskcatOrder.ReferenceOrderId,
                    RemoteJobStage = RemoteJobStage.ERROR
                }; 
            }

            logger.Info($"Sending message back to polling service");
            var pushMessageBody = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jobCreatedMessage)));
            await this.pushQueueClient.SendAsync(pushMessageBody);
            logger.Info($"Sent message back to polling service");
        }

        private async Task<Data.Entity.Job> PostNewOrder(ClassifiedDeliveryOrder taskcatOrder)
        {
            var createdJob = await this.repository.PostOrder(taskcatOrder);
            var referenceUserForActivityLog = new ReferenceUser(taskcatOrder.UserId, createdJob.User.UserName);
            activitySubject.OnNext(new JobActivity(createdJob, JobActivityOperationNames.Create, referenceUserForActivityLog));
            return createdJob;
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            logger.Error($"jobpull message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");

            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            logger.Info("Exception context for troubleshooting:");
            logger.Info($"- Endpoint: {context.Endpoint}");
            logger.Info($"- Entity Path: {context.EntityPath}");
            logger.Info($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
