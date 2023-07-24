using SealSubMod.Interfaces;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SealSkyApplierManager : MonoBehaviour, IOnAsyncPrefabTasksCompleted
{
    public SkyApplier exteriorSkyApplier;
    public SkyApplier interiorSkyApplier;
    public SkyApplier windowSkyApplier;

    private List<Renderer> _interiorRenderers = new List<Renderer>();
    private List<Renderer> _exteriorRenderers = new List<Renderer>();
    private List<Renderer> _windowRenderers = new List<Renderer>();

    public void OnAsyncPrefabTasksCompleted()
    {
        var allRenderers = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (var r in allRenderers)
        {
            List<Renderer> listToUse;
            if (r.gameObject.GetComponent<SubWindowTag>() != null)
            {
                listToUse = _windowRenderers;
            }
            else if (r.gameObject.GetComponent<SubExteriorObjectTag>() != null)
            {
                listToUse = _exteriorRenderers;
            }
            else
            {
                listToUse = _interiorRenderers;
            }
            listToUse.Add(r);
        }

        exteriorSkyApplier.renderers = _exteriorRenderers.ToArray();
        interiorSkyApplier.renderers = _interiorRenderers.ToArray();
        windowSkyApplier.renderers = _windowRenderers.ToArray();
    }
}
