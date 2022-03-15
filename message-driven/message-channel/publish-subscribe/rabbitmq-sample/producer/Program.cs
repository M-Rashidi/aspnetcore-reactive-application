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

            var exchangeName = "publish-subscribe";
            var exchangeType = ExchangeType.Fanout;

            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);

            var message = Console.ReadLine();

            var data = Encoding.UTF8.GetBytes(message);

            var routingKey = "";

            channel.BasicPublish(exchangeName, routingKey, null, data);
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0)
                ? string.Join(" ", args)
                : "info: Hello World!");
        }
    }
}