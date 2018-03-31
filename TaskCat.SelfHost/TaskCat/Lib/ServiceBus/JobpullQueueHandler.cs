using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskCat.Data.Model.Order.Delivery;

namespace TaskCat.Lib.ServiceBus
{
    public class JobpullQueueHandler
    {
        private readonly IQueueClient queueClient;

        public JobpullQueueHandler(IQueueClient queueClient)
        {
            this.queueClient = queueClient ?? throw new System.ArgumentNullException(nameof(queueClient));
        }

        public void Initiate()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Received jobpull message: SequenceNumber:{message.SystemProperties.SequenceNumber}");
            var messageString = Encoding.UTF8.GetString(message.Body);
            var taskcatOrder = JsonConvert.DeserializeObject<ClassifiedDeliveryOrder>(messageString);

            // TODO: Don't complete a message yet, we need to create a job from here. 
            // await queueClient.CompleteAsync(message.SystemProperties.LockToken);
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
