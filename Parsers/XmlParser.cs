using System;
using System.Reflection;
using System.Xml;

namespace Parsers
{
    class XmlParser: IParser
    {
        public T Parse<T>(string filePath)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath); // contains FileNotFoundException

            XmlElement xRoot = xDoc.DocumentElement;
            return (T)ParseInstance(xRoot, typeof(T));
        }

        private object ParseInstance(XmlNode xRoot, Type type)
        {
            object instance = Activator.CreateInstance(type);

            PropertyInfo prop;
            FieldInfo field;

            Type fopType = null; // type of current Field Or Property

            object value = null; // buffer for setting the value
            dynamic fop; // field or property

            foreach (XmlNode xNode in xRoot)
            {
                prop = type.GetProperty(xNode.Name);
                field = type.GetField(xNode.Name);
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

                if (xNode.ChildNodes.Count != 1)
                {
                    value = ParseInstance(xNode, fopType);
                }
                else
                {
                    value = Convert.ChangeType(xNode.InnerText, fopType);
                }

                fop?.SetValue(instance, value);
            }

            return instance;
        }
    }
}
