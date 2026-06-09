using UnityEngine;
using JoonyleGameDevKit;

public sealed class PlayerAttachedState : StateBase<PlayerBehaviour>
{
    public override void Enter(PlayerBehaviour owner)
    {
        owner.SlingBehaviour.SetPhysicsOnAnchor();

        if (owner.OccupyingPeg != null)
            owner.Rigid.position = owner.OccupyingPeg.transform.position;

        owner.PendulumAngularVelocity = 0f;
        owner.PlayAnimation("Attach");
        owner.AlignToHand();
    }

    public override void Exit(PlayerBehaviour owner) { }

    public override void Update(PlayerBehaviour owner)
    {
        var gravity = Physics2D.gravity.magnitude;
        var displacementRad = owner.Rigid.rotation * Mathf.Deg2Rad;
        var angularAccel = -(gravity / owner.PendulumLength) * Mathf.Sin(displacementRad) * Mathf.Rad2Deg;
        owner.PendulumAngularVelocity += angularAccel * Time.deltaTime;
        owner.PendulumAngularVelocity *= 1f - owner.PendulumDamping * Time.deltaTime;

        if (Mathf.Abs(owner.PendulumAngularVelocity) < 0.5f && Mathf.Abs(owner.Rigid.rotation) < 1f)
        {
            owner.PendulumAngularVelocity = 0f;
            owner.Rigid.MoveRotation(0f);
        }
        else
        {
            owner.Rigid.MoveRotation(owner.Rigid.rotation + owner.PendulumAngularVelocity * Time.deltaTime);
        }

        var pointerInput = owner.PointerInput;
        var pointerWorldPos = pointerInput.GetWorldPos;
        if (pointerInput.IsDragging
            && owner.SlingBehaviour.IsActiveSling
            && owner.SlingBehaviour.IsValidAiming(pointerWorldPos))
        {
            owner.ChangeState<PlayerAimingState>();
        }
    }
}
