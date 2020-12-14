using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Models.Result;
using System.Reflection;
using System.Data;

namespace ServiceLayer
{
	public class Layer
	{
		public static void GenerateResultFiles(Result<object> res, string path, string name)
		{
			MethodInfo toList = typeof(Layer).GetMethod("ToList", BindingFlags.NonPublic | BindingFlags.Static);
			toList = toList.MakeGenericMethod(res.TypeOfTable);
			var list = toList.Invoke(null, new object[] { res.Table });

			GenerateResultXmlAndXsd(path, name, list);
		}

		private static void GenerateResultXmlAndXsd(string path, string name, object obj)
		{
			using (var xsdStream = new FileStream(Path.Combine(path, name) + ".xsd", FileMode.CreateNew))
			using (var xmlStream = new FileStream(Path.Combine(path,name) + ".xml", FileMode.CreateNew))
			{
				new XmlSerializer(obj.GetType()).Serialize(xmlStream, obj);
				xmlStream.Seek(0, SeekOrigin.Begin);

				var ds = new DataSet();

				ds.ReadXml(xmlStream);
				ds.WriteXmlSchema(xsdStream);
			}
		}

		private static List<T> ToList<T>(IEnumerable<T> smth)
		{
			return (List<T>)smth;
		}
	}
}
