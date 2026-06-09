using JoonyleGameDevKit;

public sealed class PlayerBoostingState : StateBase<PlayerBehaviour>
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

        if (owner.Rigid.linearVelocity.y <= 0f)
            owner.ChangeState<PlayerFallingState>();
    }
}
