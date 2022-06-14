using System;
using System.Collections.Generic;
using System.Reflection;

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
        
        public static List<Type> GetTypesWithAttribute(RegisterType typee)
        {
            List<Type> e = new List<Type>();
            foreach(Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                var t = type.GetCustomAttributes(typeof(RegisterManager), true);
                if (t.Length > 0 && ((RegisterManager) t[0]).type == typee)
                {
                    e.Add(type);
                }
            }
            return e;
        }
    }
}