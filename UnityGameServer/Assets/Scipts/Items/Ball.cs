using UnityEngine;

public class Ball : MonoBehaviour
{
    private int id;

    private const int WorldEnd = -10;

    public void Init(int index)
    {
        id = index;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Player collidingPlayer = collision.gameObject.GetComponentInParent<Player>();
        if(collidingPlayer != null)
        {
            collidingPlayer.Die();
        }
    }

    private void FixedUpdate()
    {
        
        if (transform.position.y <= WorldEnd)
        {
            DestroyItem();
        }
        ServerSend.ItemPosition(id, transform.position);
    }

    private void DestroyItem()
    {
        ServerSend.DestroyItem(id);
        Spawner.Items.Remove(id);
        Destroy(gameObject);
    }
}
