using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public sealed class Peg : MonoBehaviour
{
    [SerializeField] private float _neighborRange = 5f;
    [SerializeField] private List<Peg> _neighbors = new();
    public IReadOnlyList<Peg> Neighbors => _neighbors;

    private IPegOccupant _currIPegOccupant;
    public IPegOccupant CurrIPegOccupant => _currIPegOccupant;
    public bool IsOccupied => _currIPegOccupant != null;

    // ============ 점유 ============

    public bool TryOccupy(IPegOccupant slingEntity)
    {
        if (IsOccupied) return false;

        _currIPegOccupant = slingEntity;

        return true;
    }

    public bool TryVacate(IPegOccupant slingEntity)
    {
        if (_currIPegOccupant != slingEntity) return false;

        _currIPegOccupant = null;

        return true;
    }

    // ============ Gizmo ============

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsOccupied ? Color.red : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.15f);

        if (_neighbors == null) return;

        Gizmos.color = Color.yellow;
        foreach (var neighbor in _neighbors)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }

        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, _neighborRange);

        var sqrNeighborRange = _neighborRange * _neighborRange;
        var allPegs = FindObjectsByType<Peg>(FindObjectsSortMode.None);
        foreach (var otherPeg in allPegs)
        {
            if (otherPeg == this) continue;
            var start = (Vector3)transform.position;
            var end = (Vector3)otherPeg.transform.position;
            if ((start - end).sqrMagnitude > sqrNeighborRange) continue;

            var dir = end - start;
            var perp = new Vector3(-dir.y, dir.x, 0f).normalized * (dir.magnitude * 0.25f);
            UnityEditor.Handles.DrawBezier(start, end, start + perp, end + perp, Color.green, null, 1.5f);
        }
    }
#endif
}
