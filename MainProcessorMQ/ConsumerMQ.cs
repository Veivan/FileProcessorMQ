using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MainProcessorMQ
{
	internal class ConsumerMQ
	{
		const string ConnectionString = "amqp://guest:guest@localhost:5672";
		const string QueueName = "fpstack";

		private readonly FileProcessor fileProcessor;

		private readonly IModel channel;
		private readonly IConnection connection;
		private readonly EventingBasicConsumer consumer;

		public ConsumerMQ(FileProcessor _fileProcessor)
		{
			this.fileProcessor = _fileProcessor;
			// Connect to RabbitMQ
			var factory = new ConnectionFactory();
			factory.Uri = new Uri(ConnectionString);
			connection = factory.CreateConnection();
			channel = connection.CreateModel();

			// Subscribe to incoming messages
			consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);
			consumer.Received += (sender, eventArgs) =>
			{
				fileProcessor.Process(eventArgs.BasicProperties.Headers, eventArgs.Body.ToArray());
			};
		}

		public void Init()
		{
			channel.BasicConsume(QueueName, true, consumer);
		}

		public void Disconnect()
		{
			channel.Close();
			connection.Close();
		}
	}
}
