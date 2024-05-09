using SealSubMod.MonoBehaviours;
using SealSubMod.MonoBehaviours.VisualFX;

namespace SealSubMod.Utility;

internal class Gaytilities
{
    private static bool _gayModeActive = false;
    public static bool GayModeActive
    {
        get { return _gayModeActive; }
        set
        {
            if (value && !_gayModeActive) UpdateIsGay();
            _gayModeActive = value;
        }
    }
    public static void EngaygeGayification(GameObject objectToGayify)
    {
        objectToGayify.AddComponent<GayifierBehaviour>();
    }
    public static void UpdateIsGay()
    {
        foreach (var gaySeal in GameObject.FindObjectsOfType<SealSubRoot>()) EngaygeGayification(gaySeal.gameObject);
    }
}
