using UnityEngine;
using JoonyleGameDevKit;

public sealed class PlayerFallingState : StateBase<PlayerBehaviour>
{
    public override void Enter(PlayerBehaviour owner)
    {
        owner.SlingBehaviour.SetPhysicsInAir();

        owner.PlayAnimation("Roll");
        owner.AlignToCenter();
    }

    public override void Exit(PlayerBehaviour owner) { }

    public override void Update(PlayerBehaviour owner)
    {
        owner.SetFacingDirection(owner.Rigid.linearVelocity.x);

        // var pointerInput = owner.PointerInput;
        // var pointerDragDelta = pointerInput.GetScreenDragDelta;
        // var velocity = owner.Rigid.linearVelocity;
        // var normalizedX = Mathf.Clamp(pointerDragDelta.x / owner.FallingJoystickRange, -1f, 1f);
        // velocity.x = pointerInput.IsDragging ? normalizedX * owner.FallingMoveSpeed : 0f;
        // owner.Rigid.linearVelocity = velocity;
    }
}
