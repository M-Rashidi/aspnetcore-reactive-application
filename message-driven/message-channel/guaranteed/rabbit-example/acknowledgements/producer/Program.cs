using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace producer
{
    class Program
    {
        private static readonly string url = "amqps://zmkoitrj:X5Pa_v8emJMUHzzbiL17QNoh0j52HvUo@baboon.rmq.cloudamqp.com/zmkoitrj";
        private EventHandler<BasicAckEventArgs> _acknowledgeEventHandler;

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(url)
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                var queueName = "task_queue";
                bool durable = true;
                bool exclusive = false;
                bool autoDelete = true;

                channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

                var message = Console.ReadLine();
                var body = Encoding.UTF8.GetBytes(message);

                channel.ConfirmSelect();

                channel.BasicAcks += (sender, ea) =>
                {
                    Console.WriteLine("Acks");
                };
                channel.BasicNacks += (sender, ea) =>
                {
                    Console.WriteLine("Nacks");
                };

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;


                channel.BasicPublish(
                    exchange: "",
                    routingKey: "task_queue",
                    basicProperties: properties,
                    body: body);


                //2
                //channel.WaitForConfirmsOrDie();
                Console.WriteLine("Sent {0}", message);
            }


            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}

