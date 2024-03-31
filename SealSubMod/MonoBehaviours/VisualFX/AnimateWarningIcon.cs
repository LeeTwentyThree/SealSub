namespace SealSubMod.MonoBehaviours.VisualFX;

internal class AnimateWarningIcon : MonoBehaviour
{
    private Vector3 defaultScale;

    [SerializeField] float minimumScale;
    [SerializeField] float maximumScale;
    [SerializeField] float speed;

    private void Awake()
    {
        defaultScale = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = defaultScale * (minimumScale + Mathf.PingPong(Time.time * speed, maximumScale - minimumScale));
    }
}
