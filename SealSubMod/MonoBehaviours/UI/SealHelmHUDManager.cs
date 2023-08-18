using UnityEngine.UI;
using TMPro;
using SealSubMod.Interfaces;

namespace SealSubMod.MonoBehaviours;

internal class SealHelmHUDManager : MonoBehaviour
{
    [SerializeField] SubRoot subRoot;
    [SerializeField] LiveMixin subLiveMixin;
    [SerializeField] BehaviourLOD LOD;
    [SerializeField] GameObject hornObject;
    [SerializeField] CanvasGroup canvasGroup;

    private bool hudActive;

    private List<IUIElement> uiElements;

    private void OnValidate()
    {
        if (subRoot == null) subRoot = gameObject.GetComponentInParent<SubRoot>();
        if (subLiveMixin == null) subLiveMixin = gameObject.GetComponentInParent<LiveMixin>();
        if (LOD == null) LOD = gameObject.GetComponentInParent<BehaviourLOD>();
    }

    private void Start()
    {
        canvasGroup.alpha = 0;
        uiElements = GetComponentsInChildren<IUIElement>().ToList();
    }

    private void Update()
    {
        if (!LOD.IsFull())
        {
            return;
        }
        if (subLiveMixin.IsAlive())
        {   
            foreach (var uiElement in uiElements) uiElement.UpdateUI();
        }

        if (Player.main.currentSub != subRoot || subRoot.subDestroyed)
            return;

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

    // SHOULD BE REPLACED WITH A NEW METHOD LATER BECAUSE WE ARE NOT USING THE SubFire CLASS!!!
    // This is a Unity message that is broadcasted by SubFire.OnTakeDamage

    public void OnTakeCollisionDamage(float value)
    {
        value *= 1.5f;
        value = Mathf.Clamp(value / 100f, 0.5f, 1.5f);
        MainCameraControl.main.ShakeCamera(value, -1f, MainCameraControl.ShakeMode.Linear, 1f);
    }
}