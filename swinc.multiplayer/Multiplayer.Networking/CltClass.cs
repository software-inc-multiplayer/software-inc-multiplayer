using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTcp3;
using EasyTcp3.ClientUtils;
using EasyTcp3.EasyTcpPacketUtils;
using EasyTcp3.Server.ServerUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;

namespace Multiplayer.Networking
{
	public class CltClass
	{
        EasyTcpClient client;
        bool isLoggedin = false;

        void Log(string message)
		{
            DevConsole.Console.Log("Client: " + message);
		}

        public void Connect(string server, ushort port, string password = "", Helpers.UserRole userrole = Helpers.UserRole.Client)
        {
            client = new EasyTcpClient()
            {
                Serialize = o => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(o)),
                Deserialize = (bytes, type) => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type)
            };
            client.OnConnect += (sender, client) => Log("Client connected!");
            client.OnDisconnect += (sender, client) => Log("Client disconnected!");
            client.OnDataReceive += OnDataReceive;
            client.OnDataSend += OnDataSend;

            string un = $"Player";
            if (!client.Connect(server, port)) return;
			try
			{
                un = SteamFriends.GetPersonaName();
            }catch(Exception ex)
			{
                Log("Couldn't load Steam Username: " + ex.Message);               
            }

            //Login User
            Log("Log into server...");
            Helpers.SystemMessage loginmsg = new Helpers.SystemMessage(Helpers.SysMessageType.Login,un,password,userrole);
            client.Send(loginmsg.AsMessage(),true);
        }

        private void OnDataReceive(object sender, Message e)
        {
            Helpers.MessageData data = e.Decompress().Deserialize<Helpers.MessageData>();
            JObject jo = (JObject)data.Data;
            if (data.DataType == typeof(Helpers.SystemMessage).FullName)
            {
                HandleSysMessage(jo.ToObject<Helpers.SystemMessage>(), e.Client);
            }
            else if (data.DataType == typeof(Helpers.ChatMessage).FullName)
            {
               // HandleChatMessage(jo.ToObject<Helpers.ChatMessage>());
            }
        }

        void HandleSysMessage(Helpers.SystemMessage message, EasyTcpClient client)
        {
            if (message.MessageType == Helpers.SysMessageType.Login)
            {
                string response = (string)message.Data[0];
                if (response != "ok")
				{
                    Log("Client couldn't login! => " + response);
                    client.Dispose();
                    return;
                }
                Log("Client logged in!");
            }
        }

        private void OnDataSend(object sender, Message message)
        {

        }
    }
}
