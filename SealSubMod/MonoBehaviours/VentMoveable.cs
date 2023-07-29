namespace SealSubMod.MonoBehaviours;

internal class VentMoveable : HandTarget, IHandTarget
{
    public GameObject ventModel;

    private float _state;
    private bool _target;
    private float _timeOpen;

    public static float speed = 1;//the speed it moves at
    public static float max = 2;//the max movement duration
    public static float horizontalCutoff = 0.5f;//the amount of time it goes vertically before swapping to horizontal
    public static float openDuration = 2;//extra time where the vent stays open without moving

    public static Vector3 targetPos = (new Vector3(0, 0.05f, 0.75f));

    public Vector3 closedPos;

    public override void Awake()
    {
        base.Awake();//doesn't actually do anything but whatever
        closedPos = ventModel.transform.localPosition;
    }

    public void OnHandClick(GUIHand hand)
    {
        if (_target || _state > 0) return;
        _target = true;
        //Play sound here too
    }

    private void Update()
    {
        if (!_target && _state <= 0) return;//fully closed

        if (_state >= max)
        {
            _timeOpen += Time.deltaTime;
            if (_timeOpen < openDuration)
                return;

            _timeOpen = 0;
            _target = !_target;
        }

        _state += Time.deltaTime * speed * (_target ? 1 : -1);//decrease state if closing, increase state if opening


        var currentTargetPos = closedPos;

        currentTargetPos += new Vector3(0, Mathf.Lerp(0, targetPos.y, Mathf.Clamp01(_state / horizontalCutoff)), 0);

        currentTargetPos += new Vector3(0, 0, Mathf.Lerp(0, targetPos.z, Mathf.Clamp01((_state - horizontalCutoff) / (max - horizontalCutoff))));

        ventModel.transform.localPosition = currentTargetPos;
    }

    public void OnHandHover(GUIHand hand)
    {
        //ErrorMessage.AddMessage("Hand hover");
    }
}
