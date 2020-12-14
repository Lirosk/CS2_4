namespace DataManager
{
	class Options
	{
		public string ConnectionString { get; set; }
		public string TargetDirectoryPath { get; set; }
		public string TasksDirectoryPath { get; set; }
		public string LogPath { get; set; }
		public bool DeleteFile { get; set; }
	}
}
