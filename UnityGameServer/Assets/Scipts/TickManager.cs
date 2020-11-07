using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    private void FixedUpdate()
    {
        ApplyUpdates();
        SendNewStateToClients();
        
        Server.tick++;
    }

    private void SendNewStateToClients()
    {
        foreach(Client client in Server.clients.Values)
        {
            // Not null
            if (client.player != null) // Always send a server state to every client
            {
                client.lastSentTick = client.player.lastAppliedTick;
                ServerSend.PlayerPosition(client.player);
                Debug.Log($"Send tick {client.lastSentTick} at pos {client.player.transform.position}");
            }
        }
    }

    private void ApplyUpdates()
    {
        List<ServerPlayerState> currentTickStates = Server.WorldState.FindAll(state =>  state.tick == (Server.tick - Server.tickDelay) );
        
        for(int i = 1; i < Server.clients.Count; i++)
        {
            Client client = Server.clients[i];
            if(client != null)
            {
                Player player = client.player;
                if(player != null) // Not disconnected
                {
                    Debug.Log($"tick {Server.tick} and delayedTick {Server.tick - Server.tickDelay}");

                    ServerPlayerState stateForCurrentTick = currentTickStates.Find(state => state.clientId == client.id);
                    if(stateForCurrentTick == null) // Client didn't send a packet for this tick (in time)
                    {
                        // EMPTY State, assume no action has been taken 
                        ServerPlayerState emptyState = new ServerPlayerState(Server.tick - Server.tickDelay, client.id, new bool[] { false, false, false, false, false, }, player.transform.rotation, 0f, -1f);
                        Server.clients[emptyState.clientId].player.SetInput(emptyState.inputs, emptyState.rotation, emptyState.tick, emptyState.packetClientSendTime, emptyState.serverPacketWriteTime);
                        //FIXME Update clients tick, if this happens more often?
                    }
                    else
                    {
                        // Apply this state
                        player.SetInput(stateForCurrentTick.inputs, stateForCurrentTick.rotation, stateForCurrentTick.tick, stateForCurrentTick.packetClientSendTime, stateForCurrentTick.serverPacketWriteTime);
                    }
                }
            }
        }
        Server.WorldState.RemoveAll(state => state.tick == (Server.tick - Server.tickDelay)); // Remove when inputs applied (fixme can still states be added while running the loop above?)
    }
}
