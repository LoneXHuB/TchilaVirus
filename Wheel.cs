using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public static Dictionary<int, Wheel> wheels = new Dictionary<int, Wheel>();
    public static int nextId = 1;

    public int id;
    private bool isEntered = false;

    public void InitializeWheel()
    {
        id = nextId;
        nextId++;

        wheels.Add(id, this);
    }

    public void SetEntered(bool _isEntered)
    {
        isEntered = _isEntered;
        Debug.Log("Wheel {id} is entered");
    }
}
