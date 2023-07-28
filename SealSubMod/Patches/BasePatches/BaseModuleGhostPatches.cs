using HarmonyLib;
using SealSubMod.MonoBehaviours;
using Unity.Mathematics;
using static SubFire;

namespace SealSubMod.Patches.BasePatches;

[HarmonyPatch(typeof(BaseAddModuleGhost))]
internal class BaseModuleGhostPatches
{
    public static bool posFound = true;
    public static Int3 cell = new Int3(1, 0, 1);//todo
    public static Int3 size = new Int3(2, 1, 2);//fuck with these values and try find why they're so restricted


    [HarmonyPatch(nameof(BaseAddModuleGhost.UpdatePlacement))]
    public static bool Prefix(BaseAddModuleGhost __instance, Transform camera, float placeMaxDistance, ref bool positionFound, ref bool geometryChanged, ConstructableBase ghostModelParentConstructableBase, ref bool __result)
    {
        if (Player.main.currentSub is not SealSubRoot seal) return true;

        var examples = seal.GetComponentsInChildren<BasePieceLocationMarker>(true);

        var transCam = camera;
        var marker = BasePieceLocationMarker.GetNearest(transCam.position, transCam.forward, examples);

        if(!marker) return true;

        if (Vector3.Distance(transCam.position, marker.pos) > placeMaxDistance) return true;

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


        ghostModelParentConstructableBase.transform.position = marker.pos - new Vector3(5,0,5);//offset to account for the base offset that's applied for some reason
        ghostModelParentConstructableBase.transform.rotation = marker.rot;
        ghostModelParentConstructableBase.transform.parent = marker.transform;
        positionFound = posFound;
        geometryChanged = true;
        __result = true;

        return false;
    }
}

public class BasePieceLocationMarker : MonoBehaviour
{
    public Vector3 pos => transform.position;
    public quaternion rot => transform.rotation;

    //https://forum.unity.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/
    public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }

    public static BasePieceLocationMarker GetNearest(Vector3 lineStart, Vector3 lineDir, params BasePieceLocationMarker[] markers)
    {
        BasePieceLocationMarker shortestMarker = null;
        var shortestDist = float.MaxValue;
        foreach(var marker in markers)
        {
            var point = NearestPointOnLine(lineStart, lineDir, marker.pos);
            var dist = (point - lineStart).sqrMagnitude;
            if(dist < shortestDist)
            {
                shortestDist = dist;
                shortestMarker = marker;
            }
        }
        return shortestMarker;
    }
}
