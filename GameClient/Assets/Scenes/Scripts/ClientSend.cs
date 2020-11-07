using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(packet);
        }
    }

    public static void PlayerMovement(long clientTick, bool[] inputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            packet.Write(clientTick);
            packet.Write(inputs.Length);
            foreach(bool input in inputs)
            {
                packet.Write(input);
            }
            packet.Write(GameManager.players[Client.instance.myId].transform.rotation);
            packet.Write(Time.time);// used to determine RTT

            SendUDPData(packet);
        }
    }

    #endregion
}
