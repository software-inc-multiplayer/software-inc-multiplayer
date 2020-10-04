using Multiplayer.Debugging;
using RoWa;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Multiplayer.Networking
{
    public class TCPDatas
    {
        /// <summary>
        /// Never use.
        /// </summary>
        [Serializable]
        public class TCPMessage
        {
            public string Header { get; set; }
            [XmlElement]
            public XML.XMLDictionary Data = new XML.XMLDictionary();
            public byte[] Serialize()
            {
                return Utils.Serialize(this);
            }
        }
        public class TCPLoginRequest : TCPMessage
        {
            public TCPLoginRequest(bool PasswordEnabled)
            {
                Header = "LoginRequest";
                Data.Add("HasPassword", PasswordEnabled);
            }
            public static TCPLoginRequest Deserialize(byte[] bytes)
            {
                return Utils.Deserialize<TCPLoginRequest>(bytes);
            }
        }
        /// <summary>
        /// Client Only
        /// </summary>
        [Serializable]
        public class TCPLoginResponse : TCPMessage
        {
            public TCPLoginResponse()
            {
                Header = "LoginResponse";
                Data.Add("User", ClientClass.MyUser);
            }
            public static TCPLoginResponse Deserialize(byte[] bytes)
            {
                return Utils.Deserialize<TCPLoginResponse>(bytes);
            }
        }
    }
    public static class Utils
    {
        /// <summary>
        /// Serialize an object to a byte array
        /// </summary>
        /// <param name="obj">The object you want to serialize</param>
        /// <returns>A byte array representation of the object</returns>
        public static byte[] Serialize(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserialize a byte array back to an object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="byteArray">The byte array you want to deserialize</param>
        /// <returns>The object</returns>
        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            try
            {
                if (byteArray == null)
                {
                    return null;
                }
                using (var memStream = new MemoryStream())
                {
                    var binForm = new BinaryFormatter();
                    memStream.Write(byteArray, 0, byteArray.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    var obj = (T)binForm.Deserialize(memStream);
                    return obj;
                }
            }
            catch (SerializationException ex)
            {
                Logging.Warn("[Helpers] SerializationException thrown while deserializing, probably harmless... => " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                //Workaround, will maybe break the whole thing ^^
                if (ex.HResult != -2147467262)
                    Logging.Error("[Helpers] Unknown exception while deserializing the array! => " + ex.Message);
                return null;
            }

        }

        /// <summary>
        /// returns the unique user id, if it doesn't exist it will create one and return it
        /// </summary>
        /// <returns>The unique User ID</returns>
        public static string GetUniqueID()
        {
            string uid;
            string path = Path.Combine(ModController.ModFolder, "Multiplayer");
            Directory.CreateDirectory(path); //Create path if not exists
            path = Path.Combine(path, "user.id");
            if (File.Exists(path))
            {
                uid = File.ReadAllText(path);
            }
            else
            {
                uid = Guid.NewGuid().ToString();
                File.WriteAllText(path, uid);
            }
            return uid;
        }
    }
}
