using UnityEngine;

public class BoundaryDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _boundaryLayer;

    private ITargetHitHandler _handler;

    private void Awake()
    {
        _handler = GetComponent<ITargetHitHandler>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_boundaryLayer.value & (1 << collision.gameObject.layer)) == 0) return;

        _handler?.OnTargetCollisionHit();
    }
}
