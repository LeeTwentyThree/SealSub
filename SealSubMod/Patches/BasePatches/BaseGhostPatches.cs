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
        var position = model.transform.position + new Vector3(5, 0, 5);//for some unholy fucking reason. These are, again, offset weirdly :)


        var prefab = Base.pieces[(int)piece].prefab;//for some fucking reason the geometry and the module are different things

        var mainObj = GameObject.Instantiate(prefab, position, Quaternion.identity, marker.transform);
        mainObj.gameObject.SetActive(true);

        var waterParkPieces = new List<Base.Piece>() { Base.Piece.RoomWaterParkCeilingTop, Base.Piece.RoomWaterParkFloorBottom };
        foreach(var waterParkPiece in waterParkPieces)
        {
            var piecePrefab = Base.pieces[(int)waterParkPiece].prefab;
            var pieceObj = GameObject.Instantiate(piecePrefab, position, Quaternion.identity, marker.transform);
            pieceObj.gameObject.SetActive(true);
        }


        var module = GameObject.Instantiate(WaterPark.roomWaterParkPrefab, position, Quaternion.identity, mainObj.transform);


        var constr = mainObj.gameObject.EnsureComponent<Constructable>();
        constr.techType = TechType.BaseWaterPark;
        constr.ExcludeFromSubParentRigidbody();//magic method all I know is it does things that I assume are helpful in some way


        marker.PieceObject = mainObj.gameObject;
        marker.AttachedBasePiece = piece;



        return false;
    }
}
