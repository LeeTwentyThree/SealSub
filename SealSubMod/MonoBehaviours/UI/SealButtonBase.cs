namespace SealSubMod.MonoBehaviours.UI;

public class SealButtonBase : MonoBehaviour
{
    [HideInInspector] public bool invalidButton;

    public SubRoot subRoot;

    private bool mouseHover;

    private void OnValidate()
    {
        if (subRoot == null) subRoot = gameObject.GetComponentInParent<SubRoot>();
    }

    public void OnMouseEnter()
    {
        if (Player.main.currentSub != subRoot)
        {
            return;
        }
        mouseHover = true;
        OnMouseEnterBehavior();
    }

    public void OnMouseExit()
    {
        if (Player.main.currentSub != subRoot)
        {
            return;
        }
        mouseHover = false;
        OnMouseExitBehavior();
    }

    public void OnClick()
    {
        if (invalidButton)
        {
            return;
        }
        if (Player.main.currentSub != subRoot)
        {
            return;
        }
        mouseHover = false;
        OnMouseClickBehavior();
    }

    protected virtual void OnMouseClickBehavior() { }

    protected virtual void OnMouseEnterBehavior() { }

    protected virtual void OnMouseExitBehavior() { }
}
