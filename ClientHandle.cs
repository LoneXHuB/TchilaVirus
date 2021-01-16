using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _id = _packet.ReadInt();

        Debug.Log($"Message from server : {_msg}");
        Client.instance.myId = _id;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector2 _position = _packet.ReadVector2();
        Quaternion _rotation = _packet.ReadQuaternion();
        int _team = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation , _team , _health);
    }

    public static void PlayerMovement(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector2 _newPosition = _packet.ReadVector2();
        Quaternion _newRotation = _packet.ReadQuaternion();

        GameManager.players[_id].transform.position = _newPosition;
        GameManager.players[_id].transform.rotation = _newRotation;

        //Debug.Log($"server request : move player id : {_id} to position : {_newPosition.x} , {_newPosition.y}");
    }

    public static void DisconnectPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        PlayerManager _player = GameManager.players[_id];

        Debug.Log($"{_player.username} Disconnected !");

        Destroy(_player.gameObject);
        GameManager.players.Remove(_id);
    }

    public static void KillPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.players[_id].Die();
    }

    public static void SpawnWheel(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector2 _position = _packet.ReadVector2();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnWheel(_id, _position , _rotation);
    }

    public static void WheelEntered(Packet _packet)
    {

    }
}
