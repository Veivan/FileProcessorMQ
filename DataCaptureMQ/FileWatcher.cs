using System.Text;

namespace DataCaptureMQ
{
	internal class FileWatcher
	{
		private readonly MqSender mqSender;

		public FileWatcher(string _path, MqSender _mqSender)
		{
			this.mqSender = _mqSender;
			var watcher = new FileSystemWatcher(_path);

			watcher.NotifyFilter = NotifyFilters.Attributes
										 | NotifyFilters.CreationTime
										 | NotifyFilters.DirectoryName
										 | NotifyFilters.FileName
										 | NotifyFilters.LastAccess
										 | NotifyFilters.LastWrite
										 | NotifyFilters.Security
										 | NotifyFilters.Size;
			watcher.Created += OnCreated;
			watcher.Error += OnError;
			watcher.Filter = "*.avi";
			watcher.EnableRaisingEvents = true;
		}

		private void OnCreated(object sender, FileSystemEventArgs e)
		{
			string value = $"Recieved: {e.FullPath}";
			Console.WriteLine(value);

			Thread.Sleep(1500);

			using (FileStream fs = File.OpenRead(e.FullPath))
			{
				int bufSize = 1000000;
				byte[] bytes = new byte[bufSize];
				UTF8Encoding temp = new UTF8Encoding(true);

				string fileName = Path.GetFileName(e.FullPath);
				int i = 0;
				int fsLen = fs.Read(bytes, 0, bytes.Length);
				while (fsLen > 0)
				{
					i++;
					var headers = new Dictionary<string, object>
					{
						{"sequence", fileName},
						{"position", $"{i}"},
						{"size", $"{fsLen}"}
					};
					mqSender.SendMessage(headers, bytes);
					fsLen = fs.Read(bytes, 0, bytes.Length);
				}
			}
		}

		private static void OnError(object sender, ErrorEventArgs e) =>
		PrintException(e.GetException());

		private static void PrintException(Exception? ex)
		{
			if (ex != null)
			{
				Console.WriteLine($"Message: {ex.Message}");
				Console.WriteLine("Stacktrace:");
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine();
				PrintException(ex.InnerException);
			}
		}
	}
}
