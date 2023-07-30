using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System;

namespace SealSubMod.Patches.BasePatches;

[HarmonyPatch(typeof(BaseGhost))]
internal class BaseGhostPatches
{
    [HarmonyPatch(nameof(BaseGhost.Finish))]
    public static bool Prefix(BaseGhost __instance)
    {
        if (Player.main.currentSub is not SealSubRoot seal) return true;

        if(__instance is not BaseAddWaterPark waterPark) return true;



        var cam = MainCamera.camera;
        var marker = BasePieceLocationMarker.GetNearest(cam.transform.position, cam.transform.forward, true, seal.GetComponentsInChildren<BasePieceLocationMarker>(true));
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



        return false;
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
