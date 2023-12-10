using SealSubMod.MonoBehaviours.Abstract;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class MapRoomFunctionalitySpawner : MonoBehaviour
{
    [SerializeField] Transform fabricatorPosition;

    private IEnumerator Start()
    {
        yield break;//Temporary fix until we can work with the model again

        if (!Base.mapRoomFunctionalityPrefab)
        {
            Base.Initialize();
            yield return new WaitUntil(() => Base.mapRoomFunctionalityPrefab);
        }
        var func = Instantiate(Base.mapRoomFunctionalityPrefab, transform.position, transform.rotation, transform);

        foreach (Transform child in func.transform)
        {
            if (child.name.Contains("dockingPoint"))
                Destroy(child.gameObject);
        }
        func.GetComponent<MapRoomFunctionality>().miniWorld.updatePosition = true;

        Destroy(func.transform.Find("hologram/worlddisplay/MapRoomFX/x_MapRoom_HoloTableGlow_Top"));

        if(fabricatorPosition)
        {
            var fabSpawn = func.GetComponentInChildren<PrefabSpawn>(true);
            if (!fabSpawn) yield break;
            var upgrades = func.GetComponentInChildren<StorageContainer>(true);
            var upgradesOffset = upgrades.transform.position - fabSpawn.transform.position;
            fabSpawn.transform.parent = fabricatorPosition;
            fabSpawn.transform.position = fabricatorPosition.position;
            fabSpawn.transform.rotation = fabricatorPosition.rotation;

            upgrades.transform.parent = fabricatorPosition;
            upgrades.transform.position = fabricatorPosition.position + upgradesOffset;
            upgrades.transform.parent.rotation = fabricatorPosition.rotation;
        }
    }
}
