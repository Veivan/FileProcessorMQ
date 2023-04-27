using RabbitMQ.Client;
using System.Text;

namespace MainProcessorMQ
{
	internal class Program
	{
		static void Main(string[] args)
		{
			FileProcessor fileProcessor = new FileProcessor();

			var queueName = "fpstack";

			// Connect to RabbitMQ
			var factory = new ConnectionFactory();
			factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
			var connection = factory.CreateConnection();
			var channel = connection.CreateModel();

			// Subscribe to incoming messages
			var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);
			consumer.Received += (sender, eventArgs) =>
			{
				fileProcessor.Process(eventArgs.BasicProperties.Headers, eventArgs.Body.ToArray());
			};

			channel.BasicConsume(queueName, true, consumer);

			Console.ReadLine();

			// Disconnect
			channel.Close();
			connection.Close();
		}
	}
}