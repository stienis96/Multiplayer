
using UnityEngine;

public class ServerWorldState
{
    public long tick;
    public int clientId;
    public bool[] inputs;
    public Vector3 position;
    public Quaternion rotation;
    public float serverYVelocity;

    public ServerWorldState(long tick, int clientId, Vector3 position, Quaternion rotation, float serverYVelocity)
    {
        this.tick = tick;
        this.clientId = clientId;
        this.position = position;
        this.rotation = rotation;
        this.serverYVelocity = serverYVelocity;
    }
}