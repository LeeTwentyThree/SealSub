using SealSubMod.Interfaces;
using static Player;

namespace SealSubMod.MonoBehaviours.UI;

internal class EngineIconUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] CyclopsMotorMode motorMode;
    [SerializeField] GameObject engineOffIndicator;

    public void UpdateUI()
    {
        engineOffIndicator.SetActive(!motorMode.engineOn);
    }
}
