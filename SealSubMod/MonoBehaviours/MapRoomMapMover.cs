using static SealSubMod.MonoBehaviours.Prefab.MapRoomFunctionalitySpawner;

namespace SealSubMod.MonoBehaviours;

public class MapRoomMapMover : HandTarget, IHandTarget, IInputHandler
{
    public static float MoveSpeed = 0.02f;
    public bool Active { get; private set; }
    internal MiniWorldPosition miniWorld;

    public void Toggle()
    {
        if (Active) Disable();
        else Enable();
    }

    public void Disable()
    {
        Active = false;

        Player.main.ExitLockedMode(false, false);
    }

    public void Enable()
    {
        Active = true;

        InputHandlerStack.main.Push(this);
        Player.main.EnterLockedMode(null, false);
    }

    public bool HandleInput()
    {
        var moveDirection = GameInput.GetMoveDirection();
        var direction = MainCamera.camera.transform.rotation * moveDirection.WithY(0);//Remove Y for rotation, only rotate the X and Z values
        direction.y = 0;//Remove the Y again
        direction.Normalize();
        direction.y = moveDirection.y;//Re add the Y from the move direction
        //We avoid the Y value because otherwise you'll move it up/down if you're looking at any non-level angle
        //We want to only move up or down from space/C keys by default
        miniWorld.Offset += direction * MoveSpeed;

        if (GameInput.GetButtonDown(GameInput.Button.Exit) || GameInput.GetButtonDown(GameInput.Button.UICancel)) Disable();

        return Active;
    }

    bool IInputHandler.HandleLateInput()
    {
        return true;
    }

    void IInputHandler.OnFocusChanged(InputFocusMode mode)
    {

    }

    public void OnHandHover(GUIHand hand)
    {
        HandReticle.main.SetText(HandReticle.TextType.Hand, "UseMapMover", true, GameInput.Button.LeftHand);
    }

    public void OnHandClick(GUIHand hand)
    {
        Enable();
    }
}
