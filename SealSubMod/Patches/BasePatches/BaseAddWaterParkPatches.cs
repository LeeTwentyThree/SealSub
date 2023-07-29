using HarmonyLib;
using SealSubMod.MonoBehaviours;
using static ClipMapManager;

namespace SealSubMod.Patches.BasePatches;

[HarmonyPatch(typeof(BaseAddWaterPark))]
internal class BaseAddWaterParkPatches
{
    public static Int3 cell = new Int3(1, 0, 1);//todo
    public static Int3 size = new Int3(2, 1, 2);//fuck with these values and try find why they're so restricted

    [HarmonyPatch(nameof(BaseAddWaterPark.UpdatePlacement))]
    public static bool Prefix(BaseAddWaterPark __instance, Transform camera, float placeMaxDistance, ref bool positionFound, ref bool geometryChanged, ConstructableBase ghostModelParentConstructableBase, ref bool __result)
    {
        if (Player.main.currentSub is not SealSubRoot seal) return true;

        var examples = seal.GetComponentsInChildren<BasePieceLocationMarker>(true);

        var transCam = camera;
        var marker = BasePieceLocationMarker.GetNearest(transCam.position, transCam.forward, true, examples);

        if (!marker) return true;

        if (Vector3.Distance(transCam.position, marker.pos) > placeMaxDistance) return true;



        var face = new Base.Face(cell, Base.Direction.Below);
        if (__instance.anchoredFace == null || __instance.anchoredFace.Value != face)
        {
            __instance.anchoredFace = face;



            __instance.UpdateSize(size);

            __instance.ghostBase.ClearMasks();


            for (int i = 0; i < 2; i++)
            {
                __instance.ghostBase.SetFaceType(face, Base.FaceType.WaterPark);
                __instance.ghostBase.SetFaceMask(face, true);
                face.direction = Base.OppositeDirections[(int)face.direction];
            }
            foreach (Base.Direction direction2 in Base.HorizontalDirections)
            {
                face.direction = direction2;
                __instance.ghostBase.SetFaceType(face, Base.FaceType.Solid);
                __instance.ghostBase.SetFaceMask(face, true);
            }

            __instance.RebuildGhostGeometry(true);

            for (int i = 0; i < __instance.ghostBase.cells.Length; i++)
                __instance.ghostBase.cells[i] = Base.CellType.OccupiedByOtherCell;
            __instance.ghostBase.cells[0] = Base.CellType.Room;
        }


        ghostModelParentConstructableBase.transform.position = marker.pos - new Vector3(5, 0, 5); ;//offset to account for the base offset that's applied for some reason
        ghostModelParentConstructableBase.transform.rotation = marker.rot;
        ghostModelParentConstructableBase.transform.parent = marker.transform;
        positionFound = true;
        geometryChanged = true;
        __result = true;

        return false;
    }
}

