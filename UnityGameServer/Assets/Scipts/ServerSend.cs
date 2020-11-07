using UnityEngine;

public class ServerSend
{
    private static void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.clients[toClient].tcp.SendData(packet);
    }

    private static void SendUDPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.clients[toClient].udp.SendData(packet);
    }

    private static void SendTCPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(packet);
        }
    }

    private static void SendTCPDataToAll(int exceptClient, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != exceptClient)
            {
                Server.clients[i].tcp.SendData(packet);
            }
        }
    }

    private static void SendUDPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(packet);
        }
    }

    private static void SendUDPDataToAll(int exceptClient, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != exceptClient)
            {
                Server.clients[i].udp.SendData(packet);
            }
        }
    }

    public static void Welcome(int toClient, string msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.welcome))
        {
            packet.Write(msg);
            packet.Write(toClient);
            packet.Write(Server.tick); // 150ms in ticks 30 ticks per second ==> 5/30 = 166ms

            Debug.Log($"Sending client tick-value {Server.tick} with server tick {Server.tick} - {Server.tick - Server.tickDelay}");
            SendTCPData(toClient, packet);
        }
    }

    public static void SpawnPlayer(int toClient, Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            packet.Write(player.id);
            packet.Write(player.username);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);

            SendTCPData(toClient, packet);
        }
    }

    public static void PlayerPosition(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerPosition))
        {
            packet.Write(player.lastAppliedTick);
            packet.Write(player.id);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);
            packet.Write(player.yVelocity);

            if (player.serverPacketTime == -1f)
            {
                packet.Write(-1f); // Invalidate RTT calculation, because we didn't receive this packet
            }
            else
            {
                packet.Write(player.lastAppliedTickTime - (Time.time - player.serverPacketTime) + TickManager.lag); // Calc time processing on server (and in queue)
            }

            SendUDPDataToAll(packet);
        }
    }

    public static void PlayerRotation(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRotation))
        {
            packet.Write(player.id);
            packet.Write(player.transform.rotation);

            SendUDPDataToAll(player.id, packet);
        }
    }

    public static void PlayerDisconnected(int playerId)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            packet.Write(playerId);

            SendTCPDataToAll(packet);
        }
    }

    public static void SendPlayerDeath(int id)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerDeath))
        {
            packet.Write(id);

            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerRespawned(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRespawned))
        {
            packet.Write(player.id);

            SendTCPDataToAll(packet);
        }
    }

    public static void PlatformRotation(Quaternion rotation)
    {
        using (Packet packet = new Packet((int)ServerPackets.platformRotation))
        {
            packet.Write(rotation);

            SendUDPDataToAll(packet);
        }
    }

    public static void GameFinished(string[] finishedPlayers, string[] diedPlayers)
    {
        using (Packet packet = new Packet((int)ServerPackets.gameFinished))
        {
            packet.Write(finishedPlayers.Length);
            for(int i = 0; i < finishedPlayers.Length; i++)
            {
                packet.Write(finishedPlayers[i]);
            }

            packet.Write(diedPlayers.Length);
            for (int i = 0; i < diedPlayers.Length; i++)
            {
                packet.Write(diedPlayers[i]);
            }

            SendTCPDataToAll(packet);
        }
    }

    public static void SpawnObject(int index, Vector3 position)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnItem))
        {
            packet.Write(index);
            packet.Write(position);

            SendTCPDataToAll(packet);
        }
    }

    public static void ItemPosition(int id, Vector3 position)
    {
        using (Packet packet = new Packet((int)ServerPackets.itemPosition))
        {
            packet.Write(id);
            packet.Write(position);

            SendUDPDataToAll(packet);
        }
    }

    public static void DestroyItem(int id)
    {
        using (Packet packet = new Packet((int)ServerPackets.destroyItem))
        {
            packet.Write(id);

            SendTCPDataToAll(packet);
        }
    }
}
