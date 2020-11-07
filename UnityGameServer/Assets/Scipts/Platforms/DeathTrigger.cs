using UnityEngine;

public class DeathTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if(player != null)
        {
            Debug.Log($"Player death: {player.id}");
            player.Die();
        }
        
    }
}
