using TMPro;

namespace SealSubMod.MonoBehaviours;

internal class SealDockingBay : VehicleDockingBay
{
    [SerializeField] FMOD_CustomLoopingEmitter _emitter;

    [SerializeField] TextMeshProUGUI _text;

    //Mostly a marker class, to distinguish between normal docking bays and the seal bay in patches
    public static float MaxDistance = 3;
    public static float MinForce = 0.001f;
    public static float MaxForce = 20;
    public static ForceMode ForceMode = ForceMode.VelocityChange;

    public static float dockPushOutForce = 30;
    public static float dockPullInForceMult = 0.2f;
    public static float dotProductLimit = 0.9f;


    public static bool debug = true;//useful
    public static bool doubleDebug = false;//sometimes useful but very screen spammy

    public void Awake()
    {
        onDockedChanged += OnDockChange;
    }

    private void OnDockChange()
    {
        if (dockedVehicle)
        {
            _emitter.Play();
            _text.text = $"{dockedVehicle.subName.GetName()} docked!";
        }
        else
        {
            _emitter.Stop();
            _text.text = "Ready to dock...";
        }
    }

    public void FixedUpdate()
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
            //dockedVehicle.useRigidbody.AddForce(outDirection * dockPushOutForce, ForceMode);//doesn't seem to work? Idk.
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
        var dockPos = dockingEndPos.position;

        var direction = dockPos - vehclPos;
        var distance = direction.magnitude;

        var interp = distance / MaxDistance;
        if (interpolationfractionnumbervalue != 1)//1 means it's fully docked, so if it's not fully docked it's still getting pulled in
            interp *= dockPullInForceMult;       //and it was way too fast at getting pulled in, so slow it down

        var force = Mathf.Lerp(MinForce, MaxForce, interp) * direction;

        vehcl.useRigidbody.AddForce(force * Time.deltaTime, ForceMode);

        vehcl.useRigidbody.velocity *= Mathf.Pow(0.1f, Time.deltaTime);
    }
}
