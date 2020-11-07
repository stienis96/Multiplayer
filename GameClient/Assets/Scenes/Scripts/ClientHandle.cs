using UnityEngine;
using System.Net;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myId = packet.ReadInt();
        long serverTick = packet.ReadLong();

        Debug.Log($"Message from server: {msg}");
        Client.instance.myId = myId;
        ClientSend.WelcomeReceived();
        const float LOW_RTT = 0.1f;
        AdjustTickForServer(serverTick, 0f, LOW_RTT);
        Debug.Log($"Server tick {serverTick} and client has now been updated to {GameManager.tick}");

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerPosition(Packet packet)
    {
        long serverTickReceive = packet.ReadLong();
        int playerId = packet.ReadInt();
        Vector3 serverAuthorizedPosition = packet.ReadVector3();
        Quaternion serverAuthorizedRotation = packet.ReadQuaternion();
        float serverYVelocity = packet.ReadFloat();

        float packetTime = packet.ReadFloat();
        float packetRTT = Time.time - packetTime;
        AdjustTickForServer(serverTickReceive, packetTime, packetRTT);

        PlayerController.serverStateBuffer.Add(new ServerWorldState(serverTickReceive, playerId, serverAuthorizedPosition, serverAuthorizedRotation, serverYVelocity));
    }

    private static void AdjustTickForServer(long serverTickReceive, float packetTime, float packetRTT)
    {
        // TODO get back RTT monitoring to determine how many ticks client should be ahead
        int MaxAheadTicks = 4;
        if (GameManager.tick == 0)
        {
            GameManager.tick = serverTickReceive + MaxAheadTicks;
        }
        if (GameManager.tick <= serverTickReceive + MaxAheadTicks)
        {
            Debug.Log("Skipping a tick");
            GameManager.tick++;
        }
    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();
        Debug.Log("PlayerRotation remove this method");
        GameManager.players[id].transform.rotation = rotation;
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();
        RemovePlayer(id);
    }

    public static void PlayerDeath(Packet packet)
    {
        int id = packet.ReadInt();
        Debug.Log("PLayer death packet received!");

        GameManager.players[id].Die(id);
    }

    private static void RemovePlayer(int id)
    {
        Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);
    }

    public static void PlayerRespawned(Packet packet)
    {
        int id = packet.ReadInt();

        GameManager.players[id].Respawn(id);
    }

    public static void PlatformRotation(Packet packet)
    {
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.instance.rotatingPlatform.GetComponent<RotatingPlatform>().SetRotation(rotation);
    }

    public static void GameFinished(Packet packet)
    {
        int amountOfFinishedPlayers = packet.ReadInt();
        string[] finishedPlayerNames = new string[amountOfFinishedPlayers];
        for(int i = 0; i < amountOfFinishedPlayers; i++)
        {
            finishedPlayerNames[i] = packet.ReadString();
        }

        int amountOfDiedPlayers = packet.ReadInt();
        string[] diedPlayerNames = new string[amountOfDiedPlayers];
        for (int i = 0; i < amountOfDiedPlayers; i++)
        {
            diedPlayerNames[i] = packet.ReadString();
        }
        

        GameObject.Find("Menu").GetComponent<UIManager>().OnGameFinished(finishedPlayerNames, diedPlayerNames);
    }

    public static void SpawnItem(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        GameObject.Find("SpawnManager").GetComponent<ItemManager>().Spawn(id, position);
    }

    public static void ItemPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        GameObject.Find("SpawnManager").GetComponent<ItemManager>().UpdatePosition(id, position);
    }

    public static void DestroyItem(Packet packet)
    {
        int id = packet.ReadInt();
        GameObject.Find("SpawnManager").GetComponent<ItemManager>().DestroyItem(id);
    }
}
