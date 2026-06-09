using System;
using UnityEngine;
using JoonyleGameDevKit;
using System.Collections;
using System.Collections.Generic;

public sealed class PlayerBehaviour : SlingEntity<EnemyBehaviour>
{
    public CameraController CameraController { get; private set; }
    public IPointerInput PointerInput { get; private set; }

    [SerializeField] private float _hitStopDuration = 0.1f;
    [SerializeField] private float _fallingMoveSpeed = 3f;
    public float FallingMoveSpeed => _fallingMoveSpeed;
    [SerializeField] private float _fallingJoystickRange = 150f;
    public float FallingJoystickRange => _fallingJoystickRange;

    [Space]
    [SerializeField] private float _pendulumLength = 1.5f;
    [SerializeField] private float _pendulumDamping = 3f;
    public float PendulumLength => _pendulumLength;
    public float PendulumDamping => _pendulumDamping;
    public float PendulumAngularVelocity { get; set; }

    [Space]
    [SerializeField] private float _stretchAmount = 1.4f;
    public float StretchAmount => _stretchAmount;

    [Space]

    [SerializeField] private Transform _visual;
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private Transform _handPoint;
    [SerializeField] private Transform _footPoint;

    private StateMachine<PlayerBehaviour> _fsm;

    private bool _isFacingRight;
    private Transform _alignTarget;
    private Vector2 _defaultVisualScale;
    public Vector2 DefaultVisualScale => _defaultVisualScale;

    public void Initialize(CameraController cameraController, IPointerInput pointerInput, Action<int> onDamaged, Action onDead, Action<Peg> onOccupy)
    {
        InitSingleEntity(onDamaged, onDead, onOccupy);

        CameraController = cameraController;
        PointerInput = pointerInput;
        
        _isFacingRight = true;
        _defaultVisualScale = new Vector2(Mathf.Abs(_visual.localScale.x), _visual.localScale.y);

        _fsm = new StateMachine<PlayerBehaviour>(this);

        _fsm.AddState(new PlayerAttachedState());
        _fsm.AddState(new PlayerAimingState());
        _fsm.AddState(new PlayerBoostingState());
        _fsm.AddState(new PlayerFallingState());

        _fsm.ChangeState<PlayerFallingState>();
    }

    public void Tick(float deltaTime) => _fsm.Update(deltaTime);
    public void ChangeState<TState>() where TState : StateBase<PlayerBehaviour> => _fsm.ChangeState<TState>();

    // ============ ... ============

    protected override void OnOccupy(Peg peg)
    {
        base.OnOccupy(peg);

        _fsm.ChangeState<PlayerAttachedState>();
    }

    protected override void OnKill(EnemyBehaviour enemy)
    {
        base.OnKill(enemy);

        if (_fsm.CurrState is PlayerBoostingState)
        {
            StartCoroutine(HitStop());
        }
    }

    // ========= ... =========

    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(_hitStopDuration);
        Time.timeScale = 1f;
    }

    // ========= ... =========

    public void SetVisualScale(float xAbs, float y)
    {
        var scale = _visual.localScale;
        scale.x = xAbs * (_isFacingRight ? 1f : -1f);
        scale.y = y;
        _visual.localScale = scale;
    }

    public void ResetVisualScale() => SetVisualScale(_defaultVisualScale.x, _defaultVisualScale.y);

    // ========= ... =========

    public void SetFacingDirection(float dirX)
    {
        if (Mathf.Approximately(dirX, 0f)) return;

        _isFacingRight = dirX > 0f;

        var scale = _visual.localScale;
        scale.x = Mathf.Abs(scale.x) * (_isFacingRight ? 1f : -1f);
        _visual.localScale = scale;

        if (_alignTarget != null)
            _visual.localPosition = AlignedPos(_alignTarget);
    }

    // ========= ... =========

    private Vector3 AlignedPos(Transform point)
    {
        var pos = point.localPosition;
        pos.x *= _isFacingRight ? 1f : -1f;
        return pos;
    }
    private void ApplyAlignment()
    {
        _visual.localPosition = AlignedPos(_alignTarget);
    }
    public void AlignToCenter()
    {
        _alignTarget = _centerPoint;
        ApplyAlignment();
    }
    public void AlignToHand()
    {
        _alignTarget = _handPoint;
        ApplyAlignment();
    }
    public void AlignToFoot()
    {
        _alignTarget = _footPoint;
        ApplyAlignment();
    }

    // ========= ... =========

    public void SnapToPeg()
    {
        if (OccupyingPeg == null) return;

        Rigid.linearVelocity = Vector2.zero;
        Rigid.position = OccupyingPeg.transform.position;

        OccupyingPeg.TryOccupy(this);

        _fsm.ChangeState<PlayerAttachedState>();

        // _onOccupy?.Invoke(OccupyingPeg);
    }

    protected override void HandleTargetTriggerEnter(Collider2D collider)
    {
        
    }
}
