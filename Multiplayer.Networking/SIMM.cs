#pragma warning disable IDE1006 // Naming Styles
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Networking
{
    public class SIMM
    {
        public class Constants
        {
            public class Stamps
            {

                public int created { get; set; }

                public int lastLogin { get; set; }
            }
            public class Account
            {
                public string username { get; set; }
                public string token { get; set; }
                public string email { get; set; }
                public string password { get; set; }
                public Stamps stamps { get; set; }
                public string steam { get; set; }
                public string discord { get; set; }
                public string avatar { get; set; }
            }
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles
