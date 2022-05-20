using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Xunit;
using Xunit.Abstractions;

namespace Multiplayer.Networking.Test
{
    public class SteamWorksTests : IDisposable
    {
        private const int Appid = 362620;
        private const int NetworkTimeout = 5000;
        private static int _port = 1024;
        private readonly ushort Port;
        private readonly ITestOutputHelper outputHelper;

        public SteamWorksTests(ITestOutputHelper outputHelper)
        {
            this.Port = (ushort)Interlocked.Increment(ref _port);
            SteamClient.Init(Appid);
            this.outputHelper = outputHelper;
        }

        public void Dispose()
        {
            SteamClient.Shutdown();
        }

        [Fact]
        public void InitServer()
        {
            var gameServerSocket = SteamNetworkingSockets.CreateNormalSocket<TestServer>(NetAddress.AnyIp(this.Port));
            Assert.NotNull(gameServerSocket);
            Assert.True(gameServerSocket.Close());
        }

        [Fact]
        public void InitClient()
        {
            var gameClientSocket = SteamNetworkingSockets.ConnectNormal<TestClient>(NetAddress.LocalHost(this.Port));
            Assert.NotNull(gameClientSocket);
            gameClientSocket.Close();
        }

        [Fact]
        public void InitialConnect()
        {
            var gameServerSocket = SteamNetworkingSockets.CreateNormalSocket<TestServer>(NetAddress.AnyIp(this.Port));

            var gameClientSocket = SteamNetworkingSockets.ConnectNormal<TestClient>(NetAddress.LocalHost(this.Port));

            Assert.True(gameClientSocket.ConnectedEvent.WaitOne(NetworkTimeout));
            Assert.True(gameServerSocket.ConnectedEvent.WaitOne(NetworkTimeout));

            //gameClientSocket.Connection.SendMessage()

            gameClientSocket.Close();

            Assert.True(gameServerSocket.DisconnectedEvent.WaitOne(NetworkTimeout));
            Assert.True(gameClientSocket.DisconnectedEvent.WaitOne(NetworkTimeout));
        }

        [Fact]
        public void SteamAuth()
        {
            var authEnd = new ManualResetEvent(false);
            var gameServerSocket = SteamNetworkingSockets.CreateNormalSocket<TestServer>(NetAddress.AnyIp(this.Port));
            var gameClientSocket = SteamNetworkingSockets.ConnectNormal<TestClient>(NetAddress.LocalHost(this.Port));

            Assert.True(gameClientSocket.ConnectedEvent.WaitOne(NetworkTimeout));
            Assert.True(gameServerSocket.ConnectedEvent.WaitOne(NetworkTimeout));

            SteamServer.OnValidateAuthTicketResponse += (SteamId steamId, SteamId ownerId, AuthResponse response) => {
                Assert.Equal(AuthResponse.OK, response);
                authEnd.Set();
            };

            gameServerSocket.MessageReceived += (Connection c, byte[] data) => {
                Assert.True(SteamServer.BeginAuthSession(data, SteamClient.SteamId));
            };

            var ticket = SteamUser.GetAuthSessionTicket();
            gameClientSocket.Connection.SendMessage(ticket.Data);

            authEnd.WaitOne(NetworkTimeout);

            gameClientSocket.Close();

            Assert.True(gameServerSocket.DisconnectedEvent.WaitOne(NetworkTimeout));
            Assert.True(gameClientSocket.DisconnectedEvent.WaitOne(NetworkTimeout));
        }
    }

    internal class TestServer : SocketManager
    {
        public AutoResetEvent ConnectedEvent = new AutoResetEvent(false);
        public AutoResetEvent DisconnectedEvent = new AutoResetEvent(false);
        public AutoResetEvent MessageEvent = new AutoResetEvent(false);

        public override void OnConnected(Connection connection, ConnectionInfo info)
        {
            ConnectedEvent.Set();
        }

        public override void OnConnecting(Connection connection, ConnectionInfo info)
        {
            connection.Accept();
        }

        public override void OnDisconnected(Connection connection, ConnectionInfo info)
        {
            DisconnectedEvent.Set();
        }

        public event Action<Connection, byte[]> MessageReceived;
        public byte[] receiveBuffer = new byte[1024 * 10];
        public unsafe override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            MessageEvent.Set();

            void* byteData = (void*)data;
            void* rcvBuffer = (void*)receiveBuffer[0];

            Unsafe.CopyBlock(rcvBuffer, byteData, (uint)size);

            //fixed(byte* rcvBuffer = receiveBuffer) {
                //Unsafe.CopyBlock(ref receiveBuffer, rcvBuffer, size);
            //}

            MessageReceived?.Invoke(connection, receiveBuffer);
        }
    }

    internal class TestClient : ConnectionManager
    {
        public AutoResetEvent ConnectedEvent = new AutoResetEvent(false);
        public AutoResetEvent DisconnectedEvent = new AutoResetEvent(false);
        public AutoResetEvent MessageEvent = new AutoResetEvent(false);
        public override void OnConnected(ConnectionInfo info)
        {
            ConnectedEvent.Set();
        }

        public override void OnConnecting(ConnectionInfo info)
        {
        }

        public override void OnDisconnected(ConnectionInfo info)
        {
            DisconnectedEvent.Set();
        }

        public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            MessageEvent.Set();
        }
    }
}
