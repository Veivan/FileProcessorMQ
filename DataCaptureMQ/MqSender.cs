using RabbitMQ.Client;

namespace DataCaptureMQ
{
	internal class MqSender
	{
		const string ConnectionString = "amqp://guest:guest@localhost:5672";
		const string ExchangeName = "fileproc";
		const string QueueName = "fpstack";
		const string RoutingKey = "fileproc.fpstack";
		
		private readonly IModel channel;
		private readonly IConnection connection;

		public MqSender()
		{
			// Connect to RabbitMQ
			var factory = new ConnectionFactory();
			factory.Uri = new Uri(ConnectionString);
			connection = factory.CreateConnection();
			channel = connection.CreateModel();

			// Declare exchange and queue
			channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
			channel.QueueDeclare(QueueName, true, false, true);
			channel.QueueBind(QueueName, ExchangeName, RoutingKey);
		}

		public void Disconnect()
		{
			channel.Close();
			connection.Close();
		}

		public void SendMessage(IDictionary<string, object> headers, byte[] bytes)
		{
			IBasicProperties props = channel.CreateBasicProperties();
			props.Headers = headers;

			channel.BasicPublish(ExchangeName, RoutingKey, props, bytes);
		}
	}
}
