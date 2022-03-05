using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace consumer
{
    class Program
    {
        private static IConnection? _connection;
        private static IModel? _channel;
        private static readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private static readonly string url = "amqps://zmkoitrj:X5Pa_v8emJMUHzzbiL17QNoh0j52HvUo@baboon.rmq.cloudamqp.com/zmkoitrj";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(url)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var exchangeName = "Product";
            var productQueries = "ProductQueries";
            var productPurchase = "ProductPurchase";
            var productQuote = "ProductQuote";


            var queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: productQueries);
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: productPurchase);
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: productQuote);

            var consumer = new EventingBasicConsumer(_channel);


            consumer.Received += (model, deliveryEventArgs) =>
            {
                var body = deliveryEventArgs.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("** Received message: {0} by Consumer thread in **", message);

                _channel.BasicAck(deliveryEventArgs.DeliveryTag, false);
            };

            _ = _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            _resetEvent.WaitOne();
            _channel?.Close();
            _channel = null;
            _connection?.Close();
            _connection = null;
        }
    }
}