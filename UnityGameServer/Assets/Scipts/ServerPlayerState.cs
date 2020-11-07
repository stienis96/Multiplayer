using UnityEngine;

public class ServerPlayerState
{
    public long tick;
    public int clientId;
    public bool[] inputs;
    public Vector3 position;
    public Quaternion rotation;

    public float packetClientSendTime;
    public float serverPacketWriteTime;

    public ServerPlayerState(long tick, int clientId, bool[] inputs, Quaternion rotation, float packetClientSendTime, float serverPacketWriteTime)
    {
        this.tick = tick;
        this.clientId = clientId;
        this.inputs = inputs;
        this.rotation = rotation;
        this.packetClientSendTime = packetClientSendTime;
        this.serverPacketWriteTime = serverPacketWriteTime;
    }
}