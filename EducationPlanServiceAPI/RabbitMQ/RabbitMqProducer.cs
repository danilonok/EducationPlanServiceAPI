using System.Text;

namespace EducationPlanServiceAPI.RabbitMQ
{
    public class RabbitMqProducer
    {
        private readonly RabbitMqConnection _connection;

        public RabbitMqProducer(RabbitMqConnection connection)
        {
            _connection = connection;
        }

        public void SendMessage(string queueName, string message)
        {
            var channel = _connection.GetChannel();

            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                mandatory: true,
                basicProperties: null,
                body: body);

            Console.WriteLine($"[x] Sent: {message}");
        }
    }
}
