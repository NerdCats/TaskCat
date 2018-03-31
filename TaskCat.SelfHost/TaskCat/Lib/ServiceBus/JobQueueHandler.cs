using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskCat.Data.Entity;
using TaskCat.Data.Message;
using TaskCat.Data.Model.Order.Delivery;
using TaskCat.Job.Order;
using TaskCat.Job.Order.Exceptions;

namespace TaskCat.Lib.ServiceBus
{
    public class JobQueueHandler
    {
        private readonly IQueueClient pullQueueClient;
        private readonly IQueueClient pushQueueClient;
        private readonly IOrderRepository repository;
        private readonly Subject<JobActivity> activitySubject;

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
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                MaxAutoRenewDuration = TimeSpan.FromHours(1)
            };

            pullQueueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Received jobpull message: SequenceNumber:{message.SystemProperties.SequenceNumber}");
            var messageString = Encoding.UTF8.GetString(message.Body);
            var taskcatOrder = JsonConvert.DeserializeObject<ClassifiedDeliveryOrder>(messageString);

            Console.WriteLine($"Order from {taskcatOrder.UserId} converted, ready to order");

            var pushMessage = new TaskCatMessage();

            try
            {
                var newJob = await PostNewOrder(taskcatOrder);

                Console.WriteLine($"New job created. Remote Order = {taskcatOrder.ReferenceOrderId} => TaskCat Job {newJob.Id}");
                pushMessage = new TaskCatMessage
                {
                    ReferenceId = taskcatOrder.ReferenceOrderId,
                    Job = newJob,
                    JobActivityOperationName = JobActivityOperationNames.Create
                };                
            }
            catch (Exception ex) // Catching global exception is a bad thing, fix this
            {
                Console.WriteLine(ex);
                Console.WriteLine($"Error processing remote Order = {taskcatOrder.ReferenceOrderId}");

                pushMessage = new TaskCatMessage
                {
                    ReferenceId = taskcatOrder.ReferenceOrderId,
                    JobActivityOperationName = JobActivityOperationNames.Error
                }; 
            }

            Console.WriteLine($"Sending message back to polling service");
            var pushMessageBody = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pushMessage)));
            await this.pushQueueClient.SendAsync(pushMessageBody);
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
            Console.WriteLine($"jobpull message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");

            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
