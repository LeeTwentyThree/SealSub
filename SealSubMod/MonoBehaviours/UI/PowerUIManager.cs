using SealSubMod.Interfaces;
using TMPro;

namespace SealSubMod.MonoBehaviours.UI;

internal class PowerUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] SubRoot subRoot;
    [SerializeField] TextMeshProUGUI powerText;

    private int lastDisplayedPowerPercentage = -1;

    public void UpdateUI()
    {
        int powerPercentage = subRoot.powerRelay.GetMaxPower() == 0 ? 0 : Mathf.CeilToInt(subRoot.powerRelay.GetPower() / subRoot.powerRelay.GetMaxPower() * 100f);
        if (lastDisplayedPowerPercentage != powerPercentage)
        {
            powerText.text = string.Format("{0}%", powerPercentage);
            lastDisplayedPowerPercentage = powerPercentage;
        }
    }
}
