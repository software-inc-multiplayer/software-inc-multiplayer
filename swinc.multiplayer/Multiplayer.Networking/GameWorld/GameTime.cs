using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Networking
{
	[Serializable]
	public class GameTime
	{
		SDateTime gametime;
		int speed;

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
