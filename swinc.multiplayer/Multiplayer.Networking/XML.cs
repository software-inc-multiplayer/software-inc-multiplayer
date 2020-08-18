using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Multiplayer.Debugging;

namespace RoWa
{
    public static class XML
    {
        [Serializable]
        [Obsolete]
        /// <summary>
        /// A Dictionary that can be saved to an XML file/string
        /// </summary>
        public class XMLDictionary
		{
            [XmlElement]
            public List<XMLDictionaryPair> Pairs = new List<XMLDictionaryPair>();
            public XMLDictionary() { }
            public XMLDictionary(Dictionary<object, object> dict)
			{
                foreach(KeyValuePair<object, object> kvp in dict)
                    Pairs.Add(new XMLDictionaryPair(kvp.Key, kvp.Value));
			}
            public XMLDictionary(params XMLDictionaryPair[] pairs)
			{
                Pairs.AddRange(pairs);
			}

            public bool Contains(object key)
			{
                if (Pairs.Find(x => x.Key == key) != null)
                    return true;
                else
                    return false;
			}

            public object GetValue(object key)
			{
                XMLDictionaryPair p = Pairs.Find(x => x.Key == key);
                if(p == null)
				{
                    Logging.Warn($"[XML] Couldn't find object with key {key} inside Pairs! <= Can be ignored if mod doesn't crash afterwards");
                    return null;
				}
                object value = p.Value;
                return value;
			}

            public object GetValue(string key)
			{
                XMLDictionaryPair p = Pairs.Find(x => (string)x.Key == key);
                if (p == null)
                {
                    Logging.Warn($"[XML] Couldn't find object with key {key} inside Pairs!");
                    return null;
                }
                return p.Value;
            }

            public void Add(XMLDictionaryPair pair)
			{
                Pairs.Add(pair);
			}

            public void Add(object key, object value)
			{
                Add(new XMLDictionaryPair(key, value));
			}
		}

        [Serializable]
        /// <summary>
        /// A keyvaluepair of a XMLDictionary
        /// </summary>
        public class XMLDictionaryPair
		{
            [XmlElement]
            public object Key;
            [XmlElement]
            public object Value;
            public XMLDictionaryPair() { }
            public XMLDictionaryPair(object key, object value) { Key = key; Value = value; }
		}

        [Obsolete]
        /// <summary>
        /// Transforms the object of type T into a XML string
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj">The object</param>
        /// <returns>An XML representation fo the string</returns>
        public static string To<T>(T obj)
		{
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                using (StreamWriter stream = new StreamWriter(ms, Encoding.GetEncoding("UTF-8")))
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Indent = false }))
                    {
                        serializer.Serialize(xmlWriter, obj);
                    }
                    ms.Position = 0;
                    return new StreamReader(ms).ReadToEnd();
                }
            }
        }

        [Obsolete]
        /// <summary>
        /// Transforms the XML string to an object of type T
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="xml">The XML string</param>
        /// <returns>The Object of type T from the XML string</returns>
        public static T From<T>(string xml)
		{
			try
			{
                using (MemoryStream ms = new MemoryStream(1024))
                {
                    ms.Write(Encoding.UTF8.GetBytes(xml), 0, Encoding.UTF8.GetBytes(xml).Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    //fix for encoding:
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    using (StreamReader stream = new StreamReader(ms, Encoding.GetEncoding("UTF-8"))) //using fires stream.close
                    {
                        ms.Position = 0;
                        object o;
                        o = serializer.Deserialize(stream);
                        return (T)o;
                    }
                }
            }
            catch(Exception ex)
			{
                Logging.Warn("[XML] " + ex.Message);
                return default(T);
			}          
        }
	}
}
