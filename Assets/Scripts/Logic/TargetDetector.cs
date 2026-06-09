using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;

    private ITargetHitHandler _handler;

    private void Awake()
    {
        _handler = GetComponent<ITargetHitHandler>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_targetLayer.value & (1 << collision.gameObject.layer)) == 0) return;

        _handler?.OnTargetCollisionHit();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if ((_targetLayer.value & (1 << collider.gameObject.layer)) == 0) return;

        _handler?.OnTargetTriggerHit();
    }
}
