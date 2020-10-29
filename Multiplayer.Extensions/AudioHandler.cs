namespace Multiplayer.Extensions
{
    public static class AudioHandler
    {
        public static void PlaySound(PopupManager.NotificationSound sfx)
        {
            UISoundFX.PlaySFX("Notification" + sfx.ToString());
        }
        public static void Play(this PopupManager.NotificationSound sfx)
        {
            UISoundFX.PlaySFX("Notification" + sfx.ToString());
        }
    }
}
