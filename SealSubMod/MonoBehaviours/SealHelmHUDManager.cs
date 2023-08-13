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

    [SerializeField] GameObject engineOffIndicator;

    private bool hudActive;

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