using Multiplayer.Debugging;
using Multiplayer.Extensions;
using Multiplayer.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.Core
{
    public class LaunchBehaviour : ModBehaviour
    {
        private string GetSteamUsernameText()
        {
            try
            {
                string username = Steamworks.SteamFriends.GetPersonaName();
                return "FirstTimeWindowDescription".LocDef("Thanks for participating in the Open Beta%steamusername%!\nCheckout the wiki on how to start a server, join or play with friends!").SetStringVariable("steamusername", " " + username);
            }
            catch (Exception ex)
            {
                Logging.Warn("[Mod] Couldn't fetch username from Steam! If you've a DRM-Free version thats why. => " + ex.Message);
                return "FirstTimeWindowDescription".LocDef("Thanks for participating in the Open Beta%steamusername%!\nCheckout the wiki on how to start a server, join or play with friends!").SetStringVariable("steamusername", "");
            }
        }
        public static List<GameObject> ActiveObjects = new List<GameObject>() { };
        public override void OnActivate()
        {
            bool firstTimeLaunch = !SettingsHandler.Has("firstTimeLaunch");
            if (firstTimeLaunch)
            //if (true)
            {
                GUIWindow firstTimeWindow = WindowManager.SpawnWindow();
                firstTimeWindow.Show();
                firstTimeWindow.SetTitle("FirstTimeWindowTitle".LocDef("First time? Neat!"));
                firstTimeWindow.ShowCentered = true;
                firstTimeWindow.MinSize = new Vector2(Screen.width / 2 - 5f, 32 + 32 + 5 + 5 + 5);
                Text description = WindowManager.SpawnLabel();
                description.text = GetSteamUsernameText();
                Text description2 = WindowManager.SpawnLabel();
                description2.text = "FirstTimeWindowDescription2".LocDef("The wiki also contains useful info on how to configure your server, set passwords on your server, and lots more! It also contains some useful infomation on how to debug/fix problems related to multiplayer.");
                Button openWikiButton = WindowManager.SpawnButton();
                openWikiButton.GetComponentInChildren<Text>().text = "FirstTimeWindow_OpenWiki".LocDef("Open Wiki");
                openWikiButton.onClick.AddListener(OpenWiki);
                Button closeWindowButton = WindowManager.SpawnButton();
                closeWindowButton.GetComponentInChildren<Text>().text = "FirstTimeWindow_OK".LocDef("Ok");
                closeWindowButton.onClick.AddListener(() => firstTimeWindow.gameObject.SetActive(false));
                firstTimeWindow.SizeButton.SetActive(false);
                WindowManager.AddElementToWindow(description.gameObject, firstTimeWindow, new Rect(5, 15, (Screen.width / 2) - 15f, 32), Rect.zero);
                WindowManager.AddElementToWindow(openWikiButton.gameObject, firstTimeWindow, new Rect(5, (32 * 2) + 25, (Screen.width / 2) - 15f, 60f), Rect.zero);
                WindowManager.AddElementToWindow(closeWindowButton.gameObject, firstTimeWindow, new Rect(5, (32 * 2) + 90, (Screen.width / 2) - 15f, 60f), Rect.zero);
                WindowManager.AddElementToWindow(description2.gameObject, firstTimeWindow, new Rect(5, 42 + 5, (Screen.width / 2) - 15f, 32), Rect.zero);
                SettingsHandler.Set("firstTimeLaunch", false);
            }
        }

        private void OpenWiki()
        {
            try
            {
                Steamworks.SteamFriends.ActivateGameOverlayToWebPage("https://github.com/cal3432/software-inc-multiplayer/wiki");
            }
            catch (Exception ex)
            {
                Logging.Warn("[Mod] Couldn't open webpage in steam overlay! Opening in web browser. If you've a DRM-Free version thats why. => " + ex.Message);
                Process.Start("https://github.com/cal3432/software-inc-multiplayer/wiki");
            }
        }

        public override void OnDeactivate()
        {
            List<GameObject> copy = new List<GameObject>() { };
            foreach (GameObject e in ActiveObjects)
            {
                e.SetActive(false);
                copy.Add(e);
            }
            foreach (GameObject copye in copy)
            {
                ActiveObjects.Remove(copye);
            }
            if (Client.client.Connected)
            {
                Client.Disconnect();
            }
            if (Networking.Server.IsRunning)
            {
                Networking.Server.Stop();
            }
        }
    }
}
