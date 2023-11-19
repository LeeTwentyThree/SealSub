using HarmonyLib;
using SealSubMod.MonoBehaviours;
using System;

namespace SealSubMod.Patches.BasePatches;

[HarmonyPatch(typeof(BaseAddModuleGhost))]
internal class BaseModuleGhostPatches
{
    public static Int3 cell = new Int3(1, 0, 1);//todo
    public static Int3 size = new Int3(2, 1, 2);//fuck with these values and try find why they're so restricted


    [HarmonyPatch(nameof(BaseAddModuleGhost.UpdatePlacement))]//Let the base piece modules snap into place
    public static bool Prefix(BaseAddModuleGhost __instance, Transform camera, float placeMaxDistance, ref bool positionFound, ref bool geometryChanged, ConstructableBase ghostModelParentConstructableBase, ref bool __result)
    {
        if (Player.main.currentSub is not SealSubRoot seal) return true;

        var examples = seal.GetComponentsInChildren<BasePieceLocationMarker>(true);

        var transCam = camera;
        var marker = BasePieceLocationMarker.GetNearest(transCam.position, transCam.forward, true, examples);

        if(!marker) return true;

        if (Vector3.Distance(transCam.position, marker.transform.position) > placeMaxDistance) return true;

        Builder.UpdateRotation(Base.HorizontalDirections.Length);

        Base.Direction direction = Base.HorizontalDirections[Builder.lastRotation % Base.HorizontalDirections.Length];
        var face = new Base.Face(cell, direction);
        if (__instance.anchoredFace == null || __instance.anchoredFace.Value != face)
        {
            __instance.anchoredFace = face;



            __instance.UpdateSize(size);

            __instance.ghostBase.SetFaceType(face, __instance.faceType);
            __instance.ghostBase.ClearMasks();
            __instance.ghostBase.SetFaceMask(face, true);
            __instance.RebuildGhostGeometry(true);

            for(int i = 0; i < __instance.ghostBase.cells.Length; i++)
                __instance.ghostBase.cells[i] = Base.CellType.OccupiedByOtherCell;
            __instance.ghostBase.cells[0] = Base.CellType.Room;
        }


        ghostModelParentConstructableBase.tr.position = marker.transform.position;
        ghostModelParentConstructableBase.tr.localPosition += -new Vector3(5, 0, 5);//offset to account for the base offset that's applied for some reason
        ghostModelParentConstructableBase.tr.rotation = marker.transform.rotation;
        ghostModelParentConstructableBase.tr.parent = marker.transform;
        positionFound = true;
        geometryChanged = true;
        __result = true;

        return false;
    }

    [HarmonyPatch(nameof(BaseAddModuleGhost.Finish))]//Let the base piece modules actually be placed
    public static bool Prefix(BaseAddModuleGhost __instance)
    {
        if (Player.main.currentSub is not SealSubRoot seal) return true;


        var marker = __instance.GetComponentInParent<BasePieceLocationMarker>(true);
        if (!marker) throw new InvalidOperationException("Shis fucked.");


        if (!faceToPiece.TryGetValue(__instance.faceType, out var piece))
        {
            ErrorMessage.AddMessage($"Sorry! Piece type {__instance.faceType} is not supported in this vehicle!!!");
            return false;
        }
        var model = __instance.GetComponentInParent<ConstructableBase>(true).model;
        var position = model.transform.position;
        var rotation = Quaternion.Euler(Base.DirectionNormals[(int)__instance.anchoredFace.Value.direction]);


        var prefab = Base.pieces[(int)piece].prefab;//for some fucking reason the geometry and the module are different things

        var geomObj = GameObject.Instantiate(prefab, position, rotation, marker.transform);
        geomObj.localPosition += new Vector3(5, 0, 5);//for some unholy fucking reason. These are, again, offset weirdly :)
        geomObj.localRotation = Quaternion.Euler(rotations[__instance.anchoredFace.Value.direction]);
        geomObj.gameObject.SetActive(true);

        var module = GameObject.Instantiate(__instance.modulePrefab, position, rotation, geomObj.transform);
        module.transform.localPosition += new Vector3(5, 0, 5);

        var constr = geomObj.gameObject.EnsureComponent<Constructable>();
        constr.techType = Base.FaceToRecipe[(int)__instance.faceType];
        constr.ExcludeFromSubParentRigidbody();//magic method all I know is it does things that I assume are helpful in some way

        marker.PieceObject = geomObj.gameObject;
        marker.AttachedBasePiece = piece;

        

        return false;
    }

    public static Dictionary<Base.Direction, Vector3> rotations = new()
    {
        {Base.Direction.North, new Vector3(0, 270, 0) },
        {Base.Direction.South, new Vector3(0, 90, 0) },
        {Base.Direction.West, new Vector3(0, 180, 0) },
        {Base.Direction.East, new Vector3(0, 0, 0) },
    };

    public static Dictionary<Base.FaceType, Base.Piece> faceToPiece = new Dictionary<Base.FaceType, Base.Piece>()
    {
        { Base.FaceType.BioReactor, Base.Piece.RoomBioReactor },
        { Base.FaceType.NuclearReactor, Base.Piece.RoomNuclearReactor },
        //{ Base.FaceType.WaterPark, Base.Piece. },//water park kinda sucks ngl
        //{ Base.FaceType.Partition, Base.Piece.partition },//yea fuck this
        //{ Base.FaceType.BioReactor, Base.Piece.RoomBioReactor },//there's one enum entry for every single god damn position AND rotation
                                                                    //Were these people on crack what the fuck is this
    };
}

