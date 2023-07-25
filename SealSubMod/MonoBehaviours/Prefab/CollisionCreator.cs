namespace SealSubMod.MonoBehaviours.Prefab;

internal class CollisionCreator : MonoBehaviour
{
    [SerializeField] MeshFilter mesh;
    public CollisionMarker collisionParent;
    public bool convex;

    public MeshFilter Mesh
    {
        get
        {
            if (!mesh) mesh = GetComponent<MeshFilter>();
            return mesh;
        }
    }

    public void OnValidate()
    {
        mesh = GetComponent<MeshFilter>();
        if (collisionParent == null)
        {
            collisionParent = transform.root.gameObject.GetComponentInChildren<CollisionMarker>();
        }
    }

    public void Awake()
    {
        var collider = new GameObject(Mesh.name + "Collision");
        collider.transform.parent = collisionParent.transform;
        collider.transform.position = Mesh.transform.position;
        collider.transform.rotation = Mesh.transform.rotation;
        collider.transform.localScale = Mesh.transform.localScale;

        var meshCollider = collider.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = Mesh.mesh;
        meshCollider.convex = convex;
    }
}
public class CollisionMarker : MonoBehaviour // useless class, just here so that I can put it on a single object as the collision parent
{
    // and then I don't have to pick the correct gameobject out of 30
    // cause there's only one here :)
}
