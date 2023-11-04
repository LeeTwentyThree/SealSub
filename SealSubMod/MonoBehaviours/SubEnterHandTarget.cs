namespace SealSubMod.MonoBehaviours;

public class SubEnterHandTarget : HandTarget, IHandTarget
{
    [SerializeField] SubRoot sub;
    [SerializeField] Transform targetPosition;
    [SerializeField] FMODAsset sound;

    public void OnHandClick(GUIHand hand)
    {
        Utils.PlayFMODAsset(sound, targetPosition.position);
        Player.main.SetPosition(targetPosition.position, targetPosition.rotation);
        Player.main.SetCurrentSub(sub, true);
    }

    public void OnHandHover(GUIHand hand)
    {
        var text = sub ? "Board Seal" : "Disembark Seal";
        HandReticle.main.SetText(HandReticle.TextType.Hand, text, true, GameInput.Button.LeftHand);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
    }
}
