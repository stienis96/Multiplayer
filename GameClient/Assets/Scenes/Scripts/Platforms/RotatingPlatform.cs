using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float maxRotationAngle;

    public void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
    // FIXME some good sliding, instead of jumping 
    private void Start()
    {
        maxRotationAngle = 25;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        PlayerManager collidingPlayer = collisionInfo.gameObject.GetComponentInParent<PlayerManager>();
        if (collidingPlayer != null)
        {
            Vector3 dir = transform.forward * ((transform.rotation.eulerAngles.x / maxRotationAngle) * 2);
            collidingPlayer.GetComponent<PlayerController>().slideDirection = dir;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        PlayerManager collidingPlayer = collision.gameObject.GetComponentInParent<PlayerManager>();
        if (collidingPlayer != null)
        {
            collidingPlayer.GetComponent<PlayerController>().slideDirection = Vector3.zero;
        }
    }
}
