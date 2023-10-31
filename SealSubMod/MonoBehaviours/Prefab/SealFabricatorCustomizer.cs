using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealSubMod.MonoBehaviours.Prefab;

//Placed on an object with `AddressablesPrefabSpawn` component on it for spawning the cyclops fabricator
//Customizes said fabricator to fit the seal
internal class SealFabricatorCustomizer : MonoBehaviour
{
    private Fabricator fabricator;
    public void Start()
    {
        fabricator = transform.GetChild(0).gameObject.GetComponent<Fabricator>();//no real damage if this errors, just a log and a non functional component, no different than a safety check would've been

        fabricator.handOverText = "Use Seal Upgrade Fabricator";
        fabricator.craftTree = Plugin.SealFabricatorTree;
    }
}
