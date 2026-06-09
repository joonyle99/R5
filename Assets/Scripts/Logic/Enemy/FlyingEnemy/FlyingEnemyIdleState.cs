using UnityEngine;
using JoonyleGameDevKit;

public sealed class FlyingEnemyIdleState : StateBase<EnemyBehaviour>
{
    public override void Enter(EnemyBehaviour owner)
    {
        owner.PlayAnimation("Fly");
    }

    public override void Exit(EnemyBehaviour owner) { }

    public override void Update(EnemyBehaviour owner) { }
}
