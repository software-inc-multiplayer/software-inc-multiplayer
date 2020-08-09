using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer.Debugging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;
using WatsonTcp;

namespace Multiplayer.Networking
{
    public static class ClientClass
    {

        static bool isLoggedin = false;
        static WatsonTcpClient client;

        public static async void Connect(string ip, ushort port = 52512)
        {
            client = new WatsonTcpClient(ip, port);
            client.ServerConnected += ServerConnected;
            client.ServerDisconnected += ServerDisconnected;
            client.MessageReceived += MessageReceived;
            client.SyncRequestReceived = SyncRequestReceived;
            client.Start();

            // check connectivity
            Logging.Info("[Client] Am I Connected? " + client.Connected);

            // send a message
            client.Send("Hello!");
            // send a message with metadata
            Dictionary<object, object> md = new Dictionary<object, object>();
            md.Add("foo", "bar");
            client.Send(md, "Hello, client!  Here's some metadata!");

            // send async!
            await client.SendAsync("Hello, client!  I'm async!");

            // send and wait for a response
            try
            {
                SyncResponse resp = client.SendAndWait(5000, "Hey, say hello back within 5 seconds!");
                Logging.Info("[Client] My friend says: " + Encoding.UTF8.GetString(resp.Data));
            }
            catch (TimeoutException)
            {
                Logging.Info("[Client] Too slow...");
            }
            //Helpers.SystemMessage sysmsg = new Helpers.SystemMessage(Helpers.SysMessageType.Login, "User", Helpers.UserRole.Host);
            //client.Send(sysmsg.AsMessage().ToJson());
        }

        static void MessageReceived(object sender, MessageReceivedFromServerEventArgs args)
        {
            Logging.Info("[Client] Message from server: " + Encoding.UTF8.GetString(args.Data));
        }

        static void ServerConnected(object sender, EventArgs args)
        {
            Logging.Info("[Client] Server connected");
        }

        static void ServerDisconnected(object sender, EventArgs args)
        {
            Logging.Error("[Client] Server disconnected");
        }

        static SyncResponse SyncRequestReceived(SyncRequest req)
        {
            return new SyncResponse(req, "Hello back at you!");
        }
    }
}
