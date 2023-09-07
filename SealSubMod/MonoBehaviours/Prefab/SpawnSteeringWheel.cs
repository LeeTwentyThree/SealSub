using SealSubMod.Interfaces;
using SealSubMod.MonoBehaviours.Abstract;
using SealSubMod.Utility;

namespace SealSubMod.MonoBehaviours.Prefab;

internal class SpawnSteeringWheel : MonoBehaviour, ICyclopsReferencer
{
    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var model = cyclops.transform.Find("CyclopsMeshAnimated/Submarine_Steering_Console").gameObject;
        var spawned = Instantiate(model, transform);
        spawned.transform.localPosition = Vector3.zero;
        spawned.transform.localEulerAngles = new Vector3(-90, 0, 0);
        var pilotingChair = spawned.GetComponent<PilotingChair>();
        pilotingChair.subRoot = gameObject.GetComponentInParent<SubRoot>(true);
        gameObject.GetComponentInParent<SubControl>(true).mainAnimator = gameObject.GetComponentInChildren<Animator>();
    }
}