using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System;
using UnityEngine;

namespace SealSubMod.Patches.BasePatches;

[HarmonyPatch(typeof(BaseGhost))]
internal class BaseGhostPatches
{
    [HarmonyPatch(nameof(BaseGhost.Finish))]//let the water park *actually* place
    public static bool Prefix(BaseGhost __instance)
    {
        if (Player.main.currentSub is not SealSubRoot seal) return true;

        if(__instance is not BaseAddWaterPark waterPark) return true;



        var cam = MainCamera.camera;
        //var marker = BasePieceLocationMarker.GetNearest(cam.transform.position, cam.transform.forward, true, seal.GetComponentsInChildren<BasePieceLocationMarker>(true));
        var marker = __instance.GetComponentInParent<BasePieceLocationMarker>();
        if (!marker) throw new InvalidOperationException("Shis fucked.");

        var piece = Base.Piece.RoomWaterParkBottom;


        var model = __instance.GetComponentInParent<ConstructableBase>().model;
        var position = model.transform.position;


        var prefab = Base.pieces[(int)piece].prefab;//for some fucking reason the geometry and the module are different things

        var mainObj = GameObject.Instantiate(prefab, position, marker.transform.rotation, marker.transform);
        mainObj.transform.localPosition += new Vector3(5, 0, 5);
        mainObj.gameObject.SetActive(true);

        var waterParkPieces = new List<Base.Piece>() { Base.Piece.RoomWaterParkCeilingTop, Base.Piece.RoomWaterParkFloorBottom };
        foreach(var waterParkPiece in waterParkPieces)
        {
            var piecePrefab = Base.pieces[(int)waterParkPiece].prefab;
            var pieceObj = GameObject.Instantiate(piecePrefab, mainObj.transform.position, marker.transform.rotation, mainObj.transform);
            pieceObj.gameObject.SetActive(true);
        }


        var module = GameObject.Instantiate(WaterPark.roomWaterParkPrefab, position, marker.transform.rotation, mainObj.transform);
        module.transform.localPosition += new Vector3(5, 0, 5);


        var constr = mainObj.gameObject.EnsureComponent<Constructable>();
        constr.techType = TechType.BaseWaterPark;
        constr.ExcludeFromSubParentRigidbody();//magic method all I know is it does things that I assume are helpful in some way


        marker.PieceObject = mainObj.gameObject;
        marker.AttachedBasePiece = piece;

        ModifyWaterPark(mainObj.gameObject, module.GetComponent<WaterPark>());

        return false;
    }

    public static void ModifyWaterPark(GameObject waterParkModelRoot, WaterPark waterPark)
    {
        waterPark.height = 1;
        waterPark.planter.gameObject.AddComponent<PlantHeightSetter>();

        ModifyWaterParkWalls(waterParkModelRoot, waterPark);
    }
    public class PlantHeightSetter : MonoBehaviour
    {
        public Planter planter;
        public void Awake() => planter = GetComponent<Planter>();
        public IEnumerator Start()
        {
            yield return new WaitUntil(() => planter.initialized);
            planter.SetMaxPlantsHeight(1.60f);
            //apparently 1.75 is the height of one basic one tall ACU
            //we're a bit shorter than that though
            //5.25 is the height of two combined ACUs
        }
    }
    public static void ModifyWaterParkWalls(GameObject waterParkModelRoot, WaterPark waterPark)
    {
        var model = waterParkModelRoot.transform.Find("model");
        model.GetComponentInChildren<Renderer>().material = MaterialUtils.AirWaterBarrierMaterial;//set material


        var collisionObjects = new List<GameObject>();
        var collision = waterParkModelRoot.transform.Find("collisions");
        foreach (var collider in collision.GetComponentsInChildren<Collider>())//set default collisions to triggers
        {
            collider.isTrigger = true;
            if (!collisionObjects.Contains(collider.gameObject))//add them to a list so we don't have to add the component for each collider (there's multiple on each object)
                collisionObjects.Add(collider.gameObject);
        }

        foreach (var colliderObj in collisionObjects)//add interior player triggers
        {
            var enter = colliderObj.gameObject.EnsureComponent<WaterParkEnterTrigger>();
            enter.waterPark = waterPark;
            enter.setInside = true;
        }

        var exteriorTriggerCollision = GameObject.Instantiate(collision, collision.transform.position, collision.transform.rotation, collision.transform);//make exterior triggers
        exteriorTriggerCollision.localScale = new Vector3(1.15f, 1, 1.15f);

        foreach (var enter in exteriorTriggerCollision.GetComponentsInChildren<WaterParkEnterTrigger>())//add exterior player triggers
        {
            enter.setInside = false;//make them walk triggers, rather than swim triggers
        }


        foreach (var colliderObj in collisionObjects)//go through interior collision again and add "walls"
        {
            var forceTrigger = colliderObj.AddComponent<WaterParkCreatureWall>();
            forceTrigger.center = waterPark.transform;
        }
    }

    [HarmonyPatch(nameof(BaseGhost.Place))]//set parent properly, so that the modules move with the sub
    public static void Postfix(BaseGhost __instance)
    {
        if (Player.main.currentSub is not SealSubRoot root) return;


        var comp = __instance.GetComponentInParent<ConstructableBase>();//The root ghost object that needs to be parented

        if (!comp) return;//doesn't exist on the non-ghost objects (and also a basic safety check)


        var cam = MainCamera.camera;
        var marker = BasePieceLocationMarker.GetNearest(cam.transform.position, cam.transform.forward, true, root.GetComponentsInChildren<BasePieceLocationMarker>(true));
        if (!marker) throw new InvalidOperationException("Shis fucked.");

        comp.transform.parent = marker.transform;
    }
}
