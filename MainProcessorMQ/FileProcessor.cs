using System.Text;

namespace MainProcessorMQ
{
	internal class FileProcessor
	{
		private readonly string path;
		private readonly ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();

		public FileProcessor(string path)
		{
			this.path = path;
		}

		internal void Process(IDictionary<string, object> headers, byte[] bytes)
		{
			if (headers == null)
				throw new ArgumentNullException(nameof(headers));
			if (bytes == null)
				throw new ArgumentNullException(nameof(bytes));

			var sequence = Encoding.UTF8.GetString(headers["sequence"] as byte[]);
			var position = Encoding.UTF8.GetString(headers["position"] as byte[]);
			var size = Encoding.UTF8.GetString(headers["size"] as byte[]);
			Console.WriteLine("sequence=" + sequence);
			Console.WriteLine("position=" + position);
			Console.WriteLine("size=" + size);

			string fileName = path + "\\" + sequence;
			readerWriterLockSlim.EnterWriteLock();
			using (FileStream fs = File.OpenWrite(fileName))
			{
				fs.Seek(0, SeekOrigin.End);
				fs.Write(bytes, 0, Int32.Parse(size));
			}
			readerWriterLockSlim.ExitWriteLock();
		}
	}
}
