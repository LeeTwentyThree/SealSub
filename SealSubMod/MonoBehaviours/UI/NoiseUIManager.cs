using SealSubMod.Interfaces;
using UnityEngine.UI;

namespace SealSubMod.MonoBehaviours.UI;

internal class NoiseUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] CyclopsNoiseManager noiseManager;
    [SerializeField] Image noiseBar;

    public void UpdateUI()
    {
        float noisePercent = noiseManager.GetNoisePercent();
        noiseBar.fillAmount = Mathf.Lerp(noiseBar.fillAmount, noisePercent, Time.deltaTime);
    }
}
