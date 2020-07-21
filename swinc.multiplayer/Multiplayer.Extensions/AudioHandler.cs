using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Extensions
{
    public static class AudioHandler
    {
        public static void PlaySound(PopupManager.NotificationSound sfx)
        {
            UISoundFX.PlaySFX("Notification" + sfx);
        }
        public static void Play(this PopupManager.NotificationSound sfx)
        {
            UISoundFX.PlaySFX("Notification" + sfx);
        }
    }
}
