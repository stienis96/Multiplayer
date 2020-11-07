using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    public float jumpForce = 25f;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.rigidbody.gameObject.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            player.MassiveJump(jumpForce);
        }
    }
}
