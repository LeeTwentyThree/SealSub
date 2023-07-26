namespace SealSubMod.MonoBehaviours;

internal class SealDockingBay : VehicleDockingBay
{
    //Mostly a marker class, to distinguish between normal docking bays and the seal bay in patches
    public static float MaxDistance = 3;
    public static float MinForce = 0.001f;
    public static float MaxForce = 4;
    public static ForceMode ForceMode = ForceMode.VelocityChange;

    public static float dockPushOutForce = 2;
    public static float dockPullInForceMult = 0.1f;
    public static float dotProductLimit = 0.9f;


    public static bool debug = true;//useful
    public static bool doubleDebug = false;//sometimes useful but very screen spammy

    public void FixedUpdate()
    {
        if (!dockedVehicle) return;
        if(!dockedVehicle.CanPilot() || !dockedVehicle.GetPilotingMode()) return;

        var outDirection = exit.position - dockedVehicle.transform.position;
        var dot = Vector3.Dot((dockedVehicle.transform.rotation * GameInput.GetMoveDirection()).normalized, outDirection.normalized);

        if(doubleDebug)ErrorMessage.AddMessage($"Dot product {dot}");

        if (dot >= dotProductLimit)
        {
            if(debug) ErrorMessage.AddMessage("Trying to undock!!!!!!!!!!!!!!");
            dockedVehicle.useRigidbody.AddForce(outDirection * dockPushOutForce, ForceMode);
            OnUndockingComplete(Player.main);
        }
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

        vehcl.useRigidbody.AddForce(force, ForceMode);

        vehcl.useRigidbody.velocity *= Mathf.Pow(0.1f, Time.deltaTime);
    }
}
