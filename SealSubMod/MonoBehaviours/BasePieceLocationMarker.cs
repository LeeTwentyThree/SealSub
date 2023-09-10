using Nautilus.Json;
using SealSubMod.Patches.BasePatches;
using Unity.Mathematics;
using UnityEngine.Timeline;
using static ClipMapManager;
using static VFXParticlesPool;

namespace SealSubMod.MonoBehaviours;


public class BasePieceLocationMarker : MonoBehaviour
{
    public float builtPercent => !PieceObject ? 0 : PieceObject.GetComponent<Constructable>().constructedAmount;
    private Base.Piece _attachedBasePiece = Base.Piece.Invalid;//use for serialization too
    public Base.Piece AttachedBasePiece
    {
        get
        {
            if (!PieceObject)//If the piece was removed
                _attachedBasePiece = Base.Piece.Invalid;//should update this value propery, but I'm lazy and this works well enough
            return _attachedBasePiece;
        }
        set => _attachedBasePiece = value;
    }

    public GameObject PieceObject = null;

    public Vector3 pos => transform.position;
    public quaternion rot => transform.rotation;

    public static BasePieceLocationMarker GetNearest(Vector3 position, Vector3 lineDir, bool mustBeFree, params BasePieceLocationMarker[] markers)
    {
        BasePieceLocationMarker shortestMarker = null;
        var shortestDiff = float.MinValue;
        foreach (var marker in markers)
        {
            if (mustBeFree && marker.AttachedBasePiece != Base.Piece.Invalid) continue;

            var diff = Vector3.Dot(lineDir, marker.pos - position);

            if (diff > shortestDiff)
            {
                shortestDiff = diff;
                shortestMarker = marker;
            }
        }
        return shortestMarker;
    }

    public Dictionary<Base.Piece, TechType> pieceTypes = new()
    {
        { Base.Piece.RoomWaterParkBottom, TechType.BaseWaterPark },
        { Base.Piece.RoomBioReactor, TechType.BaseBioReactor },
        { Base.Piece.RoomNuclearReactor, TechType.BaseNuclearReactor },
        { Base.Piece.RoomFiltrationMachine, TechType.BaseFiltrationMachine },
    };

    public IEnumerator Start()
    {
        var saveData = GetComponentInParent<SealSubRoot>().SaveData;
        if (!saveData.basePieces.TryGetValue(name, out var basePieces)) yield break;
        if (basePieces.pieceType == Base.Piece.Invalid) yield break;

        ErrorMessage.AddMessage($"1");
        var task = CraftData.GetPrefabForTechTypeAsync(pieceTypes[basePieces.pieceType]);
        yield return task;
        var prefab = task.GetResult();
        ErrorMessage.AddMessage($"17: {prefab}, {pieceTypes[basePieces.pieceType]}");
        var constructableBase = Instantiate(prefab).GetComponent<ConstructableBase>();
        ErrorMessage.AddMessage($"2: {constructableBase}, {(constructableBase ? constructableBase.model : null)}");
        var ghost = constructableBase.model.GetComponent<BaseGhost>();
        ghost.SetupGhost();
        ErrorMessage.AddMessage($"3");


        constructableBase.tr.position = transform.position;
        //constructableBase.tr.localPosition += -new Vector3(5, 0, 5);//offset to account for the base offset that's applied for some reason (yes I'm still copy pasting these comments)


        ghost.Place();

        PieceObject = ghost.gameObject;
        AttachedBasePiece = basePieces.pieceType;

        if(ghost is BaseAddModuleGhost moduleGhost)
        {
            var direction = basePieces.direction;
            var face = new Base.Face(BaseModuleGhostPatches.cell, direction);
            moduleGhost.anchoredFace = face;
        }


        constructableBase.SetState(false, true);
        ErrorMessage.AddMessage($"4");

        constructableBase.constructedAmount = basePieces.constructedAmount;
        constructableBase.UpdateMaterial();
        if (constructableBase.constructedAmount >= 1) constructableBase.SetState(true, true);
        ErrorMessage.AddMessage($"5");
    }

    public static Base.Piece pieceTest = Base.Piece.RoomNuclearReactor;
    public static float constructedTest = 1;
    public static Base.Direction directionTest = Base.Direction.North;
    public void Test()
    {
        var saveData = GetComponentInParent<SealSubRoot>().SaveData;
        saveData.basePieces[name] = new (pieceTest, constructedTest, directionTest);
        UWE.CoroutineHost.StartCoroutine(Start());
    }

    private void OnEnable()
    {
        Plugin.SaveCache.OnStartedSaving += OnBeforeSave;
    }
    private void OnDisable()
    {
        Plugin.SaveCache.OnStartedSaving -= OnBeforeSave;
    }

    public void OnBeforeSave(object sender, JsonFileEventArgs args)
    {
        var saveData = GetComponentInParent<SealSubRoot>().SaveData;
        saveData.basePieces[name] = new(AttachedBasePiece, PieceObject ? PieceObject.GetComponent<Constructable>().constructedAmount : 0, GetDirection());
    }

    public Base.Direction GetDirection() => Base.Direction.North;
}