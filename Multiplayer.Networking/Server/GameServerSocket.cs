using System;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Multiplayer.Debugging;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Server
{
    public class GameServerSocket : SocketManager, IDisposable
    {
        private ILogger log;


        public GameServerSocket()
        {
            this.log = new FileLogger();
        }

        public override void OnConnectionChanged(Connection connection, ConnectionInfo info)
        {
            base.OnConnectionChanged(connection, info);
        }


        public override void OnConnected(Connection connection, ConnectionInfo info)
        {
            log.Debug($"Connection: {connection.ToString()}, connectionInfo: {info.Address}");
            log.Debug($"Connected!");
        }

        public override void OnConnecting(Connection connection, ConnectionInfo info)
        {
            log.Debug($"Connection: {connection.ToString()}, connectionInfo: {info.Address}");
            var result = connection.Accept();
            log.Debug($"Accepted");
        }

        public override void OnDisconnected(Connection connection, ConnectionInfo info)
        {
            log.Debug($"Connection: {connection.ToString()}, connectionInfo: {info.Address}");
            var result = connection.Close();
            log.Debug($"Disconnected and Closed");
        }

        public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            log.Debug($"Connection: {connection.ToString()}, identity: {identity.Address}");
            log.Debug($"On Message, size: {size}, messageNum: {messageNum}");
        }

        public void Dispose()
        {
            try
            {
                foreach (var connection in Connected)
                {
                    connection.Close();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                log.Error(ex);

            }
        }
    }
}
