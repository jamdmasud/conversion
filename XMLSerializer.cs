using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Converter
{
    public class XMLSerializer
    {
        /// <summary>
        /// Method to get 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize<T>(T obj)
        {
            string xml = string.Empty; 

            if (obj != null)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

                System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
                settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
                settings.Indent = true;
                settings.OmitXmlDeclaration = false;

                using (System.IO.StringWriter textWriter = new System.IO.StringWriter())
                {
                    using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(textWriter, settings))
                    {
                        serializer.Serialize(xmlWriter, obj);
                    }
                    xml = textWriter.ToString();
                }
            }

            return xml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public T Deserialize<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();

            using (System.IO.StringReader textReader = new System.IO.StringReader(xml))
            {
                using (System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }

    }
}
