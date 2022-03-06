using System.Text;
using Newtonsoft.Json;
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

            var exchangeName = "Product";

            var exchangeType = ExchangeType.Direct;

            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

            var messages = new List<Event>
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

    public class Event
    {
        public string Type { get; set; }
        public Guid Id { get; set; }
    }
    public class ProductQueries : Event
    {
        public ProductQueries()
        {
            Id = Guid.NewGuid();
            Type = "Queries";
        }
    }
    public class ProductQuote : Event
    {
        public ProductQuote()
        {
            Id = Guid.NewGuid();
            Type = "Quote";
        }
    }
    public class ProductPurchase : Event
    {
        public ProductPurchase()
        {
            Id = Guid.NewGuid();
            Type = "Purchase";
        }
    }
}