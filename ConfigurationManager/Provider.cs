namespace ConfigurationManager
{
	//as Artsiom said
	public class Provider
	{
		private readonly Parsers.IParser _configParser;

		public T GetConfig<T>(string filePath)
		{
			return _configParser.Parse<T>(filePath);
		}

		public Provider(Parsers.IParser configParser)
		{
			_configParser = configParser;
		}

		public Provider() { }
	}
}
