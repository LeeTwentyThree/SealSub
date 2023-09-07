using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

// the class that spawns the power cell model
internal class SpawnPowerCellModel : MonoBehaviour, ICyclopsReferencer
{
    private static GameObject[] _cachedModels = null;

    // serialize field is my favorite access modifier (I love serialize field <3)
    [SerializeField] Transform parent;
    [SerializeField] Vector3 localPos;
    [SerializeField] Vector3 localAngles;
    [SerializeField] Vector3 localScale;
    [SerializeField] BatterySource batterySource;

    private void OnValidate()
    {
        batterySource = GetComponent<BatterySource>();
    }

    // isn't this such a useful interface??!?!??!?!? ikr?!?
    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        if (_cachedModels == null) LoadModels(cyclops);

        for(int i = 0; i < _cachedModels.Length; i++)
            batterySource.batteryModels[i].model = SpawnModel(_cachedModels[i]);

        // um what do we do for custom batteries...? eldritch, any ideas? is that even a concern?

        //Not sure there, could try get the batteries from the seamoth/cyclops directly though?
        //or if custom batteries is integrated into nautilus that would help with compat here
    }

    public void LoadModels(GameObject cyclops)
    {
        var powerCellModelReference = cyclops.transform.Find("cyclopspower/generator/SubPowerSocket1/SubPowerCell1/model").gameObject;
        var ionPowerCellModelReference = cyclops.transform.Find("cyclopspower/generator/SubPowerSocket1/SubPowerCell1/engine_power_cell_ion").gameObject;

        _cachedModels = new[]
        {
            powerCellModelReference, 
            ionPowerCellModelReference,
        };
    }

    // method name can't be same as class name, I wanted it to be called "SpawnPowerCellModel" as well :(
    private GameObject SpawnModel(GameObject modelReference)
    {
        var spawnedModel = Instantiate(modelReference, parent);

        spawnedModel.transform.localPosition = localPos;
        spawnedModel.transform.localEulerAngles = localAngles;
        spawnedModel.transform.localScale = localScale;

        return spawnedModel;
    }
}