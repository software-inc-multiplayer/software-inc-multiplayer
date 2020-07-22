using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EasyTcp3;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Multiplayer.Networking
{
    public class SrvClass
    {
        private ushort Port = 52512;
        public ushort MaxPlayers = 0;
        private string Password = "";
        static EasyTcpServer server;
        public static SrvClass instance;
        void Log(string message)
        {
            DevConsole.Console.Log("Sever: " + message);
        }

        public void Start(ushort port = 52512)
        {
            Port = port;
            Log("Starting Server...");
            Log("Port: " + Port);
            Log("Max Players: " + MaxPlayers);
            Log("Password: " + Password);
            server = new EasyTcpServer()
            {
                Serialize = o => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(o)),
                Deserialize = (bytes, type) => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type)
            }.Start(Port);

            server.OnConnect += (sender, client) => Log($"Client connected [ip: {client.GetIp()}]");
            server.OnDisconnect += (sender, client) => Log($"Client disconnected [ip: {client.GetIp()}]");
            server.OnDataReceive += OnDataReceive;
            server.OnDataSend += OnDataSend;
        }

		private void OnDataSend(object sender, Message e)
		{
			//throw new NotImplementedException();
		}

		private void OnDataReceive(object sender, Message e)
		{
            Helpers.MessageData data = e.Decompress().Deserialize<Helpers.MessageData>();
            JObject jo = (JObject)data.Data;
            if(data.DataType == typeof(Helpers.SystemMessage).FullName)
			{
                HandleSysMessage(jo.ToObject<Helpers.SystemMessage>(), e.Client);
			}
            else if(data.DataType == typeof(Helpers.ChatMessage).FullName)
			{
                HandleChatMessage(jo.ToObject<Helpers.ChatMessage>());
			}
		}

        void HandleSysMessage(Helpers.SystemMessage message, EasyTcpClient client)
		{
            if(message.MessageType == Helpers.SysMessageType.Login)
			{
                Helpers.SystemMessage sm;

                string username = message.Data[0] as string;
                string password = message.Data[1] as string;
                long nrole = (long)message.Data[2];

                if (Password != password)
				{
                    Log("Player did use wrong password!");
                    sm = new Helpers.SystemMessage(Helpers.SysMessageType.Login, "wrong_pass");
                    client.Send(sm.AsMessage(), true);
                    return;
				}
                if (MaxPlayers < server.ConnectedClientsCount)
				{
                    Log("Player limit reached!");
                    sm = new Helpers.SystemMessage(Helpers.SysMessageType.Login, "player_limit");
                    client.Send(sm.AsMessage(),true);
                    return;
				}

                Helpers.UserRole role = nrole == 1 ? Helpers.UserRole.Client : Helpers.UserRole.Host;
                Log(username + " is now logged in as " + role + " with IP " + client.GetIp());

                sm = new Helpers.SystemMessage(Helpers.SysMessageType.Login, "ok");
                client.Send(sm.AsMessage(), true);
            }
		}

        void HandleChatMessage(Helpers.ChatMessage message)
		{

		}
	}
}
