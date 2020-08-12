using Multiplayer.Debugging;
using Multiplayer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Core
{
	class DebugConsole : ModBehaviour
	{
		public static ServerClass server = new ServerClass();
		public static ClientClass client = new ClientClass();

		public override void OnActivate()
		{
			Logging.Info("[DebugConsole] Adding console commands");
			DevConsole.Command startservercmd = new DevConsole.Command("MULTIPLAYER_START", OnStartServer);
			DevConsole.Console.AddCommand(startservercmd);
			DevConsole.Command<string> connectclientcmd = new DevConsole.Command<string>("MULTIPLAYER_CONNECT", OnClientConnect);
			DevConsole.Console.AddCommand(connectclientcmd);
			DevConsole.Command<string> sendchatcmd = new DevConsole.Command<string>("MULTIPLAYER_CHAT", OnSendChat);
			DevConsole.Console.AddCommand(sendchatcmd);
		}

		private void OnClientConnect(string arg0)
		{
			if (ServerClass.Instance != null)
				ServerClass.Instance.Dispose();
			if (ClientClass.Instance != null)
				ClientClass.Instance.Dispose();
			client.Connect(arg0);
		}

		private void OnSendChat(string arg0)
		{
			client.SendChatMessage("", arg0);
		}

		private void OnStartServer()
		{
			server.Start();
			client.Connect("127.0.0.1");
		}

		public override void OnDeactivate()
		{
			Logging.Info("[DebugConsole] Removing console commands");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_START");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_CONNECT");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_CHAT");
		}
	}
}
