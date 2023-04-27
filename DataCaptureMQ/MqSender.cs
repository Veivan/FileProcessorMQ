using RabbitMQ.Client;

namespace DataCaptureMQ
{
	internal class MqSender
	{
		const string exchangeName = "fileproc";
		const string queueName = "fpstack";
		const string routingKey = "fileproc.fpstack";

		IModel channel;
		IConnection connection;

		public MqSender()
		{
			// Connect to RabbitMQ
			var factory = new ConnectionFactory();
			factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
			connection = factory.CreateConnection();
			channel = connection.CreateModel();

			// Declare exchange and queue
			channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
			channel.QueueDeclare(queueName, true, false, true);
			channel.QueueBind(queueName, exchangeName, routingKey);
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

			channel.BasicPublish(exchangeName, routingKey, props, bytes);
		}
	}
}
