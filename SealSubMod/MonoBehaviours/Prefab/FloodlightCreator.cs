using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.MonoBehaviours.Prefab.Tags;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class FloodlightCreator : PrefabModifierAsync
{
    public override IEnumerator SetupPrefabAsync()
    {
        if (!CyclopsReferenceManager.CyclopsReference) yield return CyclopsReferenceManager.EnsureCyclopsReferenceExists();

        var cyclopsLightParent = CyclopsReferenceManager.CyclopsReference.transform.Find("Floodlights");
        if (!cyclopsLightParent) { Plugin.Logger.LogError("Cyclops floodlights null! Can't create seal floodlights"); yield break; }

        foreach (var lightChild in GetComponentsInChildren<FloodlightMarker>(true))
        {
            var light = cyclopsLightParent.Find(lightChild.lightPrefabObjectName);
            if(!light) Plugin.Logger.LogError($"light child {lightChild} has invalid prefab name. Can't find {lightChild.lightPrefabObjectName} child in cyclops floodlights");

            Instantiate(light, lightChild.transform.position, lightChild.transform.rotation, lightChild.transform).gameObject.SetActive(true);
        }
    }
}
