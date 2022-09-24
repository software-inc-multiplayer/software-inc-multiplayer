using Multiplayer.Debugging;
using RoWa;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;

namespace Multiplayer.Networking
{
	public static class Helpers
	{
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

		[ProtoContract]
		public class User
		{
			/// <summary>
			/// The ID of the User inside the server
			/// </summary>
			[ProtoMember(1)]
			public int ID { get; set; }
			/// <summary>
			/// The (Steam) username of the User
			/// </summary>
			[ProtoMember(2)]
			public string Username { get; set; }
			/// <summary>
			/// The UserRole of the user (Host or Client)
			/// </summary>
			[ProtoMember(3)]
			public UserRole Role { get; set; }
			/// <summary>
			/// The Unique User ID
			/// </summary>
			[ProtoMember(4)]
			public string UniqueID { get; set; }

			/// <summary>
			/// The UserCompany of the User, will be set with the User() function
			/// </summary>
			[ProtoMember(5)]
			public UserCompany Usercompany { get; set; }

			public User()
			{
				Usercompany = new UserCompany(this);
			}
		}

		[ProtoContract]
		public class UserCompany : Company
		{
			/// <summary>
			/// The player who owns the company
			/// </summary>
			[ProtoMember(1)]
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
		
		[ProtoContract]
		/// <summary>
		/// Base message to send over the network. DO NOT USE!
		/// </summary>
		public class TcpMessage
		{
			[ProtoMember(1)]
			public string Header = "";

			[ProtoMember(2)]
			public XML.XMLDictionary Data = new XML.XMLDictionary();

			public virtual byte[] Serialize()
			{
				MemoryStream memoryStream = new MemoryStream();
				Serializer.Serialize(memoryStream, this);
				return memoryStream.GetBuffer();
			}

			public static T Deserialize<T>(byte[] array)
			{

				return Serializer.Deserialize<T>(new MemoryStream(array));
			}
		}
		
		[ProtoContract]
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
		}

		[ProtoContract]
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
		}

		[ProtoContract]
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
		}

		public enum TcpServerChatType
		{
			Info,
			Error,
			Warn,
		}

		[ProtoContract]
		public class TcpServerChat : TcpMessage
		{
			public TcpServerChat() { }
			public TcpServerChat(string message, TcpServerChatType type)
			{
				Header = "serverchat";
				Data.Add("message", message);
				Data.Add("type", type);
			}
		}

		[ProtoContract]
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
		}

		[ProtoContract]
		public class TcpRequest : TcpMessage
		{
			public TcpRequest() { }

			public TcpRequest(string request)
			{
				Header = "request";
				Data.Add("request", request);
			}
		}

		[ProtoContract]
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

			public TcpData(string key, string value)
			{
				Header = "data";
				Data.Add(key, value);
			}
		}

		[ProtoContract]
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
		}

		[ProtoContract]
		public enum UserRole
		{
			Host,
			Admin,
			Client
		}
	}
}