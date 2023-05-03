namespace DataCaptureMQ
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string path = args[0];
			//string path = @"C:\TEMP\mq\input01";

			if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
				throw new ArgumentNullException(path);

			MqSender mqSender = new MqSender();

			try
			{
				_ = new FileWatcher(path, mqSender);
				
				// Read input
				var input = Console.ReadLine();
			}
			finally
			{
				mqSender.Disconnect();
			}
		}
	}
}