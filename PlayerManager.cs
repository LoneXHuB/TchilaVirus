using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public Team team;
    public float health;

    public void InitializePlayer(int _id , string _username , Team _team , float _health)
    {
        id = _id;
        username = _username;
        team = _team;
        health = _health;
    }

    public void Die()
    {
        health = 0f;
        Debug.Log("you died");
    }
}
