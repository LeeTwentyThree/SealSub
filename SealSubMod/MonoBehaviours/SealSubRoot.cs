namespace SealSubMod.MonoBehaviours;

internal class SealSubRoot : SubRoot
{
    [SerializeField] SealUpgradeConsole[] _consoles;
    public SealUpgradeConsole[] Consoles { get => _consoles; }
}
