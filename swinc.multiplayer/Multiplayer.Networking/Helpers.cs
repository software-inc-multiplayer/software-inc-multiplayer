using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Multiplayer.Networking
{
	public static class Helpers
	{
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
		/// Do not use this, but use one of the objects that are based on the message
		/// </summary>
		public class Message
		{
			public string Data = "";
			public Dictionary<object, object> Meta = new Dictionary<object, object>();
		}

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

		public class ChatMessage : Message
		{
			/// <summary>
			/// [CLIENT ONLY] Sends a chatmessage to a receiver, if receiver is empty it will send it to all clients
			/// </summary>
			/// <param name="receiver">The receiver of the chat message, if its empty it will send it to all clients</param>
			/// <param name="message">The chat message of the client</param>
			public ChatMessage(string receiver, string message)
			{
				Data = "chat";
				Meta.Add("receiver", receiver);
				Meta.Add("message", message);
			}
		}

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