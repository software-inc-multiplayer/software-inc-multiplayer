using Multiplayer.Debugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Multiplayer.Networking
{
    public static class Helpers
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
                {
                    Logging.Error("[Helpers] Unknown exception while deserializing the array! => " + ex.Message);
                }

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

        [Serializable]
        public class User
        {
            /// <summary>
            /// The ID of the User inside the server
            /// </summary>
            public int ID { get; set; }
            /// <summary>
            /// The (Steam) username of the User
            /// </summary>
            public string Username { get; set; }
            /// <summary>
            /// The UserRole of the user (Host or Client)
            /// </summary>
            public UserRole Role { get; set; }
            /// <summary>
            /// The Unique User ID
            /// </summary>
            public string UniqueID { get; set; }

            /// <summary>
            /// The UserCompany of the User, will be set with the User() function
            /// </summary>
            public UserCompany Usercompany { get; set; }

            public User()
            {
                Usercompany = new UserCompany(this);
            }
#pragma warning disable IDE0060 // Remove unused parameter
            public User(bool placebo)
#pragma warning restore IDE0060 // Remove unused parameter
            {

            }
        }

        [Serializable]
        public class UserCompany : Company
        {
            /// <summary>
            /// The player who owns the company
            /// </summary>
            public User Owner { get; private set; }

            public UserCompany() { }

            public UserCompany(User owner)
            {
                Owner = owner;
                FetchEmployees();
            }

            /// <summary>
            /// Fetches the employees from the company the User (owner) owns
            /// </summary>
            public List<Employee> FetchEmployees()
            {
                //TODO: Fetch the employees from the users company
                return null;
            }
        }


        [Serializable]
        public enum UserRole
        {
            Host,
            Admin,
            Client
        }
    }
}