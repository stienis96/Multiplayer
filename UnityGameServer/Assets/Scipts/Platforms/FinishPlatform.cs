using UnityEngine;

public class FinishPlatform : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.rigidbody.gameObject.GetComponentInParent<Player>();
        Debug.Log($"Player finished: {player.username}");
        RoundManager.AddToFinishedPlayers(player);
        // Message client to show a screen + flying camera or smth
        player.Finish();
    }
}
