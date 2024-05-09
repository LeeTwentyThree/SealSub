namespace SealSubMod.MonoBehaviours.VisualFX.Propellors;

public class PropellerManager : MonoBehaviour
{
    [SerializeField] SubControl control;
    [SerializeField] float minVelocity;
    [SerializeField] float maxVelocity;
    [SerializeField] float deceleration;
    [SerializeField] float engineAccel;

    private float angularVel;

    public float RotationsPerSecond => angularVel;

    private void Update()
    {
        if (control.appliedThrottle)
        {
            angularVel += engineAccel * Time.deltaTime;
        }
        angularVel = Mathf.Clamp(angularVel - deceleration * Time.deltaTime, minVelocity, maxVelocity);
    }
}
