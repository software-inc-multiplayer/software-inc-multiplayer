using EasyTcp3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Multiplayer.Networking
{
	public static class Helpers
	{
		public class User
		{
			public string Username { get; set; }
			public UserRole Role { get; set; }

			public string ToJson()
			{
				return JsonConvert.SerializeObject(this);
			}

			public static User FromJson(string json)
			{
				return JsonConvert.DeserializeObject<User>(json);
			}
		}

		public class ChatMessage
		{
			public string Sender { get; set; }
			public string Receiver { get; set; }
			public string Chatmessage { get; set; }
			public MessageData AsMessage()
			{
				return MessageData.FromObject(this);
			}
		}

		public class SystemMessage
		{
			public SysMessageType MessageType { get; set; }
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
			public string DataType { get; set; }
			public object Data { get; set; }

			public static MessageData FromObject<T>(T obj)
			{
				return new MessageData() { Data = obj, DataType = typeof(T).FullName };
			}
		}

		public enum SysMessageType
		{
			Login
		}

		public enum UserRole
		{
			Host,
			Client
		}
	}
}