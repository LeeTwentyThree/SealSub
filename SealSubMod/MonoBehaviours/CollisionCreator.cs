using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SealSubMod.MonoBehaviours;

internal class CollisionCreator : MonoBehaviour
{
    [SerializeField]
    private MeshFilter mesh;
    public CollisionMarker collisionParent;

    public MeshFilter Mesh
    { 
        get 
        { 
            if(!mesh) mesh = GetComponent<MeshFilter>();
            return mesh; 
        } 
    }
    public void Awake()
    {
        var collider = new GameObject(Mesh.name + "Collision");
        collider.transform.parent = collisionParent.transform;
        collider.transform.position = Mesh.transform.position;
        collider.transform.rotation = Mesh.transform.rotation;
        collider.transform.localScale = Mesh.transform.localScale;

        collider.AddComponent<MeshCollider>().sharedMesh = Mesh.mesh;
    }
}
public class CollisionMarker : MonoBehaviour//useless class, just here so that I can put it on a single object as the collision parent
{
    //and then I don't have to pick the correct gameobject out of 30
    //cause there's only one here :)
}
