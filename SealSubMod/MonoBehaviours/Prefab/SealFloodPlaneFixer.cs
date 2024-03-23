using SealSubMod.MonoBehaviours.Abstract;
using UWE;

namespace SealSubMod.MonoBehaviours.Prefab;

public class SealFloodPlaneFixer : PrefabModifierAsync
{
    public BaseWaterPlane waterPlane;

    public override IEnumerator SetupPrefabAsync()
    {
        var request = PrefabDatabase.GetPrefabForFilenameAsync("Assets/Prefabs/Base/GeneratorPieces/BaseRoom.prefab");
        yield return request;
        if (!request.TryGetPrefab(out var baseRoomPrefab))
        {
            Plugin.Logger.LogError("Failed to load the multipurpose room prefab!");
            yield break;
        }

        var referenceWaterPlane = baseRoomPrefab.transform.Find("Flood_BaseRoom").GetComponent<BaseWaterPlane>();
        var planeMesh = Instantiate(referenceWaterPlane.transform.Find("x_BaseWaterPlane_RoomGen").gameObject,
            waterPlane.transform);
        planeMesh.transform.localPosition = Vector3.zero;
        var fogMesh = Instantiate(referenceWaterPlane.transform.Find("x_BaseWaterFog_BaseRoom").gameObject,
            waterPlane.transform);
        fogMesh.SetActive(false);
        fogMesh.transform.localPosition = Vector3.zero;
        waterPlane.waterPlane = planeMesh.transform;
        waterPlane.waterRender = planeMesh.GetComponent<Renderer>();
        waterPlane.fogRenderers = new[] {fogMesh.GetComponent<Renderer>()};
        var childTransforms = planeMesh.GetComponentsInChildren<Transform>();
        var children = new GameObject[childTransforms.Length];
        for (int i = 0; i < childTransforms.Length; i++)
        {
            children[i] = childTransforms[i].gameObject;
        }
        waterPlane.children = children;
    }
}