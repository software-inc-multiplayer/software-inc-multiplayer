using System;
using System.Collections.Generic;
using System.Reflection;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Server;

namespace Multiplayer.Networking.Shared
{
    public enum RegisterType
    {
        CLIENT,
        SERVER
    }
    
    public class RegisterManager : Attribute
    {
        private RegisterType type;
        
        public virtual RegisterType Type { get => this.type; set => type = value; }

        public RegisterManager(RegisterType type)
        {
            this.type = type;
        }
        
        public static List<IPacketHandler> FetchInstancesWithAttribute(RegisterType type, object networkInstance)
        {
            var e = new List<IPacketHandler>();
            foreach(var t in Assembly.GetCallingAssembly().GetTypes())
            {
                var f = t.GetCustomAttributes(typeof(RegisterManager), true);
                if (f.Length <= 0 || ((RegisterManager)f[0]).type != type) 
                    continue;
                var ctor = t.GetConstructor(new[] { type == RegisterType.CLIENT ? typeof(GameClient) : typeof(GameServer) });
                if (ctor != null) e.Add((IPacketHandler) ctor.Invoke(new[] { networkInstance }));
            }
            return e;
        }
    }
}