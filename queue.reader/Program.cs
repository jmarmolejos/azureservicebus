using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using queue.model;

namespace queue.reader
{
    class Program
    {
        static void Main(string[] args)
        {
            var workerConfigurationManager = new WorkerConfigurationManager();
            var workerConfiguration = workerConfigurationManager.GetWorkerConfiguration();

            var connectionString = workerConfiguration.ConnectionString;
            var queueName = workerConfiguration.QueueName;
            var deadLetterQueueName = QueueClient.FormatDeadLetterPath(queueName);
            
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            client.OnMessage(message =>
            {
                Console.WriteLine($"Message body: {message.GetBody<String>()}");
                Console.WriteLine($"Message id: {message.MessageId}");
            });

            //RetrieveLoop(client);

            //PeekMessages(client, stopwatch);

            Console.ReadLine();
        }

        private static void RetrieveLoop(QueueClient client)
        {
            while (true)
            {
                var message = client.Receive();
                Console.WriteLine($"Message body: {message.GetBody<String>()}");
                Console.WriteLine($"Message id: {message.MessageId}");

                message.Complete();
            }
        }

        private static void PeekMessages(QueueClient client)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var messages = client.PeekBatch(250).ToList();

            messages.ForEach(message =>
            {
                Console.WriteLine($"Message body: {message.GetBody<String>()}");
                Console.WriteLine($"Message id: {message.MessageId}");
            });

            Console.WriteLine($"Read {messages.Count} messages.");
            Console.WriteLine($"Finished, took {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
