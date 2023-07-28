using HarmonyLib;
using SealSubMod.MonoBehaviours;

namespace SealSubMod.Patches;

[HarmonyPatch(typeof(Player))]
internal class PlayerPatches
{
    [HarmonyPatch(nameof(Player.CanEject))]
    public static void Postfix(ref bool __result)
    {
        if(Player.main.currentSub is SealSubRoot) __result = true;
    }

    [HarmonyPatch(nameof(Player.SetMotorMode))]//the game checks for subroot before vehicle, this fixes that
    public static bool Prefix(Player.MotorMode newMotorMode)
    {
        if (Player.main.currentSub is not SealSubRoot) return true;//not a seal
        if (!Player.main.currentMountedVehicle) return true;//not in a docked vehicle
        if (newMotorMode != Player.MotorMode.Walk) return true;//honestly not necessary but safety check works ig

        Player.main.SetMotorMode(Player.MotorMode.Vehicle);
        //want to be safe here, it's better to accidentally return true than accidentally return false
        return false;
    }

    [HarmonyPatch(nameof(Player.ExitLockedMode))]
    public static void Prefix(ref bool findNewPosition)
    {
        if (Player.main.currentSub is not SealSubRoot) return;
        if (!Player.main.currentMountedVehicle) return; 

        findNewPosition = true;
    }
}


public class ASD : BaseAddModuleGhost
{
    // BaseAddModuleGhost
    // Token: 0x060003D8 RID: 984 RVA: 0x0001BCEC File Offset: 0x00019EEC
    public override bool UpdatePlacement(Transform camera, float placeMaxDistance, out bool positionFound, out bool geometryChanged, ConstructableBase ghostModelParentConstructableBase)
    {
        positionFound = false;
        geometryChanged = Builder.UpdateRotation(Base.HorizontalDirections.Length);




        Player Player = Player.main;
        if (Player == null || Player.currentSub == null || /**/!Player.currentSub.isBase)//issue here. Sub isn't a base
        {
            geometryChanged = this.SetupInvalid();
            return false;
        }
        this.targetBase = BaseGhost.FindBase(camera, 20f);//same here. Subs aren't bases, but subroots
        if (this.targetBase == null)
        {
            geometryChanged = this.SetupInvalid();
            return false;
        }


        this.targetBase.SetPlacementGhost(this);//just a field set


        ConstructableBase ConstructableBase = base.GetComponentInParent<ConstructableBase>();
        float distance = (ConstructableBase != null) ? ConstructableBase.placeDefaultDistance : 0f;


        Base.Direction direction = Base.HorizontalDirections[Builder.lastRotation % Base.HorizontalDirections.Length];

        Base.Face correctFace = new Base.Face(this.targetBase.WorldToGrid(camera.position + camera.forward * distance), direction);
        //"correctFace".cell is always 1, 0, 1 for a single multipurpose room. May change for other rooms or base sizes
        //direction is based on the direction the builder tool was rotated (using mouse scroll wheel), defaults to north always?


        if (!this.targetBase.CanSetModule(ref correctFace, this.faceType))//checks if the room is valid and the sides are open and the top and bottom are closed
        {//need to patch .CanSetModule too
            geometryChanged = this.SetupInvalid();
            return false;
        }


        Int3 cellNormalized = this.targetBase.NormalizeCell(correctFace.cell);//Some byte shit idk. Mostly able to ignore though, mostly just returns same value

        var anchor = this.targetBase.GetAnchor();//I believe this is just the "starting point" or maybe the center?
        Base.Face face2 = new Base.Face(correctFace.cell - anchor, correctFace.direction);



        //anchored face is null when not snapped
        //else is presumably the face it is snapped to
        //example: `Face (1,0,1) North`
        if (this.anchoredFace == null || this.anchoredFace.Value != face2)
        {//triggers when NOT snapped to the "correct" face

            this.anchoredFace = new Base.Face?(face2);//sets the anchor (snap point) to the "correct face"


            Base.CellType cell = this.targetBase.GetCell(cellNormalized);//the room type. Can PROBABLY just use Base.CellType.Room


            //the available space within the room
            //for multipurpose rooms (Base.CellType.Room) it's (3, 1, 3) which means it is three slots wide on both sides and one slot tall
            Int3 roomSize = Base.CellSize[(int)cell];
            this.UpdateSize(roomSize);

                                                                                            //should equal 3,0,3       should equal -1,0,-1
            this.ghostBase.CopyFrom(this.targetBase, new Int3.Bounds(cellNormalized, (cellNormalized + roomSize - 1)), cellNormalized * -1);
            //I'm ultimately not entirely sure what this method does. Need to check around a bit more later


            //I think it would be the offset between the correct cell and the normalized one?
            //but for multipurpose rooms this should just be 0,0,0
            Int3 offset = correctFace.cell - cellNormalized;
            //on second viewing this definition seems inaccurate. I'm not sure what this is


            //I would initially think this to be an offset face
            //but it's treated as the final position
            Base.Face finalPositionMaybe = new Base.Face(offset, correctFace.direction);


            this.ghostBase.SetFaceType(finalPositionMaybe, this.faceType);
            this.ghostBase.ClearMasks();
            this.ghostBase.SetFaceMask(finalPositionMaybe, true);
            base.RebuildGhostGeometry(true);
            geometryChanged = true;
        }
        ghostModelParentConstructableBase.transform.position = this.targetBase.GridToWorld(cellNormalized);
        ghostModelParentConstructableBase.transform.rotation = this.targetBase.transform.rotation;
        positionFound = true;
        return !this.targetBase.IsCellUnderConstruction(correctFace.cell) && (ghostModelParentConstructableBase.transform.position.y <= float.PositiveInfinity || BaseGhost.GetDistanceToGround(ghostModelParentConstructableBase.transform.position) <= 25f);
    }
}