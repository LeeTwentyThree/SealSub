﻿using SealSubMod.MonoBehaviours.Abstract;

namespace SealSubMod.MonoBehaviours.Prefab;

public class MapRoomFunctionalitySpawner : MonoBehaviour
{
    [SerializeField] Transform fabricatorPosition;
    [SerializeField] Transform mapPosition;

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

        mini.miniWorld.gameObject.AddComponent<MiniWorldPosition>();
        mini.miniWorld.transform.position = mapPosition.position;
        mini.miniWorld.transform.parent = mapPosition;


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

    public class MiniWorldPosition : MonoBehaviour
    {
        public bool invert = false;
        public float multTwo = -1;
        public Vector3 offset;
        private Transform map;
        private void Awake() => gameObject.GetComponentInParent<MapRoomMapMover>(true).miniWorld = this;
        private void Start() 
        { 
            map = transform.GetChild(0).GetChild(0).GetChild(1);


            Destroy(transform.Find("worlddisplay/MapRoomFX/x_MapRoom_HoloTableGlow_Top"));
        }
        private void Update() => map.transform.localPosition = invert ? -offset : offset;
    }
}
