using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;

namespace SealSubMod.MonoBehaviours.Prefab;

public class LoadDistanceFieldIntoWaterClipProxy : PrefabModifier, ICyclopsReferencer
{
    public WaterClipProxy waterClipProxy;
    public DistanceField distanceField;
    
    // The water clip proxy component should remain disabled in the editor so that we can override its initialization without patching a coroutine
    private void OnValidate()
    {
        waterClipProxy.enabled = false;
    }

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        waterClipProxy.clipMaterial =
            cyclops.transform.Find("WaterClipProxy").GetComponent<WaterClipProxy>().clipMaterial;
    }

    private void Start()
    {
        // Trick the original component into thinking it has been initialized naturally, to avoid duplicate initialization
        waterClipProxy.initialized = true;
        // This is necessary because we're delaying the Start method
        if (waterClipProxy.waterSurface == null) waterClipProxy.waterSurface = WaterSurface.Get();
        // This is normally called in Update when initialized = false
        ApplyDistanceField();
        // Now get the distance field ready
        waterClipProxy.enabled = true;
        gameObject.layer = 28;
    }
    
    // This method is based on WaterClipProxy.LoadAsync and was modified to suit our needs.
    // I really don't want to transpile a coroutine. Those can have major compatibility issues.
    private void ApplyDistanceField()
    {
        waterClipProxy.UnloadDistanceField();
        var borderSizeScaled = default(Vector3);
        borderSizeScaled.x = waterClipProxy.waterSurface.foamDistance / base.transform.lossyScale.x;
        borderSizeScaled.y = waterClipProxy.waterSurface.foamDistance / base.transform.lossyScale.y;
        borderSizeScaled.z = waterClipProxy.waterSurface.foamDistance / base.transform.lossyScale.z;
        if (distanceField != null)
        {
            waterClipProxy.distanceFieldMin = distanceField.min;
            waterClipProxy.distanceFieldMax = distanceField.max;
            waterClipProxy.distanceFieldSize = waterClipProxy.distanceFieldMax - waterClipProxy.distanceFieldMin;
            waterClipProxy.distanceFieldTexture = distanceField.texture;
            var extents = waterClipProxy.distanceFieldSize * 0.5f + borderSizeScaled;
            var vector = (waterClipProxy.distanceFieldMin + waterClipProxy.distanceFieldMax) * 0.5f;
            // Unnecessary optimization for a submarine, but I'm keeping it
            if (waterClipProxy.immovable && !waterClipProxy.GetIntersectsWaterSurface(vector, distanceField.meshBoundsSize * 0.5f))
            {
                gameObject.SetActive(false);
                return;
            }
            waterClipProxy.CreateBoxMesh(vector, extents);
        }
        else
        {
            Plugin.Logger.LogInfo($"No distance field found on {this}! Using a box instead.");
            var extents = Vector3.one * 0.5f + borderSizeScaled;
            var center = Vector3.zero;
            // Once again, unnecessary optimization for an edge case
            if (waterClipProxy.immovable && !waterClipProxy.GetIntersectsWaterSurface(center, extents))
            {
                gameObject.SetActive(value: false);
                return;
            }
            waterClipProxy.CreateBoxMesh(center, extents);
        }
        var renderer = gameObject.EnsureComponent<MeshRenderer>();
        renderer.material = waterClipProxy.clipMaterial;
        waterClipProxy.clipMaterial = renderer.material;
        waterClipProxy.UpdateMaterial();
    }
}