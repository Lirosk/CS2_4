using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FileManager
{
	//from lab1...
	internal class Commands
	{
		private string targetDirectoryPath;
		private string logPath;
		private object locker = new object();

		public Commands(Options options)
		{
			targetDirectoryPath = options.TargetDirectoryPath;
			logPath = options.LogPath;
		}

		internal void OnAdded(object sender, FileSystemEventArgs e)
		{
			Added(e.FullPath);
		}

		private void Added(in string path)
		{
			lock (locker)
			{
				var sourceDirectoryPath = Path.GetDirectoryName(path);
				var name = Path.GetFileName(path);

				try
				{
					//to zip
					Manager.overseer.watcher.EnableRaisingEvents = false;
					Zip(sourceDirectoryPath, sourceDirectoryPath, name);
					Delete(sourceDirectoryPath, name);
					Manager.overseer.watcher.EnableRaisingEvents = true;

					//encrypt
					Encrypt(sourceDirectoryPath, name + ".gz"); //because Zip(...) adds ".gz" to zipped file name

					//move
					Move(sourceDirectoryPath, targetDirectoryPath, name + ".gz");

					//unzip
					Decrypt(targetDirectoryPath, name + ".gz");
					Unzip(targetDirectoryPath, targetDirectoryPath, name + ".gz");
					Delete(targetDirectoryPath, name + ".gz");
				}
				catch (IOException ex)
				{
					using (StreamWriter writer = new StreamWriter(logPath, true))
					{
						writer.WriteLine("\nException!\n" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss ") + ex.Message + "\n\n");
						writer.Flush();
					}

					Delete(sourceDirectoryPath, name + ".gz");
				}
			}
		}

		void Unzip(string sourceDirectoryPath, string targetDirectoryPath, string name)
		{
			var buf = name.Substring(0, name.Length - 3);//[..^3];

			using (var targetStream = File.Create(Path.Combine(targetDirectoryPath, buf)))
			using (var sourceStream = File.OpenRead(Path.Combine(sourceDirectoryPath, name)))
			using (var decomressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
			{
				decomressionStream.CopyTo(targetStream);
			}
		}

		private void Move(string sourcePath, string targetPath, string name)
		{
			if (File.Exists(Path.Combine(targetPath, name)))
			{
				Delete(targetPath, name);
			}

			File.Move(Path.Combine(sourcePath, name), Path.Combine(targetPath, name));
		}

		private void Delete(string filePath, string name)
		{
			var buf = Path.Combine(filePath, name);

			File.Delete(buf);
		}

		private void Zip(string sourcePath, string targetPath, string name)
		{
			var buf = new StringBuilder(name);
			buf.Append(".gz");

			using (var targetStream = File.Create(Path.Combine(targetPath, buf.ToString())))
			using (var sourceStream = new FileStream(Path.Combine(sourcePath, name), FileMode.OpenOrCreate, FileAccess.Read))
			using (var compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
			{
				sourceStream.CopyTo(compressionStream);
			}
		}

		private void Encrypt(string filePath, string name)
		{
			File.Encrypt(Path.Combine(filePath, name));
		}

		private void Decrypt(string filePath, string name)
		{
			File.Decrypt(Path.Combine(filePath, name));
		}
	}
}
