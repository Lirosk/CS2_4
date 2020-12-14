using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Parsers
{
	class JsonParser : IParser
	{
		public T Parse<T>(string filePath)
		{
			var instance = (T)Activator.CreateInstance(typeof(T));

			string content;

			{
				var forTrim = new StringBuilder(File.ReadAllText(filePath));
				//sorry
				forTrim.Replace("\r", null);
				forTrim.Replace("\n", null);
				//forTrim.Replace(" ", null);
				forTrim.Replace("\t", null);
				forTrim.Replace(@"\\", "\\");

				content = forTrim.ToString();
			}

			return (T)ParseInstance(content, typeof(T));
		}

		private object ParseInstance(string content, Type type)
		{
			object instance = Activator.CreateInstance(type);

			PropertyInfo prop;
			FieldInfo field;

			Type fopType = null; // type of current Field Or Property

			object value = null; // buffer for setting the value
			dynamic fop; // field or property

			var nodes = InstanceToNodesArray(content);

			foreach(var node in nodes)
			{
				prop = type.GetProperty(node.Name);
				field = type.GetField(node.Name);
				fop = null;

				if (prop != null)
				{
					fopType = prop.PropertyType;
					fop = prop;
				}
				else if (field != null)
				{
					fopType = field.FieldType;
					fop = field;
				}
				else
				{
					continue;
				}

				if (node.Depth != 1)
				{
					value = ParseInstance(node.Content, fopType);
				}
				else
				{
					if (fopType == typeof(float) || fopType == typeof(double) || fopType == typeof(decimal))
					{
						value = Convert.ChangeType(node.Content.Replace('.', ','), fopType);
					}
					else
					{
						value = Convert.ChangeType(node.Content, fopType);
					}
				}

				fop?.SetValue(instance, value);
			}

			return instance;
		}

		private Node[] InstanceToNodesArray(string instance)
		{
			var arr = new List<Node>();

			int openedBracesCounter = 0;
			int closedBracesCounter = 0;

			int startOfNodeIntance = 1;

			for (int i = 0; i < instance.Length; i++)
			{
				var a = instance[i];

				if (instance[i] == ',')
				{
					if ((closedBracesCounter + 1 == openedBracesCounter) && (i - 1 > startOfNodeIntance))
					{
						//arr.Add(new Node(instance[startOfNodeIntance..i]));
						arr.Add(new Node(instance.Substring(startOfNodeIntance, i - startOfNodeIntance)));
						startOfNodeIntance = i + 1;
					}
				}
				else if (instance[i] == '{')
				{
					openedBracesCounter++;
				}
				else if (instance[i] == '}')
				{
					closedBracesCounter++;

					if (closedBracesCounter == openedBracesCounter)
					{
						//arr.Add(new Node(instance[startOfNodeIntance..i]));
						arr.Add(new Node(instance.Substring(startOfNodeIntance, i - startOfNodeIntance)));
					}
				}
			}

			return arr.ToArray();
		}

		private struct Node
		{
			public string Name { get; set; }
			public string Content { get; set; }
			public int Depth { get; set; }

			public Node(string field)
			{
				var forTrim = new StringBuilder(field);
				forTrim.Replace("\"", null);
				field = forTrim.ToString();

				var column = field.IndexOf(':');

				Depth = CharCount(field, '{') + 1;
				//Name = field[..column];
				Name = field.Substring(0, column);
				//Content = field[(column + 1)..];
				Content = field.Substring(column + 1, field.Length - column - 1);
			}
		}

		private static int CharCount(string str, char symbol)
		{
			int counter = 0;

			foreach (var ch in str)
			{
				if (symbol == ch)
				{
					counter++;
				}
			}

			return counter;
		}
	}
}
