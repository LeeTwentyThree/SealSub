using Unity.Mathematics;

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

    public void Awake()
    {
        if (!Plugin.SaveCache.saves.TryGetValue(GetComponentInParent<PrefabIdentifier>().Id, out var saveData)) return;

        if (!saveData.basePieces.TryGetValue(name, out var basePieces)) return;

        //do shit here (lazy);
        //Will do tomorrow;
    }
}