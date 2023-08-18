using SealSubMod.Interfaces;

namespace SealSubMod.MonoBehaviours;

internal class PropCannonBeamFX : MonoBehaviour, IOnDockChange
{
    public static float VectorRandomRange = 0.85f;
    public static float centerOffset = 0.1f;



    private static GameObject _beamFXPrefab;
    private bool _coroutineRunning = false;

    private GameObject _beamFX;
    private Transform _target;
    private Vector3 _noise;

    public void OnDockChange(Vehicle vehicle)
    {
        //vehicle?.transform doesn't work great with unity because of destroyed objects, iirc
        _target = vehicle ? vehicle.transform : null;

        //set once when the vehicle is docked, that way it's random each dock
        //but not constantly changing too much
        _noise = new Vector3(Random.Range(-VectorRandomRange, VectorRandomRange), Random.Range(-VectorRandomRange, VectorRandomRange), Random.Range(-VectorRandomRange, VectorRandomRange));
    }

    public void Update()
    {
        if (!_beamFX) return;

        _beamFX.SetActive(_target);

        if(!_target) return;

        var origin = GetOriginPosition();
        var originVector = GetOriginVector();

        foreach (var beam in _beamFX.GetComponentsInChildren<VFXElectricLine>(true))
        {
            beam.target = GetTargetPosition();
            beam.origin = origin;

            //Make the beams come from just slightly different angles
            
            beam.originVector = originVector + _noise;
        }
    }

    public Vector3 GetTargetPosition()
    {
        return _target.position - (GetOriginVector() * centerOffset);
    }

    public Vector3 GetOriginVector()
    {
        return (_target.position - transform.position).normalized;
    }

    public Vector3 GetOriginPosition()
    {
        return transform.position;//update a bit
        //make it go along the track
        //At the closest point of the track to the target
    }

    public IEnumerator Start()
    {
        if(_beamFXPrefab)
        {
            _beamFX = Instantiate(_beamFXPrefab, transform.position, transform.rotation, transform);
            yield break;
        }

        while(_coroutineRunning) yield return null;
        if(_beamFXPrefab)
        {
            _beamFX = Instantiate(_beamFXPrefab, transform.position, transform.rotation, transform);
            yield break;
        }

        _coroutineRunning = true;

        var task = CraftData.GetPrefabForTechTypeAsync(TechType.PropulsionCannon);
        yield return task;
        _beamFXPrefab = task.GetResult().FindChild("xPropulsionCannon_Beams");


        _beamFX = Instantiate(_beamFXPrefab, transform.position, transform.rotation, transform);
        _coroutineRunning = false;
    }
}
