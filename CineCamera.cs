using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CineCamera : MonoBehaviour
{
    public static CineCamera instance;
    
    private CinemachineVirtualCamera cineCam;

    private void Start()
    {
        cineCam = this.GetComponent<CinemachineVirtualCamera>();
    }
    private void Awake()
    {
        instance = this;
    }
    public void Follow(Transform _subject)
    {
        cineCam.Follow = _subject;
    }
}
