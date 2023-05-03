using RabbitMQ.Client;
using System.Text;

namespace MainProcessorMQ
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string path = args[0];
			//string path = @"C:\TEMP\mq\output";

			if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
				throw new ArgumentNullException(path);

			FileProcessor fileProcessor = new FileProcessor(path);

			ConsumerMQ consumer = new ConsumerMQ(fileProcessor);
			try
			{
				consumer.Init();
				Console.ReadLine();
			}
			finally
			{
				consumer.Disconnect();
			}
		}
	}
}