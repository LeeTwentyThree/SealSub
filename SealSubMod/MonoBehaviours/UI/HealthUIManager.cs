using SealSubMod.Interfaces;
using UnityEngine.UI;

namespace SealSubMod.MonoBehaviours.UI;

public class HealthUIManager : MonoBehaviour, IUIElement
{
    [SerializeField] LiveMixin subLiveMixin;
    [SerializeField] Image hpBar;

    public void UpdateUI()
    {
        float healthFraction = subLiveMixin.GetHealthFraction();
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, healthFraction, Time.deltaTime * 2f);
    }
}
