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

            var exchangeName = "dlx_exchange";
            var exchangeType = ExchangeType.Direct;

            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

            var queueName = "queue_x-dead-letter";
            bool durable = true;
            bool exclusive = false;
            bool autoDelete = false;

            Dictionary<string, object> arguments = new Dictionary<string, object>()
            {
                { "x-dead-letter-exchange", "dl.exchange" },
                {"x-dead-letter-routing-key", "dlx_key"}
            };

            channel.QueueDeclare(queueName, durable, exclusive, autoDelete, arguments);
        }
    }
}