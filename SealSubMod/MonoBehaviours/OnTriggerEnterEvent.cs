using UnityEngine.Events;

namespace SealSubMod.MonoBehaviours;

internal class OnTriggerEnterEvent : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent onTriggerStay;
    private void OnTriggerEnter(Collider collider)
    {
        onTriggerEnter.Invoke();
    }
    private void OnTriggerExit(Collider collider)
    {
        onTriggerExit.Invoke();
    }
    private void OnTriggerStay(Collider collider)
    {
        onTriggerStay.Invoke();
    }
}
