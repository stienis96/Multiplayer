using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float maxRotationAngle;
    public float xRotation;

    private void Start()
    {
        maxRotationAngle = 25;
    }

    public void FixedUpdate()
    {
        xRotation = Mathf.Sin(Time.time) * maxRotationAngle;
        transform.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        ServerSend.PlatformRotation(transform.rotation);
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        Player collidingPlayer = collisionInfo.gameObject.GetComponentInParent<Player>();
        //Debug.Log($"collidingPlayer: {collidingPlayer}");
        if(collidingPlayer != null)
        {
            Vector3 dir = transform.forward * ((xRotation / maxRotationAngle) * 2);
            Server.clients[collidingPlayer.id].player.slideDirection = dir;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Player collidingPlayer = collision.gameObject.GetComponentInParent<Player>();
        if(collidingPlayer != null)
        {
            Server.clients[collidingPlayer.id].player.slideDirection = Vector3.zero;
        }
    }

}
