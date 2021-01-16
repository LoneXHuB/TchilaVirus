using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject virusP;
    public GameObject virusLP;
    public GameObject whiteCellP;
    public GameObject whiteCellLP;
    public GameObject wheel;
    public GameObject world;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists ! ");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int _id , string _username , Vector2 _position , Quaternion _rotation, int _team , float _health)
    {
        GameObject _player = null;
        Debug.Log($"player team = {_team}");
        Debug.Log($"virus == {(int)Team.Virus}");
        Debug.Log($"White == {(int)Team.White}");

        if (_id == Client.instance.myId)
        {
            if (_team == (int)Team.Virus)
                _player = Instantiate(virusLP, _position, _rotation);
            else if (_team == (int)Team.White)
                _player = Instantiate(whiteCellLP, _position, _rotation);
            else
            {
                Debug.Log("Somthing horrible happened ( unexisting team assinged in GameManager.SpawPlayer())");
                return;
            }

        }
        else
        {
            if (_team == (int)Team.Virus)
                _player = Instantiate(virusP, _position, _rotation);
            else if (_team == (int)Team.White)
                _player = Instantiate(whiteCellP, _position, _rotation);
            else
            {
                Debug.Log("Somthing horrible happened ( unexisting team assinged in GameManager.SpawPlayer())");
                return;
            }
        }

        _player.GetComponent<PlayerManager>().InitializePlayer(_id, _username, (Team)_team, _health);

        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void SpawnWheel(int _id , Vector2 _position , Quaternion _rotation)
    {
        GameObject _wheel = Instantiate(wheel, _position, _rotation);
        Debug.Log($"Wheel {_id} spawned!");
        _wheel.gameObject.transform.parent = world.transform;

    }
}
