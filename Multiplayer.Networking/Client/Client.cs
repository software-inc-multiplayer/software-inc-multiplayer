using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;
namespace Multiplayer.Networking
{
    public class ClientClass
    {
        public static User MyUser { get; set; }
        public Client RawClient { get; set; }
        public void Connect(string ip, int port)
        {
            MyUser = new User();
        }
    }
}
