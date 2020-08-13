using Multiplayer.Debugging;
using Multiplayer.Networking;
using Multiplayer.Networking.GameWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Multiplayer.Core
{
	class DebugConsole : ModBehaviour
	{
		bool inmain = false;

		public override void OnActivate()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			Logging.Info("[DebugConsole] Adding console commands");
			DevConsole.Command startservercmd = new DevConsole.Command("MULTIPLAYER_START", OnStartServer);
			DevConsole.Console.AddCommand(startservercmd);
			DevConsole.Command<string, ushort> connectclientcmd = new DevConsole.Command<string, ushort>("MULTIPLAYER_CONNECT", OnClientConnect);
			DevConsole.Console.AddCommand(connectclientcmd);
			DevConsole.Command<string> sendchatcmd = new DevConsole.Command<string>("MULTIPLAYER_CHAT", OnSendChat);
			DevConsole.Console.AddCommand(sendchatcmd);
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if(scene.name == "MainScene")
			{
				inmain = true;
			}
			else
			{
				inmain = false;
			}
		}

		private void OnClientConnect(string ip, ushort port)
		{
			if(!inmain)
			{
				Logging.Warn("[DebugConsole] You can't use this command outside of the MainScene!");
				return;
			}
			Networking.Client.Connect(ip, port);
		}

		private void OnSendChat(string arg0)
		{
			if (!inmain)
			{
				Logging.Warn("[DebugConsole] You can't use this command outside of the MainScene!");
				return;
			}
			Logging.Warn("[DebugConsole] The Chat function is not included in this version!");
		}

		private void OnStartServer()
		{
			if (!inmain)
			{
				Logging.Warn("[DebugConsole] You can't use this command outside of the MainScene!");
				return;
			}
			Networking.Server.Start(53223);
			Networking.Client.Connect("127.0.0.1", 53223);
			//Networking.Client.Send(new Helpers.TcpLogin("daRedLoCo", "test")); This function is not working and gives an exception!
		}

		public override void OnDeactivate()
		{
			Logging.Info("[DebugConsole] Removing console commands");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_START");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_CONNECT");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_CHAT");
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
	}
}
