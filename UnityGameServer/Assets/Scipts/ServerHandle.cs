using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected succesfully and is now player {fromClient}");

        if (fromClient != clientIdCheck)
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient} has assumed the wrong client ID ({clientIdCheck})!");
        }
        Server.clients[fromClient].SendIntoGame(username);
    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        long clientTick = packet.ReadLong();
        bool[] inputs = new bool[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        Quaternion rotation = packet.ReadQuaternion();
        float packetClientSendTime = packet.ReadFloat();

        if (Server.clients.TryGetValue(fromClient, out Client client))
        {
            // Add client inputs to worldStateBuffer on client's tick
            if(clientTick >= Server.tick - Server.tickDelay)
            {
                Server.WorldState.Add(new ServerPlayerState(clientTick, client.id, inputs, rotation, packetClientSendTime, Time.time));
            }
            else
            {
                // else drop, packet arrived too late
                Debug.Log($"Dropping packet of client {client.id} for tick {clientTick} server is gonna process {Server.tick - Server.tickDelay}");
            }
        }
    }
}
