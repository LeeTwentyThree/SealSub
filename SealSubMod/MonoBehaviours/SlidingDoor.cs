namespace SealSubMod.MonoBehaviours;

internal class SlidingDoor : MonoBehaviour
{
    [SerializeField] Vector3 openPos;
    private Vector3 defaultLocalPos;
    private float speed = 1;
    private float state = 0;
    private float target = 0;
    public void SetOpen(bool open)
    {
        target = open ? 1 : 0;
    }
    public void Toggle()
    {
        target = Mathf.Approximately(target, 0) ? 1 : 0;
    }

    private void Awake()
    {
        defaultLocalPos = transform.localPosition;
    }

    private void Update()
    {
        var direction = Mathf.Sign(target - state);
        if (direction == 0) return;

        state += speed * direction * Time.deltaTime;
        state = Mathf.Clamp01(state);

        transform.localPosition = defaultLocalPos + Vector3.Lerp(Vector3.zero, openPos, state);
    }
}
