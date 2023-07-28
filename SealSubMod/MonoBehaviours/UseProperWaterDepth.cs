namespace SealSubMod.MonoBehaviours;

internal class UseProperWaterDepth : MonoBehaviour
{
    [SerializeField] float yOffset = 5;

    private void Start()
    {
        GetComponent<WorldForces>().waterDepth = Ocean.GetOceanLevel() + yOffset;
    }
}