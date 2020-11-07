using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    public float jumpForce = 25f;

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.rigidbody.gameObject.GetComponentInParent<Player>();
        if(player != null)
        {
            player.MassiveJump(jumpForce);
        }
    }
}
