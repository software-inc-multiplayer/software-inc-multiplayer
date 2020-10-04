using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telepathy;
using Multiplayer.Debugging;
using Multiplayer.Networking;
using Multiplayer.Extensions;

namespace Multiplayer.Networking
{
    public partial class ServerClass
    {
        #region Events
        public event EventHandler ServerStarted;
        public event EventHandler ServerStopped;
        #endregion
        public static ServerClass instance;
        public Telepathy.Server RawServer { get; set; }
        public UserManager UserManager { get; set; }
        public ServerInfo ServerInfomation { get; set; }
        public void Start(int port, string password = "")
        {
            RawServer = new Telepathy.Server();
            ServerInfomation = new ServerInfo()
            {
                Port = port,
                Password = password,
                Host = new User(),
            };
            RawServer.Start(port);
            HandleMessages();
            ServerStarted.Invoke(this, null);
        }
        public void HandleMessages()
        {
            Message data;
            while(RawServer.GetNextMessage(out data))
            {
                switch (data.eventType)
                {
                    case Telepathy.EventType.Connected:
                        OnClientConnect(data);
                        break;
                    case Telepathy.EventType.Data:

                        break;
                    case Telepathy.EventType.Disconnected:

                        break;
                }
            }
        }
    }
}
