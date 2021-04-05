using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusTrail : MonoBehaviour
{
    [SerializeField]
    public Transform[] points;
    public float MaxFadeTime;
    private float fadeTime;

    private void Awake()
    {
        fadeTime = MaxFadeTime;
    }
    private void Update()
    {
        if(fadeTime <= 0.0f)
        {
            Destroy(this.gameObject);
        }
        fadeTime -= Time.deltaTime;
    }
    public void SetPointsPositions(Transform[] _points)
    {
       for(int i=0; i < points.Length ; i++)
        {
            points[i].position = _points[i].position;
        }
    }
}
