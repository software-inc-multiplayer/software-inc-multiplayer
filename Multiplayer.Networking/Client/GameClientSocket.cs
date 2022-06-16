using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Multiplayer.Debugging;
using Multiplayer.Networking.Shared.Managers;
using Multiplayer.Packets;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Client
{
    public class GameClientSocket : ConnectionManager, IDisposable
    {
        private readonly ILogger log;

        public GameClient Parent;

        private readonly ArrayPool<byte> bufferPool = ArrayPool<byte>.Create();

        public GameClientSocket()
        {
            log = new UnityLogger();
        }

        public override void OnConnected(ConnectionInfo info)
        {
            base.OnConnected(info);
        }

        public override void OnConnecting(ConnectionInfo info)
        {
            base.OnConnecting(info);
        }


        //base implementation is good enaugh
        /*public override void OnConnectionChanged(ConnectionInfo info)
        {
            base.OnConnectionChanged(info);
        }*/

        public override void OnDisconnected(ConnectionInfo info)
        {
            this.Close();
            base.OnDisconnected(info);
        }

        public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            log.Debug($"On Message, size: {size}, messageNum: {messageNum}");

            // TODO this should have a reasonable size
            if (size > 10 * 1024 * 1024) // 10kb
            {
                log.Warn("Discarding large packet");
                return;
            }

            var buffer = this.bufferPool.Rent(size);
            Marshal.Copy(data, buffer, 0, size);

            var gamePacket = GamePacket.Parser.ParseFrom(buffer, 0, size);

            if (gamePacket.PacketCase == GamePacket.PacketOneofCase.None) return;
            if (RegisterManager.ClientPacketHandlersCache.TryGetValue(gamePacket.PacketCase, out var handler))
            {
                handler.HandlePacket(null, gamePacket);
            }
        }

        public void Dispose()
        {
            try
            {
                if (this.Connected || Connecting)
                {
                    this.Connection.Close(true);
                    Close();
                }
            }
            catch (Exception ex)
            {
                this.log.Error(ex);
            }
        }
    }
}