using System;

using Multiplayer.Networking.Utility;
using Multiplayer.Shared;
using Telepathy;
using Packets;
using System.Threading;
using Multiplayer.Networking.Shared;

namespace Multiplayer.Networking.Client
{
    public class GameClient : IDisposable
    {
        #region Events
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler ConnectionReady;
        #endregion

        private readonly ILogger logger;
        private readonly PacketSerializer packetSerializer;
        public IUserManager UserManager { get; }
        private readonly GameUser gameUser;

        public GameClient(ILogger logger, GameUser user, PacketSerializer packetSerializer, IUserManager userManager)
        {
            this.logger = logger;
            this.packetSerializer = packetSerializer;
            this.UserManager = userManager;
            this.RawClient = new Telepathy.Client();
            this.gameUser = user;
        }

        public void Dispose()
        {
            // disconnect the client
            this.Disconnect();
            // clear all events
            this.ClientConnected = null;
            this.ClientDisconnected = null;
        }

        public Telepathy.Client RawClient { get; set; }

        public void Send(IPacket packet)
        {
            // maybe add a check if we are still connected
            if (!this.RawClient.Send(this.packetSerializer.SerializePacket(packet)))
            {
                this.logger.Error("could not send packet");
            }
        }

        private void HandleWelcomeUser(WelcomeUser welcomeUser)
        {
            this.logger.Debug("[client] welcome client", welcomeUser.Sender, welcomeUser.UserName);

            var newUser = this.UserManager.GetOrAddUser(new GameUser()
            {
                Id = welcomeUser.Sender,
                Name = welcomeUser.UserName,
            });

            if (welcomeUser.Sender == this.gameUser.Id)
                this.ConnectionReady?.Invoke(this, null);
        }

        private void HandleDisconnect(Disconnect disconnect)
        {
            var disconnectedUserId = disconnect.Sender;

            if(disconnectedUserId == 0UL || disconnectedUserId == this.gameUser.Id)
            {
                // duh something bad happened and this client is doomed...
                this.RawClient.Disconnect();

                this.UserManager.Clear();
                return;
            }

            this.UserManager.RemoveUser(disconnectedUserId);
            this.logger.Debug("[client] removing client", disconnectedUserId);
        }

        private void InternalHandleMessage(Message msg)
        {
            switch (msg.eventType)
            {
                case EventType.Connected:
                    this.ClientConnected?.Invoke(this, new ClientConnectedEventArgs(msg.connectionId));

                    this.Send(new Handshake(this.gameUser.Id, this.gameUser.Name));
                    this.logger.Debug("[client] connected to server");
                    break;
                case EventType.Data:

                    // TODO check msg.connectionId. It should not change

                    var packet = this.packetSerializer.DeserializePacket(msg.data);
                    if (packet == null)
                    {
#if DEBUG
                        // maybe add some more details
                        this.logger.Warn("received unknown packet");
#endif
                        break;
                    }

                    if (packet is WelcomeUser welcomeUser)
                    {
                        this.HandleWelcomeUser(welcomeUser);
                        break;
                    }

                    if (packet is Disconnect disconnect)
                    {
                        this.HandleDisconnect(disconnect);
                        break;
                    }

                    break;
                case EventType.Disconnected:

                    this.UserManager.Clear();
                    this.ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(msg.connectionId));
                    this.logger.Debug("[client] disconnected from server");
                    break;
            }
        }

        public bool HandleMessages()
        {
            var hadMessage = false;

            while (this.RawClient.GetNextMessage(out Message msg))
            {
                hadMessage = true;
                this.InternalHandleMessage(msg);
            }
            return hadMessage;
        }

        public void Connect(string ip, int port)
        {
            this.RawClient.Connect(ip, port);
        }

        public void Disconnect()
        {
            // TODO this send does not really work as the disconnect kills the connection
            this.Send(new Disconnect(this.gameUser.Id, DisconnectReason.Leaving));
            //this.RawClient.Disconnect();
        }
    }
}
