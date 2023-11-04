using SealSubMod.Interfaces;

namespace SealSubMod.MonoBehaviours.UI;

public class EngineUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] CyclopsMotorMode motorMode;
    [SerializeField] GameObject engineOffIndicator;
    [SerializeField] Animator toggleSpeedSettingsAnimator;

    public void UpdateUI()
    {
        engineOffIndicator.SetActive(!motorMode.engineOn);
    }

    public void StartPiloting()
    {
        if (motorMode.engineOn)
        {
            toggleSpeedSettingsAnimator.SetTrigger("EngineOn");
            return;
        }
        toggleSpeedSettingsAnimator.SetTrigger("EngineOff");
    }

    public void InvokeChangeEngineState(bool changeToState)
    {
        if (changeToState)
        {
            toggleSpeedSettingsAnimator.SetTrigger("StartEngine");
            return;
        }
        toggleSpeedSettingsAnimator.SetTrigger("StopEngine");
    }
}
