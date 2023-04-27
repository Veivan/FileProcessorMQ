using RabbitMQ.Client;
using System.Text;

namespace MainProcessorMQ
{
	internal class Program
	{
		static void Main(string[] args)
		{
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
				var msg = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
				IBasicProperties props = eventArgs.BasicProperties;
				var sequence = Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["sequence"] as byte[]);
				var position = Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["position"] as byte[]);
				var end = Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["end"] as byte[]);
				Console.WriteLine("sequence=" + sequence);
				Console.WriteLine("position=" + position);
				Console.WriteLine("end=" + end);
				//Console.WriteLine(msg);
			};

			channel.BasicConsume(queueName, true, consumer);

			Console.ReadLine();

			// Disconnect
			channel.Close();
			connection.Close();
		}
	}
}