using mset;
using SealSubMod.Interfaces;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SealSkyApplierManager : MonoBehaviour, IAsyncPrefabSetupOperation, IOnAsyncPrefabTasksCompleted
{
    public SkyApplier exteriorSkyApplier;
    public SkyApplier interiorSkyApplier;
    public SkyApplier windowSkyApplier;
    public float[] lightBrightnessMultipliers = new float[] {1, 0.5f, 0f};

    public LightingController lightingController;

    [HideInInspector] public Sky skyBaseGlass;
    [HideInInspector] public Sky skyBaseInterior;

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

    public IEnumerator SetupPrefabAsync(GameObject prefabRoot)
    {
        yield return CyclopsReferenceManager.EnsureCyclopsReferenceExists();
        var cyclops = CyclopsReferenceManager.CyclopsReference;
        skyBaseGlass = Instantiate(cyclops.transform.Find("SkyBaseGlass"), transform).GetComponent<Sky>();
        skyBaseInterior = Instantiate(cyclops.transform.Find("SkyBaseInterior"), transform).GetComponent<Sky>();
        lightingController.skies[0].sky = skyBaseGlass;
        lightingController.skies[1].sky = skyBaseInterior;

        var lights = gameObject.GetComponentsInChildren<Light>(true)
            .Where((l) => l.gameObject.GetComponent<ExcludeLightFromController>() == null)
            .ToArray();

        lightingController.lights = new MultiStatesLight[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            var intensity = lights[i].intensity;
            lightingController.lights[i] = new MultiStatesLight()
            {
                light = lights[i],
                intensities = new float[]
                {
                    intensity * lightBrightnessMultipliers[0], intensity * lightBrightnessMultipliers[1], intensity * lightBrightnessMultipliers[2]
                },
            };
        }

        var sub = GetComponent<SealSubRoot>();
        sub.interiorSky = skyBaseInterior;
        sub.glassSky = skyBaseGlass;
    }
}