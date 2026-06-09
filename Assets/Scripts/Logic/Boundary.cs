using UnityEngine;

public class Boundary : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerBehaviour>(out var player))
        {
            player.SnapToPeg();
        }
    }
}
