namespace SealSubMod.MonoBehaviours.UI;

internal class SealSubEasterEggButton : SealButtonBase
{
    [SerializeField] VoiceNotification easterEggVoiceNotification;

    protected override void OnMouseClickBehavior()
    {
        subRoot.voiceNotificationManager.PlayVoiceNotification(easterEggVoiceNotification);
    }
}