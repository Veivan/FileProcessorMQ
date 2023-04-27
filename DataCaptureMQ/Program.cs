using RabbitMQ.Client;
using System.Text;

namespace DataCaptureMQ
{
	internal class Program
	{
		static MqSender mqSender;

		static void Main(string[] args)
		{
			var path = @"C:\TEMP\mq\input";
			mqSender = new MqSender();

			using var watcher = new FileSystemWatcher(path);
			watcher.NotifyFilter = NotifyFilters.Attributes
								 | NotifyFilters.CreationTime
								 | NotifyFilters.DirectoryName
								 | NotifyFilters.FileName
								 | NotifyFilters.LastAccess
								 | NotifyFilters.LastWrite
								 | NotifyFilters.Security
								 | NotifyFilters.Size;
			watcher.Created += OnCreated;
			watcher.Filter = "*.txt";
			watcher.EnableRaisingEvents = true;

			// Read input
			var input = Console.ReadLine();
			mqSender.Disconnect();
		}

		private static void OnCreated(object sender, FileSystemEventArgs e)
		{
			string value = $"Created: {e.FullPath}";
			Console.WriteLine(value);
			using (FileStream fs = File.OpenRead(e.FullPath))
			{
				int bufSize = 1024;
				byte[] bytes = new byte[bufSize];
				UTF8Encoding temp = new UTF8Encoding(true);

				string fileName = e.FullPath;
				int i = 0;
				int fsLen = fs.Read(bytes, 0, bytes.Length);
				while (fsLen > 0)
				{
					i++;
					string end = fsLen < bufSize ? "true" : "false";
					var headers = new Dictionary<string, object>
					{
						{"sequence", fileName},
						{"position", $"{i}"},
						{"end", end}
					};
					mqSender.SendMessage(headers, bytes);
					fsLen = fs.Read(bytes, 0, bytes.Length);
				}
			}
		}
	}
}