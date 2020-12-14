using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Parsers
{
	public class Parser : IParser
	{
		private readonly XmlParser _xmlParser;
		private readonly JsonParser _jsonParser;

		public T Parse<T>(string filePath)
		{
			if (File.Exists(filePath))
			{
				switch (Path.GetExtension(filePath))
				{
					case (".json"):
						{
							return _jsonParser.Parse<T>(filePath);
						}
					case (".xml"):
						{
							return _xmlParser.Parse<T>(filePath);
						}
				}
			}
			else
			{
				throw new FileNotFoundException("File with obj to parse doesn't exist!");
			}

			throw new Exception("Cannot to parse!");
		}

		//sorry, IEnumerables stronger than my parsers
		public static T LazyParse<T>(string filePath)
		{
			if (File.Exists(filePath))
			{
				switch(Path.GetExtension(filePath))
				{
					case (".json"):
						{
							return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
						}
					case (".xml"):
						{
							T result;

							using (var stream = new FileStream(filePath, FileMode.Open))
							{
								result = (T)new XmlSerializer(typeof(T)).Deserialize(stream);
							}

							return result;
						}
				}
			}
			else
			{
				throw new FileNotFoundException("File with obj to parse doesn't exist!");
			}

			throw new Exception("Cannot to parse!");
		}

		public Parser()
		{
			_xmlParser = new XmlParser();
			_jsonParser = new JsonParser();
		}
	}
}