using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Prefab.Tags;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class FloodlightCreator : MonoBehaviour, ICyclopsReferencer
{
    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var cyclopsLightParent = cyclops.transform.Find("Floodlights");
        if (!cyclopsLightParent) { Plugin.Logger.LogError("Cyclops floodlights null! Can't create seal floodlights"); return; }

        foreach (var lightChild in GetComponentsInChildren<FloodlightMarker>(true))
        {
            var light = cyclopsLightParent.Find(lightChild.lightPrefabObjectName);
            if(!light) Plugin.Logger.LogError($"light child {lightChild} has invalid prefab name. Can't find {lightChild.lightPrefabObjectName} child in cyclops floodlights");

            Instantiate(light, lightChild.transform.position, lightChild.transform.rotation, lightChild.transform).gameObject.SetActive(true);
        }
    }
}
