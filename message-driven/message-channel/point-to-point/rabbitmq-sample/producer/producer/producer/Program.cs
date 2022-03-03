using System;
using System.Text;
using RabbitMQ.Client;

namespace producer
{
    class Program
    {
        private static readonly string url = "amqp://guest:guest@localhost/%2f";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(url)
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var queueName = "point-to-point-queue";
            bool durable = false;
            bool exclusive = false;
            bool autoDelete = true;

            channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

            var message = Console.ReadLine();

            var data = Encoding.UTF8.GetBytes(message);

            var exchangeName = "";
            var routingKey = queueName;
            channel.BasicPublish(exchangeName, routingKey, null, data);
        }
    }
}
