using System;
using System.IO;
using System.Threading;

namespace FileManager
{
	class Overseer
	{
		private bool enabled = true;
		private Commands slave;// does all the work
		private string sourceDirectoryPath;
		private string logPath;

		internal FileSystemWatcher watcher;

		public Overseer(Options options)
		{
			sourceDirectoryPath = options.SourceDirectoryPath;
			logPath = options.LogPath;

			slave = new Commands(options);

			watcher = new FileSystemWatcher(sourceDirectoryPath);

			watcher.Error += Watcher_Error;
			watcher.Deleted += Watcher_Deleted;
			watcher.Created += Watcher_Created;
			watcher.Created += slave.OnAdded;
			watcher.Changed += Watcher_Changed;
			watcher.Renamed += Watcher_Renamed;
		}

		public void Start()
		{
			watcher.EnableRaisingEvents = true;
			while (enabled)
			{
				Thread.Sleep(1000);
			}
		}

		public void Stop()
		{
			watcher.EnableRaisingEvents = false;
			enabled = false;
		}

		private void Watcher_Renamed(object sender, RenamedEventArgs e)
		{
			string fileEvent = "renamed to " + e.FullPath;
			string filePath = e.OldFullPath;
			RecordEntry(fileEvent, filePath);
		}

		private void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			string fileEvent = "changed";
			string filePath = e.FullPath;
			RecordEntry(fileEvent, filePath);
		}


		private void Watcher_Created(object sender, FileSystemEventArgs e)
		{
			string fileEvent = "created";
			string filePath = e.FullPath;
			RecordEntry(fileEvent, filePath);
		}


		private void Watcher_Deleted(object sender, FileSystemEventArgs e)
		{
			string fileEvent = "deleted";
			string filePath = e.FullPath;
			RecordEntry(fileEvent, filePath);
		}

		private void Watcher_Error(object sender, ErrorEventArgs e)
		{
			using (StreamWriter writer = new StreamWriter(logPath, true))
			{
				writer.WriteLine("\nException!\n" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss ") + e.GetException().Message + "\n\n");
				writer.Flush();
			}
		}

		private void RecordEntry(string fileEvent, string filePath)
		{
			using (StreamWriter writer = new StreamWriter(logPath, true))
			{
				writer.WriteLine(String.Format("{0} file {1} was {2}\n",
					DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
				writer.Flush();
			}
		}
	}
}
