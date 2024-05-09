using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.MonoBehaviours.VisualFX;

namespace SealSubMod.MonoBehaviours.Prefab;

public class MapRoomFunctionalitySpawner : MonoBehaviour
{
    [SerializeField] Transform fabricatorPosition;
    [SerializeField] Transform mapPosition;
    [SerializeField] MapRoomMapMover mapMover;

    private IEnumerator Start()
    {
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
        var mini = func.GetComponent<MapRoomFunctionality>();
        mini.hologramRadius = 1.75f;
        mini.miniWorld.updatePosition = true;

        mapMover.miniWorld = mini.miniWorld.gameObject.AddComponent<MiniWorldPosition>();
        mini.miniWorld.transform.position = mapPosition.position;
        mini.miniWorld.transform.parent = mapPosition;


        if(fabricatorPosition)
        {
            var fabSpawn = func.GetComponentInChildren<PrefabSpawn>(true);
            if (!fabSpawn) yield break;
            var upgrades = func.GetComponentInChildren<StorageContainer>(true);

            fabSpawn.transform.parent = fabricatorPosition;
            fabSpawn.transform.position = fabricatorPosition.position;
            fabSpawn.transform.rotation = fabricatorPosition.rotation;

            upgrades.transform.parent = fabricatorPosition;
            upgrades.transform.localPosition = new Vector3(0.5f, 0, -3.3f);//For some reason *some* things have offsets while others don't. Yes I hate this.
            upgrades.transform.rotation = fabricatorPosition.rotation;

            var slots = func.transform.Find("UpgradeSlots");
            slots.transform.position = fabricatorPosition.position;
            slots.transform.rotation = fabricatorPosition.rotation;
        }
    }

    public class MiniWorldPosition : MonoBehaviour
    {
        public Vector3 Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = Vector3.ClampMagnitude(value, maxOffset);
            }
        }
        internal float maxOffset;
        private Vector3 _offset;
        private Transform mapRoot;
        private void Start() 
        {
            mapRoot = transform.GetChild(0).GetChild(0);


            Destroy(transform.Find("worlddisplay/MapRoomFX/x_MapRoom_HoloTableGlow_Top").gameObject);
        }
        private void Update()
        {
            for(int  i = 0; i < mapRoot.childCount; i++)
            {
                var child = mapRoot.GetChild(i);
                child.transform.localPosition = _offset;
            }
        }
    }
}
