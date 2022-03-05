using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace producer
{
    class Program
    {
        private static readonly string url = "amqps://zmkoitrj:X5Pa_v8emJMUHzzbiL17QNoh0j52HvUo@baboon.rmq.cloudamqp.com/zmkoitrj";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(url)
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var exchangeName = "Product";

            var exchangeType = ExchangeType.Direct;

            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

            var messages = new List<DomainEvent>
            {
                new ProductQueries(),
                new ProductPurchase(),
                new ProductQuote()
            };

            foreach (var _message in messages)
            {
                var message = JsonConvert.SerializeObject(_message);
                var data = Encoding.UTF8.GetBytes(message);
                var routingKey = _message.GetType().Name;

                channel.BasicPublish(exchangeName, routingKey, null, data);
            }
        }
    }

    public class DomainEvent
    {
    }

    public class ProductQueries : DomainEvent
    {
        public ProductQueries()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
    public class ProductQuote : DomainEvent
    {
        public ProductQuote()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
    public class ProductPurchase : DomainEvent
    {
        public ProductPurchase()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}