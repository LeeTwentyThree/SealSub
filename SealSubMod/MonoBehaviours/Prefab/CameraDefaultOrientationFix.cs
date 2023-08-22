namespace SealSubMod.MonoBehaviours.Prefab;

internal class CameraDefaultOrientationFix : MonoBehaviour
{
    public Transform parent;

    public Vector3 angle;

    private void OnValidate()
    {
        if (parent)
        {
            transform.forward = parent.forward;
            transform.eulerAngles += angle;
        }
    }
}