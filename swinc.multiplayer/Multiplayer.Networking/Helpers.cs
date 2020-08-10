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
		[Obsolete("Use Multiplayer.Debugging.Logging instead.")]
		public static void Log(string from, string message)
		{
			DevConsole.Console.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + " " + from + ": " + message);
		}

		public class User
		{
			/// <summary>
			/// The (Steam) username of the User
			/// </summary>
			public string Username { get; set; }
			/// <summary>
			/// The UserRole of the user (Host or Client)
			/// </summary>
			public UserRole Role { get; set; }
			/// <summary>
			/// The TCP Client of the user
			/// </summary>
			public object TcpClient { get; set; }
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

		public class ChatMessage
		{
			/// <summary>
			/// The receiver of the chatmessage
			/// </summary>
			public string Receiver { get; set; }
			/// <summary>
			/// The message itself
			/// </summary>
			public string Chatmessage { get; set; }

			public MessageData AsMessage()
			{
				return MessageData.FromObject(this);
			}
		}

		public class SystemMessage
		{
			/// <summary>
			/// The type of the system message (Login/Chat)
			/// </summary>
			public SysMessageType MessageType { get; set; }
			/// <summary>
			/// the data of the system message. The content depends on the type
			/// </summary>
			public List<object> Data { get; set; }

			public SystemMessage(SysMessageType messageType, params object[] data)
			{
				MessageType = messageType;
				Data = new List<object>();
				Data.AddRange(data);
			}

			public MessageData AsMessage()
			{
				return MessageData.FromObject(this);
			}
		}

		public class MessageData
		{
			/// <summary>
			/// The type of the data as string
			/// </summary>
			public string DataType { get; set; }
			/// <summary>
			/// The data itself as object
			/// </summary>
			public object Data { get; set; }

			public static MessageData FromObject<T>(T obj)
			{
				return new MessageData() { Data = obj, DataType = typeof(T).FullName };
			}

			public static MessageData FromJson(string json)
			{
				return JsonConvert.DeserializeObject<MessageData>(json);
			}

			public string ToJson()
			{
				return JsonConvert.SerializeObject(this);
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