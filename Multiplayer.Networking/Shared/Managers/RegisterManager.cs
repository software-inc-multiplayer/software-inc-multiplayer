using System;
using System.Collections.Generic;
using System.Reflection;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Server;
using Multiplayer.Packets;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Shared.Managers
{
    public enum RegisterType
    {
        Client,
        Server
    }

    public class RegisterManager : Attribute
    {
        public static readonly Dictionary<GamePacket.PacketOneofCase, List<IPacketHandler>> ClientPacketHandlersCache = new Dictionary<GamePacket.PacketOneofCase, List<IPacketHandler>>();
        public static readonly Dictionary<GamePacket.PacketOneofCase, List<IPacketHandler>> ServerPacketHandlersCache = new Dictionary<GamePacket.PacketOneofCase, List<IPacketHandler>>();
        
        private RegisterType type;
        private GamePacket.PacketOneofCase catcher;
        
        public virtual RegisterType Type { get => this.type; set => type = value; }
        public virtual GamePacket.PacketOneofCase Catcher { get => this.catcher; set => catcher = value; }

        public RegisterManager(RegisterType type, GamePacket.PacketOneofCase catcher)
        {
            this.type = type;
            this.catcher = catcher;
        }
        
        public static void LoadInstances(ILogger logger, GameClient? client, GameServer? server)
        {
            logger.Info("Loading Handlers @ " + (client != null ? "Client" : "Server"));
            foreach(var t in Assembly.GetCallingAssembly().GetTypes())
            {
                var f = t.GetCustomAttributes(typeof(RegisterManager), true);
                
                if (f.Length <= 0) 
                    continue;
                
                logger.Info("Found class with RegisterManager: " + t.Name);
                
                var manager = (RegisterManager)f[0];
                
                logger.Info("RegisterManager at " + t.Name + " for " + manager.type + " catches " + manager.Catcher);
                
                var ctor = t.GetConstructor(new[] { manager.type == RegisterType.Client ? typeof(GameClient) : typeof(GameServer) });
                if (ctor == null)
                        break;

                switch (manager.type)
                {
                    case RegisterType.Client when client != null:
                    {
                        if (ClientPacketHandlersCache.TryGetValue(manager.Catcher, out var arr))
                        {
                            arr.Add((IPacketHandler) ctor.Invoke(new object[] { client }));
                        }
                        else
                        {
                            ClientPacketHandlersCache.Add(manager.Catcher, new List<IPacketHandler> { (IPacketHandler)ctor.Invoke(new object[] { client }) });
                        }
                        break;
                    }
                    case RegisterType.Server when server != null:
                    {
                        if (ServerPacketHandlersCache.TryGetValue(manager.Catcher, out var arr))
                        {
                            arr.Add((IPacketHandler) ctor.Invoke(new object[] { server }));
                        }
                        else
                        {
                            ServerPacketHandlersCache.Add(manager.Catcher, new List<IPacketHandler> { (IPacketHandler)ctor.Invoke(new object[] { server }) });
                        }
                        break;
                    }
                    default:
                        break;
                }

                logger.Info("Registered Catcher - " + manager.Catcher );
            }
        }
    }
}