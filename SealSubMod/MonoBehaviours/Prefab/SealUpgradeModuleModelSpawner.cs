using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SealUpgradeModuleModelSpawner : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] Transform[] moduleSlots;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var upgradeModuleModel = cyclops.transform
            .Find("CyclopsMeshStatic/undamaged/cyclops_LOD0/cyclops_engine_room/cyclops_engine_console/" +
            "Submarine_engine_GEO/submarine_engine_console_01_wide/engine_console_key_01_01")
            .gameObject;

        foreach (var slot in moduleSlots)
        {
            var clone = Instantiate(upgradeModuleModel, slot, false);
            clone.transform.localPosition = new Vector3(-0.41f, -0.69f, -2.69f);
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one * 2.7f;
            clone.gameObject.SetActive(true);
        }
    }
}