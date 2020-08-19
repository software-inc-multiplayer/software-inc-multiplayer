﻿using Multiplayer.Debugging;
using Multiplayer.Networking;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer.Core
{
	class DebugConsole : ModBehaviour
	{
		bool inmain = false;
		public bool rrinuse { get; set; }
		public void PlayRR()
		{
			if (rrinuse) return;
			rrinuse = true;
			string[] lines = File.ReadAllLines(Path.Combine(Meta.ThisMod.ModPath, "Assets", "rr.txt"));
			StartCoroutine(ReadLines(lines));
		}

		private IEnumerator ReadLines(string[] lines)
		{
			foreach(string line in lines)
			{
				Logging.Info(line);
				yield return new WaitForSeconds(0.95f);
			}
			rrinuse = false;
		}

		public override void OnActivate()
		{
			rrinuse = false;
			SceneManager.sceneLoaded += OnSceneLoaded;
			Logging.Info("[DebugConsole] Adding console commands");
			DevConsole.Command<ushort> startservercmd = new DevConsole.Command<ushort>("MULTIPLAYER_START", OnStartServer);
			DevConsole.Console.AddCommand(startservercmd);
			DevConsole.Command<string, ushort> connectclientcmd = new DevConsole.Command<string, ushort>("MULTIPLAYER_CONNECT", OnClientConnect);
			DevConsole.Console.AddCommand(connectclientcmd);
			DevConsole.Command<string> sendchatcmd = new DevConsole.Command<string>("MULTIPLAYER_CHAT", OnSendChat);
			DevConsole.Console.AddCommand(sendchatcmd);
			DevConsole.Command closeserver = new DevConsole.Command("MULTIPLAYER_STOP", OnServerStop);
			DevConsole.Console.AddCommand(closeserver);
			DevConsole.Command getuserlist = new DevConsole.Command("MULTIPLAYER_USERS", OnRequestUserList);
			DevConsole.Console.AddCommand(getuserlist);
			DevConsole.Command getgameworld = new DevConsole.Command("MULTIPLAYER_GAMEWORLD", OnRequestGameWorld);
			DevConsole.Console.AddCommand(getgameworld);
			DevConsole.Command easterEgg = new DevConsole.Command("I_HATE_UI_UGH", PlayRR);
			DevConsole.Console.AddCommand(easterEgg);
		}

		private void OnRequestGameWorld()
		{
			if (!Networking.Client.Connected)
				Logging.Warn("[DebugConsole] You need to be connected to a Server to use this command!");

			Networking.Client.Send(new Helpers.TcpRequest("gameworld"));
		}

		private void OnRequestUserList()
		{
			if (!Networking.Client.Connected)
				Logging.Warn("[DebugConsole] You need to be connected to a Server to use this command!");

			Networking.Client.Send(new Helpers.TcpRequest("userlist"));
		}

		private void OnServerStop()
		{
			Networking.Client.Disconnect();
			Networking.Server.Stop();
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
			if (!inmain || !Networking.Client.Connected)
			{
				Logging.Warn("[DebugConsole] You can't use this command outside of the MainScene!");
				return;
			}
			Networking.Client.Send(new Helpers.TcpChat(arg0));
		}

		private void OnStartServer(ushort port)
		{
			if (!inmain)
			{
				Logging.Warn("[DebugConsole] You can't use this command outside of the MainScene!");
				return;
			}
			Networking.Server.Start(port);
			Networking.Client.Connect("127.0.0.1", port);
		}

		public override void OnDeactivate()
		{
			Logging.Info("[DebugConsole] Removing console commands");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_START");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_CONNECT");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_CHAT");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_STOP");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_USERS");
			DevConsole.Console.RemoveCommand("MULTIPLAYER_GAMEWORLD");
			DevConsole.Console.RemoveCommand("I_HATE_UI_UGH");
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
	}
}
