using System.Text;

namespace MainProcessorMQ
{
	internal class FileProcessor
	{
		string path = @"C:\TEMP\mq\output\";
		static ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();

		internal void Process(IDictionary<string, object> headers, byte[] bytes)
		{
			var sequence = Encoding.UTF8.GetString(headers["sequence"] as byte[]);
			var position = Encoding.UTF8.GetString(headers["position"] as byte[]);
			var size = Encoding.UTF8.GetString(headers["size"] as byte[]);
			Console.WriteLine("sequence=" + sequence);
			Console.WriteLine("position=" + position);
			Console.WriteLine("size=" + size);

			string fileName = path + sequence;
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
