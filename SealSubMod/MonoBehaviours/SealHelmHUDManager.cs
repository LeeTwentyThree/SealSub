using UnityEngine.UI;
using TMPro;

namespace SealSubMod.MonoBehaviours;

internal class SealHelmHUDManager : MonoBehaviour
{
    [SerializeField] SubRoot subRoot;
    [SerializeField] LiveMixin subLiveMixin;
    [SerializeField] CyclopsMotorMode motorMode;
    [SerializeField] BehaviourLOD LOD;
    [SerializeField] GameObject hornObject;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] CrushDamage crushDamage;
    [SerializeField] CyclopsNoiseManager noiseManager;

    [SerializeField] GameObject engineOffIndicator;
    [SerializeField] TextMeshProUGUI powerText;
    [SerializeField] TextMeshProUGUI depthText;
    [SerializeField] Image hpBar;
    [SerializeField] Image noiseBar;

    private bool hudActive;

    private int lastDisplayedPowerPercentage = -1;
    private int lastDisplayedDepth = -1;
    private int lastDisplayedCrushDepth = -1;

    private void OnValidate()
    {
        if (subRoot == null) subRoot = gameObject.GetComponentInParent<SubRoot>();
        if (subLiveMixin == null) subLiveMixin = gameObject.GetComponentInParent<LiveMixin>();
        if (motorMode == null) motorMode = gameObject.GetComponentInParent<CyclopsMotorMode>();
        if (LOD == null) LOD = gameObject.GetComponentInParent<BehaviourLOD>();
    }

    private void Update()
    {
        if (!LOD.IsFull())
        {
            return;
        }
        if (subLiveMixin.IsAlive())
        {
            engineOffIndicator.SetActive(!motorMode.engineOn);

            float healthFraction = subLiveMixin.GetHealthFraction();
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, healthFraction, Time.deltaTime * 2f);
            float noisePercent = noiseManager.GetNoisePercent();
            noiseBar.fillAmount = Mathf.Lerp(noiseBar.fillAmount, noisePercent, Time.deltaTime);

            int powerPercentage = subRoot.powerRelay.GetMaxPower() == 0 ? 0 : Mathf.CeilToInt(subRoot.powerRelay.GetPower() / subRoot.powerRelay.GetMaxPower() * 100f);
            if (lastDisplayedPowerPercentage != powerPercentage)
            {
                powerText.text = string.Format("{0}%", powerPercentage);
                lastDisplayedPowerPercentage = powerPercentage;
            }

            int depth = (int)crushDamage.GetDepth();
            int crushDepth = (int)crushDamage.crushDepth;

            Color depthTextColor = Color.white;

            if (depth >= crushDepth)
            {
                depthTextColor = Color.red;
            }

            if (lastDisplayedDepth != depth || lastDisplayedCrushDepth != crushDepth)
            {
                lastDisplayedDepth = depth;
                lastDisplayedCrushDepth = crushDepth;
                depthText.text = string.Format("{0}m / {1}m", depth, crushDepth);
            }

            depthText.color = depthTextColor;
        }
        if (Player.main.currentSub == subRoot && !subRoot.subDestroyed)
        {
            // Implement other display stuff here...

            if (hudActive)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, Time.deltaTime * 3f);
                canvasGroup.interactable = true;
            }
            else
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime * 3f);
                canvasGroup.interactable = false;
            }
        }
    }

    public void StartPiloting()
    {
        hudActive = true;
        hornObject.SetActive(true);
    }

    public void StopPiloting()
    {
        hudActive = false;
        hornObject.SetActive(false);
    }

    public void OnTakeCollisionDamage(float value)
    {
        value *= 1.5f;
        value = Mathf.Clamp(value / 100f, 0.5f, 1.5f);
        MainCameraControl.main.ShakeCamera(value, -1f, MainCameraControl.ShakeMode.Linear, 1f);
    }
}