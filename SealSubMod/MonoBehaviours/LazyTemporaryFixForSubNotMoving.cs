namespace SealSubMod.MonoBehaviours;

internal class LazyTemporaryFixForSubNotMoving : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        GetComponent<SubControl>().cyclopsMotorMode.InvokeChangeEngineState(true);
    }
}
