using UnityEngine;

public class ClientPlayerState
{
    public long tick;
    public bool[] inputs;
    public Vector3 position;
    public float yVelocity;

    public ClientPlayerState(long tick, bool[] inputs, Vector3 position, float yVelocity)
    {
        this.tick = tick;
        this.inputs = inputs;
        this.position = position;
        this.yVelocity = yVelocity;
    }
}


