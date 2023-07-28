using HarmonyLib;
using SealSubMod.MonoBehaviours;
using Unity.Mathematics;

namespace SealSubMod.Patches.BasePatches;

[HarmonyPatch(typeof(BaseAddModuleGhost))]
internal class BaseModuleGhostPatches
{
    public static bool posFound = false;
    public static bool geomChange = true;
    [HarmonyPatch(nameof(BaseAddModuleGhost.UpdatePlacement))]
    public static bool Prefix(BaseAddModuleGhost __instance, Transform camera, float placeMaxDistance, ref bool positionFound, ref bool geometryChanged, ConstructableBase ghostModelParentConstructableBase, ref bool __result)
    {
        if (Player.main.currentSub is not SealSubRoot seal) return true;

        var examples = seal.GetComponentsInChildren<BasePieceLocationMarker>(true);

        var transCam = camera;
        var marker = BasePieceLocationMarker.GetNearest(transCam.position, transCam.forward, examples);

        if(!marker) return true;

        if (Vector3.Distance(transCam.position, marker.pos) > placeMaxDistance) return true;


        ghostModelParentConstructableBase.transform.position = marker.pos;
        ghostModelParentConstructableBase.transform.rotation = marker.rot;
        positionFound = posFound;
        geometryChanged = geomChange;
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
