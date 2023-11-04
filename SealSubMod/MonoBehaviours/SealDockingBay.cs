using SealSubMod.Interfaces;
using TMPro;

namespace SealSubMod.MonoBehaviours;

public class SealDockingBay : VehicleDockingBay
{
    [SerializeField] FMOD_CustomLoopingEmitter _emitter;

    [SerializeField] TextMeshProUGUI _text;

    public static float MaxDistance = 3;
    public static float MaxPilotingDistance = 1;
    public static float MinForce = 0.001f;
    public static float MaxForce = 20;
    public static ForceMode ForceMode = ForceMode.VelocityChange;

    public static float dockPushOutForce = 10;
    public static float dockPullInForceMult = 0.2f;
    public static float dotProductLimit = 0.9f;


    public static bool debug = false;//useful
    public static bool doubleDebug = false;//sometimes useful but very screen spammy

    public static float MaxAllowedDistance => Player.main?.GetPilotingChair() ? MaxPilotingDistance : MaxDistance;

    private void Awake()
    {
        onDockedChanged += OnDockChange;
        OnDockChange();
    }

    private void OnDockChange()
    {
        if (dockedVehicle)
        {
            _emitter.Play();
            string vehicleName = dockedVehicle.subName != null ? dockedVehicle.subName.GetName() : "vehicle";
            _text.text = $"{vehicleName} docked";
        }
        else
        {
            _emitter.Stop();
            _text.text = "Ready to dock";
        }

        UWE.Utils.GetEntityRoot(gameObject).GetComponentsInChildren<IOnDockChange>().ForEach((dockChange) => dockChange.OnDockChange(dockedVehicle));
    }

    private void FixedUpdate()
    {
        if (!dockedVehicle) return;
        if(!dockedVehicle.CanPilot() || !dockedVehicle.GetPilotingMode()) return;

        

        var outDirection = GetOutDirection();
        var dot = Vector3.Dot((dockedVehicle.transform.rotation * GameInput.GetMoveDirection()).normalized, outDirection.normalized);

        if(doubleDebug)ErrorMessage.AddMessage($"Dot product {dot}");

        if (dot >= dotProductLimit)
        {
            if(debug) ErrorMessage.AddMessage("Trying to undock!!!!!!!!!!!!!!");
            OnUndockingComplete(Player.main);
            dockedVehicle.useRigidbody.AddForce(outDirection * dockPushOutForce, ForceMode);
        }
    }

    public Vector3 GetOutDirection()
    {
        if (!exit || !dockedVehicle) return -transform.forward;//not super accurate but good enough for if the vehicle isn't docked
        return (exit.position - dockedVehicle.transform.position).normalized;//much more accurate
    }

    private new void LateUpdate()
    {
        Vehicle dockedVehicle = this.dockedVehicle;
        if (this.interpolatingVehicle != null)
        {
            dockedVehicle = this.interpolatingVehicle;
        }
        if (dockedVehicle != null)
        {
            float num = 1f;
            if (this.interpolationTime > 0f)
            {
                num = Mathf.Clamp01((Time.time - this.timeDockingStarted) / this.interpolationTime);
            }
            this.UpdateDockedPosition(dockedVehicle, num);
            if (this.interpolatingVehicle != null && num == 1f)
            {
                this.DockVehicle(this.interpolatingVehicle, false);
                this.interpolatingVehicle = null;
            }
        }
    }
    internal void UpdateVehclPos(Vehicle vehcl, float interpolationfractionnumbervalue)
    {
        var vehclPos = vehcl.transform.position;
        var dockPos = vehcl is Exosuit ? dockingEndPosExo.position : dockingEndPos.position;
        

        var direction = dockPos - vehclPos;
        var distance = direction.magnitude;

        var interp = distance / MaxAllowedDistance;
        if (interpolationfractionnumbervalue != 1)//1 means it's fully docked, so if it's not fully docked it's still getting pulled in
            interp *= dockPullInForceMult;       //and it was way too fast at getting pulled in, so slow it down

        var force = Mathf.Lerp(MinForce, MaxForce, interp) * direction;

        vehcl.useRigidbody.AddForce(force * Time.deltaTime, ForceMode);

        vehcl.useRigidbody.velocity *= Mathf.Pow(0.1f, Time.deltaTime);

        vehcl.UpdateCollidersForDocking(true);//This method is patched, so it'll enable colliders if the player isn't piloting, and disable them if they are
    }
}
