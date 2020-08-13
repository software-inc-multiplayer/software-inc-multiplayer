using Multiplayer.Debugging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Tyd;

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
			string path = Path.Combine(ModController.ModFolder, "Multiplayer", "user.id");
			Directory.CreateDirectory(path); //Create path if not exists
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

		public class User
		{
			/// <summary>
			/// The ID of the User inside the server
			/// </summary>
			public ushort ID { get; set; }
			/// <summary>
			/// The (Steam) username of the User
			/// </summary>
			public string Username { get; set; }
			/// <summary>
			/// The UserRole of the user (Host or Client)
			/// </summary>
			public UserRole Role { get; set; }
			/// <summary>
			/// The Ip & Port of the user
			/// </summary>
			public string IpPort { get; set; }
			/// <summary>
			/// The UserCompany of the User, will be set with the User() function
			/// </summary>
			public UserCompany Usercompany { get; set; }

			public User()
			{
				Usercompany = new UserCompany(this);
			}

			/// <summary>
			/// Serialize the User to Json
			/// </summary>
			/// <returns>The serialized User as string</returns>
			public string ToJson()
			{
				return JsonConvert.SerializeObject(this);
			}

			/// <summary>
			/// Deserialize the User from Json
			/// </summary>
			/// <param name="json">The serialized user as json string</param>
			/// <returns>An User object from the deserialized string</returns>
			public static User FromJson(string json)
			{
				return JsonConvert.DeserializeObject<User>(json);
			}
		}

		public class UserCompany : Company
		{
			/// <summary>
			/// The player who owns the company
			/// </summary>
			public User Owner { get; private set; }
			/// <summary>
			/// A list of employees from the company
			/// </summary>
			public List<Employee> Employees { get { return FetchEmployees(); } }

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
		
		/// <summary>
		/// Base message to send over the network. DO NOT USE!
		/// </summary>
		public class TcpMessage
		{
			public string Header = "";
			public Dictionary<object, object> Data = new Dictionary<object, object>();

			public virtual string ToJson()
			{
				return JsonConvert.SerializeObject(this);
			}

			public virtual byte[] ToArray()
			{
				return Encoding.UTF8.GetBytes(ToJson());
			}
		}

		/// <summary>
		/// [Client Only] Login message used to send a login request from the client to the server
		/// </summary>
		public class TcpLogin : TcpMessage
		{
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
				Data.Add("uniqueid", Helpers.GetUniqueID());
			}
		}

		/// <summary>
		/// [Client/Server] GameWorld message used to update the GameWorld. Can be used by the Server and the Client!
		/// </summary>
		public class TcpGameWorld : TcpMessage
		{
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

		/// <summary>
		/// [Server Only] A response from the server (For example a response to a TcpLogin message from the client)
		/// </summary>
		public class TcpResponse : TcpMessage
		{
			/// <summary>
			/// [Server Only] A response from the server (For example a response to a TcpLogin message from the client)
			/// </summary>
			/// <param name="type">The type of the response, for a TcpLogin response it would be "login" for example</param>
			/// <param name="response">The response as a string, if the password for the login is wrong it would be "wrong_password"</param>
			public TcpResponse(string type, string response)
			{
				Header = "response";
				Data.Add("type", type);
				Data.Add("data", response);
			}
		}

		[Obsolete]
		/// <summary>
		/// Do not use this, but use one of the objects that are based on the message
		/// </summary>
		public class Message
		{
			public string Data = "";
			public Dictionary<object, object> Meta = new Dictionary<object, object>();
		}

		[Obsolete]
		public class LoginMessage : Message
		{
			/// <summary>
			/// [CLIENT ONLY] The loginmessage a user sends when trying to login to the server
			/// </summary>
			/// <param name="username">The username of the user (TODO: Steam username)</param>
			/// <param name="password">The server password, if empty it won't have a password</param>
			public LoginMessage(string username, string password)
			{
				Data = "login";
				Meta.Add("username", username);
				Meta.Add("password", password);
			}
		}

		[Obsolete]
		public class ChatMessage : Message
		{
			/// <summary>
			/// [CLIENT ONLY] Sends a chatmessage to a receiver, if receiver is empty it will send it to all clients
			/// </summary>
			/// <param name="receiver">The receiver of the chat message, if its empty it will send it to all clients. If this message is coming from the server, receiver will be the sender</param>
			/// <param name="message">The chat message of the client</param>
			public ChatMessage(string receiver, string message)
			{
				Data = "chat";
				Meta.Add("receiver", receiver);
				Meta.Add("message", message);
			}
		}

		[Obsolete]
		public class ServerMessage : Message
		{
			/// <summary>
			/// [SERVER ONLY] Sends a message from the server to a receiver client
			/// </summary>
			/// <param name="receiver">The receiver of the message, if its empty it will send it to every client</param>
			/// <param name="message">The system message the server sends</param>
			public ServerMessage(string receiver, string message)
			{
				Data = "server";
				Meta.Add("receiver", receiver);
				Meta.Add("message", message);
			}
		}

		[Obsolete]
		public class DataMessage : Message
		{
			/// <summary>
			/// Sends a Datamessage with a Company object within it
			/// </summary>
			/// <param name="receiver">The receiver of the datamessage if its empty its for every client, if its "server" its for the server</param>
			/// <param name="company">The company object that needs to be sent</param>
			public DataMessage(string receiver, Company company)
			{
				Data = "data";
				Meta.Add("receiver", receiver);
				Meta.Add("type", "company");
				Meta.Add("data", JsonConvert.SerializeObject(company));
			}
		}

		[Obsolete]
		public class GameWorldMessage : Message
		{
			/// <summary>
			/// [SERVER ONLY] A message sent if the gameworld gets updated. Contains lists of companies etc
			/// Actual content: Companies
			/// </summary>
			/// <param name="world">The gameworld that you want to be sent to the client</param>
			public GameWorldMessage(GameWorld.World world, bool isAddition)
			{
				Data = "gameworld";
				Meta.Add("add", isAddition);
				Meta.Add("data", JsonConvert.SerializeObject(world));
			}
		}

		public enum SysMessageType
		{
			Login,
			Chat
		}

		public enum UserRole
		{
			Host,
			Client
		}
	}
}