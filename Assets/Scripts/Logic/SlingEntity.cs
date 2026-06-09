using System;
using UnityEngine;

public abstract class SlingEntity<TTarget> : SlingEntity where TTarget : CombatEntity
{
    protected override void HandleTargetTriggerEnter(Collider2D collider)
    {
        if (collider.TryGetComponent<TTarget>(out var target))
        {
            HandleTargetTriggerEnter(target);
        }
    }

    private void HandleTargetTriggerEnter(TTarget target)
    {
        if (!target.IsDead)
        {
            target.TakeDamage(attack);
            OnHit(target);
            if (target.IsDead)
                OnKill(target);
        }
    }

    protected virtual void OnHit(TTarget target) { }
    protected virtual void OnKill(TTarget target) { }
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SlingBehaviour))]
public abstract class SlingEntity : CombatEntity, IPegOccupant
{
    public Rigidbody2D Rigid { get; protected set; }
    public SlingBehaviour SlingBehaviour { get; protected set; }

    public Peg OccupyingPeg { get; protected set; }

    private bool _isSnapping;

    private Action<Peg> _onOccupy;

    protected void InitSingleEntity(Action<int> onDamaged, Action onDead, Action<Peg> onOccupy)
    {
        InitCombatEntity(onDamaged, onDead);

        _onOccupy = onOccupy;

        Rigid = GetComponentInChildren<Rigidbody2D>();
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

    protected override void OnDamaged(int damage)
    {
        base.OnDamaged(damage);
    }

    protected override void OnDead()
    {
        base.OnDead();

        OccupyingPeg?.TryVacate(this);
    }

    // ============ ... ============

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Peg>(out var peg))
        {
            HandlePegTriggerEnter(peg);
        }
        else
        {
            HandleTargetTriggerEnter(collider);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Peg>(out var peg))
        {
            HandlePegTriggerStay(peg);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Peg>(out var peg))
        {
            HandlePegTriggerExit(peg);
        }
    }

    protected virtual void HandleTargetTriggerEnter(Collider2D collider) { }

    // ========= ... =========

    private void HandlePegTriggerEnter(Peg peg)
    {
        if (peg.TryOccupy(this) && peg != OccupyingPeg)
        {
            Occupy(peg);
        }
    }

    // ========= ... =========

    private void HandlePegTriggerStay(Peg peg)
    {
        if (peg == OccupyingPeg && _isSnapping)
        {
            var dist = SlingBehaviour.Rigid.position - (Vector2)peg.transform.position;
            var sqrDist = dist.sqrMagnitude;
            if (sqrDist < 0.01f)
            {
                _isSnapping = false;
                return;
            }

            SlingBehaviour.Magnet(peg.transform.position);
        }
    }

    // ========= ... =========

    private void HandlePegTriggerExit(Peg peg)
    {
        if (peg.TryVacate(this) && peg == OccupyingPeg)
        {
            OccupyingPeg = null;
        }
    }
}
