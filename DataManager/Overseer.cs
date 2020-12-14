using System;
using System.Threading;
using System.IO;
using DataAccess;
using ServiceLayer;
using Models.Result;

namespace DataManager
{
	class Overseer
	{
		private bool enabled = true;
		private readonly FileSystemWatcher watcher;
		private readonly string connectionString;
		private readonly string targetDirectorPath;
		private readonly string tasksDirectoryPath;
		private readonly string logPath;
		private object locker = new object();
		private readonly bool deleteFile;
		private readonly DBAccess dbAccess;

		public Overseer(Options options)
		{			
			connectionString = options.ConnectionString;

			targetDirectorPath = options.TargetDirectoryPath;
			tasksDirectoryPath = options.TasksDirectoryPath;
			logPath = options.LogPath;

			deleteFile = options.DeleteFile;

			dbAccess = new DBAccess(connectionString);
			watcher = new FileSystemWatcher(tasksDirectoryPath);

			watcher.Error += Watcher_Error;
			watcher.Deleted += Watcher_Deleted;
			watcher.Created += Watcher_Added;
			if (deleteFile)
			{
				watcher.Created += Delete;
			}
			watcher.Changed += Watcher_Changed;
			watcher.Renamed += Watcher_Renamed;
		}

		public void Watcher_Added(object sender, FileSystemEventArgs e)
		{
			lock (locker)
			{
				try
				{
					var result = GetResult(e.FullPath);
					Layer.GenerateResultFiles(result, targetDirectorPath, "result");
				}
				catch (Exception ex)
				{
					Watcher_Error(this, new ErrorEventArgs(ex));
				}
			}
		}

		private Result<object> GetResult(string file)
		{
			return dbAccess.GetTable<object>(file);
		}

		private void Delete(object sender, FileSystemEventArgs e)
		{
			File.Delete(e.FullPath);
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
			lock (locker)
			{
				using (StreamWriter writer = new StreamWriter(logPath, true))
				{
					writer.WriteLine(String.Format("{0} task {1} was {2}\n",
						DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
					writer.Flush();
				}
			}
		}
	}
}
