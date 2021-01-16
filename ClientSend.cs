using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }
    #region
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            Debug.Log("welcome received packet sent to server!");
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.userNameField.text);
            _packet.Write((int) UIManager.instance.teamDropdown.value);

            SendTCPData(_packet);
        }
    }
    
    public static void PlayerMovement(Vector2 _movement)
    {
        using (Packet _packet = new Packet((int)ClientPackets.PlayerMovement))
        {
            _packet.Write(_movement);

            SendUDPData(_packet);
        }
    }
    #endregion
}
