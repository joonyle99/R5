using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SlingBehaviour))]
public abstract class SlingEnemyBehaviour : EnemyBehaviour, IPegOccupant
{
    public Rigidbody2D Rigid { get; private set; }
    public Animator Animator { get; private set; }
    public SlingBehaviour SlingBehaviour { get; private set; }

    public Peg OccupyingPeg { get; private set; }

    private bool _isSnapping;
    private Action<Peg> _onOccupy;

    protected void InitSlingEnemy(Action<int> onDamaged, Action onDead, Action<Peg> onOccupy)
    {
        Initialize(onDamaged, onDead);

        _onOccupy = onOccupy;

        Rigid = GetComponentInChildren<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();
        SlingBehaviour = GetComponentInChildren<SlingBehaviour>();
        SlingBehaviour.Initialize(Rigid);
        SlingBehaviour.SetActiveSling(true);
    }

    // ========= ... =========

    private void Occupy(Peg peg)
    {
        if (peg == OccupyingPeg) return;

        _isSnapping = true;
        OccupyingPeg = peg;
        OnOccupy(peg);
    }

    protected virtual void OnOccupy(Peg peg)
    {
        _onOccupy?.Invoke(peg);
    }

    // ========= ... =========

    protected override void OnDead()
    {
        base.OnDead();

        OccupyingPeg?.TryVacate(this);
    }

    // ============ ... ============

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Peg>(out var peg))
            HandlePegTriggerEnter(peg);
        else
            HandleTargetTrigger(collider);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Peg>(out var peg))
            HandlePegTriggerStay(peg);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Peg>(out var peg))
            HandlePegTriggerExit(peg);
    }

    protected virtual void HandleTargetTrigger(Collider2D collider) { }

    // ========= ... =========

    private void HandlePegTriggerEnter(Peg peg)
    {
        if (peg.TryOccupy(this) && peg != OccupyingPeg)
            Occupy(peg);
    }

    private void HandlePegTriggerStay(Peg peg)
    {
        if (peg == OccupyingPeg && _isSnapping)
        {
            var dist = SlingBehaviour.Rigid.position - (Vector2)peg.transform.position;
            if (dist.sqrMagnitude < 0.01f)
            {
                _isSnapping = false;
                return;
            }

            SlingBehaviour.Magnet(peg.transform.position);
        }
    }

    private void HandlePegTriggerExit(Peg peg)
    {
        if (peg.TryVacate(this) && peg == OccupyingPeg)
            OccupyingPeg = null;
    }
}
