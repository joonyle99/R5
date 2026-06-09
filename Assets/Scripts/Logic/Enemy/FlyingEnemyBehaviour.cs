using UnityEngine;
using JoonyleGameDevKit;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class FlyingEnemyBehaviour : EnemyBehaviour
{
    private StateMachine<EnemyBehaviour> _fsm;

    protected override void OnInitialize()
    {
        _fsm = new StateMachine<EnemyBehaviour>(this);
        _fsm.AddState(new FlyingEnemyIdleState());
        _fsm.ChangeState<FlyingEnemyIdleState>();
    }

    public override void Tick(float deltaTime) => _fsm?.Update(deltaTime);
}
