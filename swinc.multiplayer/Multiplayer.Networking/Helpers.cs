using Multiplayer.Debugging;
using RoWa;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.Xml;
using System.Xml.Serialization;

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
			try
			{
				if (obj == null)
				{
					throw new NullReferenceException("Can't serialize object, because object is NULL!");
				}
				var bf = new BinaryFormatter();
				using (var ms = new MemoryStream())
				{
					bf.Serialize(ms, obj);
					return ms.ToArray();
				}
			}
			catch(Exception ex)
			{
				Logging.Error(ex.Message, ex.StackTrace);
				return null;
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
				if(ex.HResult != -2147467262)
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
			if(File.Exists(path))
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
				//Usercompany = new UserCompany(this);
			}

			public bool IsAdmin()
			{
				if (Role == UserRole.Host || Role == UserRole.Admin)
					return true;
				return false;
			}

			public bool IsHost()
			{
				if (Role == UserRole.Host)
					return true;
				return false;
			}
		}

		[Serializable]
		public class UserCompany
		{
			public Company company;

			/// <summary>
			/// The player who owns the company
			/// </summary>
			public int Owner { get; set; }
			
			public UserCompany() {
				company = new Company();
			}

			public UserCompany(Company comp)
			{
				company = comp;
			}

			public UserCompany(User owner)
			{
				Owner = owner.ID;
				company = new Company();
			}
		}
		
		[Serializable]
		/// <summary>
		/// Base message to send over the network. DO NOT USE!
		/// </summary>
		public class TcpMessage
		{
			public string Header = "";
			[XmlElement]
			public XML.XMLDictionary Data = new XML.XMLDictionary();

			public virtual byte[] Serialize()
			{
				return Helpers.Serialize(this);
			}

			public static TcpMessage Deserialize(byte[] array)
			{
				return Helpers.Deserialize<TcpMessage>(array);
			}
		}

		[Serializable]
		/// <summary>
		/// [Client Only] Login message used to send a login request from the client to the server
		/// </summary>
		public class TcpLogin : TcpMessage
		{
			public TcpLogin()
			{
			}

			/// <summary>
			/// [Client Only] Login message used to send a login request from the client to the server.
			/// Uses Helpers.GetUniqueID() as uniqueid to identify the user.
			/// </summary>
			/// <param name="username">The username which will be saved with the server</param>
			/// <param name="password">The servers password</param>
			public TcpLogin(string username, string password)
			{
				Header = "login";
				Data.Add("username", username);
				Data.Add("password", password);
				Data.Add("uniqueid", GetUniqueID());
			}

			public override byte[] Serialize()
			{
				return Helpers.Serialize(this);
			}

			new public static TcpLogin Deserialize(byte[] array)
			{
				return Helpers.Deserialize<TcpLogin>(array);
			}
		}

		[Serializable]
		/// <summary>
		/// [Client/Server] GameWorld message used to update the GameWorld. Can be used by the Server and the Client!
		/// </summary>
		public class TcpGameWorld : TcpMessage
		{
			public TcpGameWorld()
			{
			}

			/// <summary>
			/// [Client/Server] GameWorld message used to update the GameWorld. Can be used by the Server and the Client!
			/// </summary>
			/// <param name="worldchanges">GameWorld.Server.CompareWorlds() for Server or a new GameWorld.World with the changes for the client</param>
			/// <param name="isAddition">If the content from worldchanges should be added or removed from the GameWorld</param>
			public TcpGameWorld(GameWorld.World worldchanges, bool isAddition)
			{
				Header = "gameworld";
				Data.Add("addition", isAddition);
				Data.Add("changes", worldchanges);
			}

			public override byte[] Serialize()
			{
				return Helpers.Serialize(this);
			}

			new public static TcpGameWorld Deserialize(byte[] array)
			{
				return Helpers.Deserialize<TcpGameWorld>(array);
			}
		}

		[Serializable]
		/// <summary>
		/// [Server Only] A response from the server (For example a response to a TcpLogin message from the client)
		/// </summary>
		public class TcpResponse : TcpMessage
		{
			public TcpResponse() { }

			/// <summary>
			/// [Server Only] A response from the server (For example a response to a TcpLogin message from the client)
			/// </summary>
			/// <param name="type">The type of the response, for a TcpLogin response it would be "login" for example</param>
			/// <param name="response">The response as a string, if the password for the login is wrong it would be "wrong_password"</param>
			public TcpResponse(string type, object response)
			{
				Header = "response";
				Data.Add("type", type);
				Data.Add("data", response);
			}

			public override byte[] Serialize()
			{
				return Helpers.Serialize(this);
			}

			new public static TcpResponse Deserialize(byte[] array)
			{
				return Helpers.Deserialize<TcpResponse>(array);
			}
		}

		[Serializable]
		public class TcpChat : TcpMessage
		{
			public TcpChat() { }
			public TcpChat(User receiver, string message, User sender = null)
			{
				Header = "chat";
				Data.Add("sender", sender);
				Data.Add("receiver", receiver.ID);
				Data.Add("message", message);
			}

			public TcpChat(string receivername, string message)
			{
				Logging.Warn("[Helpers] TcpChat(receivername, message) is non functional!");
			}

			public TcpChat(string message, User sender = null)
			{
				Header = "chat";
				Data.Add("sender", sender);
				Data.Add("receiver", null);
				Data.Add("message", message);
			}

			public TcpChat(int receiverid, string message, User sender = null)
			{
				Header = "chat";
				Data.Add("sender", sender);
				Data.Add("receiver", receiverid);
				Data.Add("message", message);
			}

			public TcpChat(int receiverid, string message, int senderid)
			{
				Header = "chat";
				Data.Add("sender", senderid);
				Data.Add("receiver", receiverid);
				Data.Add("message", message);
			}

			public override byte[] Serialize()
			{
				return Helpers.Serialize(this);
			}

			new public static TcpChat Deserialize(byte[] array)
			{
				return Helpers.Deserialize<TcpChat>(array);
			}
		}

		[Serializable]
		public class TcpRequest : TcpMessage
		{
			public TcpRequest() { }

			public TcpRequest(string request)
			{
				Header = "request";
				Data.Add("request", request);
			}

			public override byte[] Serialize()
			{
				return Helpers.Serialize(this);
			}

			new public static TcpRequest Deserialize(byte[] array)
			{
				return Helpers.Deserialize<TcpRequest>(array);
			}
		}

		[Serializable]
		public class TcpData : TcpMessage
		{
			public TcpData() { }

			public TcpData(Dictionary<object,object> dict)
			{
				Header = "data";
				Data = new XML.XMLDictionary(dict);
			}

			public TcpData(params string[] keyvalues)
			{
				Header = "data";
				foreach(string keyvalue in keyvalues)
				{
					string k = keyvalue.Split('|')[0];
					string v = keyvalue.Split('|')[1];
					Data = new XML.XMLDictionary();
					Data.Add(k, v);
				}
			}

			public TcpData(object key, object value)
			{
				Header = "data";
				Data.Add(key, value);
			}

			public override byte[] Serialize()
			{
				return Helpers.Serialize(this);
			}

			new public static TcpData Deserialize(byte[] array)
			{
				return Helpers.Deserialize<TcpData>(array);
			}
		}

		[Serializable]
		public class TcpGamespeed : TcpMessage
		{
			public TcpGamespeed() {}
			/// <summary>
			/// Todo
			/// </summary>
			/// <param name="speed">The GameTime.GameSpeed object</param>
			/// <param name="type">The type of request. 0 = skip vote (should only be used by server/admin. 1 = request</param>
			public TcpGamespeed(int speed, int type = 0)
			{
				Header = "gamespeed";
				Data.Add("type", type);
				Data.Add("speed", speed);
			}

			public override byte[] Serialize()
			{
				return Helpers.Serialize(this);
			}

			new public static TcpGamespeed Deserialize(byte[] array)
			{
				return Helpers.Deserialize<TcpGamespeed>(array);
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