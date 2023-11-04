namespace SealSubMod.MonoBehaviours;

public class AnimateAlarm : MonoBehaviour
{
    [SerializeField] Transform lightsParent;
    [SerializeField] Vector3 rotationAxis;
    [SerializeField] float rotationDegreesPerSecond = 340;

    private bool active = false;

    private Material _glassMaterial;
    private Material _lightMaterial;
    private Light[] lights;

    private void Awake()
    {
        var r = GetComponentInChildren<Renderer>();
        var m = r.materials;
        _glassMaterial = m[1];
        _lightMaterial = m[2];
        lights = GetComponentsInChildren<Light>(true);
        SetAlarmEnabled(false);
    }
    public void SetAlarmEnabled(bool active)
    {
        this.active = active;
        _glassMaterial.SetFloat("_SpecInt", active ? 8f : 1f);
        _glassMaterial.SetColor("_Color", active ? new Color(1f, 0, 0, 0.74f) : new Color(0, 0, 0, 0.74f));
        _lightMaterial.SetFloat(ShaderPropertyID._GlowStrength, active ? 4f : 0f);
        _lightMaterial.SetFloat(ShaderPropertyID._GlowStrengthNight, active ? 4f : 0f);
        lights.ForEach((l) => l.enabled = active);
    }

    private void Update()
    {
        if (active) lightsParent.localEulerAngles += rotationAxis * rotationDegreesPerSecond * Time.deltaTime;
    }

    private void OnDestroy()
    {
        Destroy(_glassMaterial);
        Destroy(_lightMaterial);
    }
}