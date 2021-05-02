using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Steamworks;
using Steamworks.Data;
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
        public void Test()
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
        public void AppDomainChecks()
        {
            var domain = AppDomain.CurrentDomain;
            var assembly = Assembly.GetExecutingAssembly();

            domain.AssemblyResolve += Domain_AssemblyResolve;
            domain.AssemblyLoad += Domain_AssemblyLoad;

            var references = assembly.GetReferencedAssemblies();
            //assembly.
        }

        private void Domain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            outputHelper.WriteLine($"{args.LoadedAssembly.FullName} AssemblyLoad");
        }

        private Assembly Domain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            outputHelper.WriteLine($"{args.RequestingAssembly.FullName} AssemlbyResolve {args.Name}");
            return null;
        }
    }

    internal class TestServer : SocketManager
    {
        public ManualResetEvent ConnectedEvent = new ManualResetEvent(false);
        public ManualResetEvent DisconnectedEvent = new ManualResetEvent(false);
        public ManualResetEvent MessageEvent = new ManualResetEvent(false);

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

        public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            MessageEvent.Set();
        }
    }

    internal class TestClient : ConnectionManager
    {
        public ManualResetEvent ConnectedEvent = new ManualResetEvent(false);
        public ManualResetEvent DisconnectedEvent = new ManualResetEvent(false);
        public ManualResetEvent MessageEvent = new ManualResetEvent(false);
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
