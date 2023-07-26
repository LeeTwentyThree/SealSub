namespace SealSubMod.MonoBehaviours;

internal class SealDockingBay : VehicleDockingBay
{
    //Mostly a marker class, to distinguish between normal docking bays and the seal bay in patches
    public static float MaxDistance = 2;
    public static float MinForce = 0.001f;
    public static float MaxForce = 3;
    public static ForceMode ForceMode = ForceMode.VelocityChange;

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
    internal void UpdateVehclPos(Vehicle vehcl)
    {
        var vehclPos = vehcl.transform.position;
        var dockPos = dockingEndPos.position;

        var direction = dockPos - vehclPos;
        var distance = direction.magnitude;

        var force = Mathf.Lerp(MinForce, MaxForce, distance / MaxDistance) * direction;

        vehcl.useRigidbody.AddForce(force, ForceMode);

        vehcl.useRigidbody.velocity *= Mathf.Pow(0.1f, Time.deltaTime);
    }
}
