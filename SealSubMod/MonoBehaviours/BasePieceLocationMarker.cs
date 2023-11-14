using Nautilus.Json;
using SealSubMod.Interfaces;
using SealSubMod.Patches.BasePatches;
using Unity.Mathematics;
using UnityEngine.Timeline;
using UWE;
using static ClipMapManager;
using static SealSubMod.SaveData;
using static VFXParticlesPool;

namespace SealSubMod.MonoBehaviours;


public class BasePieceLocationMarker : MonoBehaviour, IOnSaveDataLoaded
{
    public static Dictionary<Base.Piece, TechType> pieceTypes = new()
    {
        { Base.Piece.RoomWaterParkBottom, TechType.BaseWaterPark },
        { Base.Piece.RoomBioReactor, TechType.BaseBioReactor },
        { Base.Piece.RoomNuclearReactor, TechType.BaseNuclearReactor },
        { Base.Piece.RoomFiltrationMachine, TechType.BaseFiltrationMachine },
    };



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


    public static BasePieceLocationMarker GetNearest(Vector3 position, Vector3 lineDir, bool mustBeFree, params BasePieceLocationMarker[] markers)
    {
        BasePieceLocationMarker shortestMarker = null;
        var shortestDiff = float.MinValue;
        foreach (var marker in markers)
        {
            if (mustBeFree && marker.AttachedBasePiece != Base.Piece.Invalid) continue;

            var diff = Vector3.Dot(lineDir, marker.transform.position - position);

            if (diff > shortestDiff)
            {
                shortestDiff = diff;
                shortestMarker = marker;
            }
        }
        return shortestMarker;
    }

    void IOnSaveDataLoaded.OnSaveDataLoaded(SaveData saveData)
    {
        Plugin.Logger.LogMessage($"smth, {saveData.basePieces.Count}");
        if (saveData.basePieces.TryGetValue(name, out var basePieces))
            CoroutineHost.StartCoroutine(LoadPiece(basePieces));
    }

    public IEnumerator LoadPiece(BasePieceSaveData data)
    {
        if (data.pieceType == Base.Piece.Invalid) yield break;

        ErrorMessage.AddMessage($"1");
        var task = CraftData.GetPrefabForTechTypeAsync(pieceTypes[data.pieceType]);
        yield return task;
        var prefab = task.GetResult();
        ErrorMessage.AddMessage($"17: {prefab}, {pieceTypes[data.pieceType]}");
        var constructableBase = Instantiate(prefab).GetComponent<ConstructableBase>();
        ErrorMessage.AddMessage($"2: {constructableBase}, {(constructableBase ? constructableBase.model : null)}");
        var ghost = constructableBase.model.GetComponent<BaseGhost>();
        ghost.SetupGhost();
        ErrorMessage.AddMessage($"3");


        constructableBase.tr.position = transform.position;
        constructableBase.tr.parent = transform;

        yield return new WaitUntil(() => LargeWorld.main.streamer.globalRoot);//this seems to be null when done too early for some reason?
        ghost.Place();

        PieceObject = ghost.gameObject;
        AttachedBasePiece = data.pieceType;

        if(ghost is BaseAddModuleGhost moduleGhost)
        {
            var direction = data.direction;
            var face = new Base.Face(BaseModuleGhostPatches.cell, direction);
            moduleGhost.anchoredFace = face;
        }


        constructableBase.SetState(false, true);
        ErrorMessage.AddMessage($"4");

        constructableBase.constructedAmount = data.constructedAmount;
        constructableBase.UpdateMaterial();
        if (constructableBase.constructedAmount >= 1)
        {
            constructableBase.SetState(true, true);
            //PieceObject is set in the BaseModuleGhost.Finish patch, and is set to the "geometry" object
            //aka the model
            //The constructable base is I *think* destroyed?
            //Been a while don't quite remember what happens to it anymore
            PieceObject.transform.localPosition -= new Vector3(5, 0, 5);
        }
        ErrorMessage.AddMessage($"5");
    }
    public static Base.Direction directionTest = Base.Direction.North;
    public Base.Direction GetDirection() => directionTest;




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


}