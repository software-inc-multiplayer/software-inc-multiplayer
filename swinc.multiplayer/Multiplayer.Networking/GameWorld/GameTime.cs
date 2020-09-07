using System;
using System.Runtime.Serialization;

namespace Multiplayer.Networking
{
    [Serializable]
    public class GameTime
    {
        private SDateTime gametime;
        private int speed;

        [IgnoreDataMember]
        public SDateTime Get { get { return gametime; } }
        [IgnoreDataMember]
        public int Speed { get { return speed; } set { speed = value; } }

        public GameTime(SDateTime starttime, int startspeed)
        {
            gametime = starttime;
            speed = startspeed;
        }
    }
}
