using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using queue.model;

namespace queue.writer
{
    class Program
    {
        static void Main(string[] args)
        {
            var workerConfigurationManager = new WorkerConfigurationManager();
            var workerConfiguration = workerConfigurationManager.GetWorkerConfiguration();

            var connectionString = workerConfiguration.ConnectionString;
            var queueName = workerConfiguration.QueueName;
            
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            var orderList = new List<BrokeredMessage>();

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 250; i++)
            {
                var order = new Order
                {
                    CreationDate = DateTime.UtcNow,
                    OrderNumber = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString(),
                    SalesStatus = SalesStatus.Sold
                };

                var jsonContent = JsonConvert.SerializeObject(order);
                
                var message = new BrokeredMessage(jsonContent);
                orderList.Add(message);
            }

            client.SendBatch(orderList);
            Console.WriteLine($"Finished, took {stopwatch.ElapsedMilliseconds} ms");

        }
    }
}
