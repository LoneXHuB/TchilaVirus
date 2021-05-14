using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SoftBody : MonoBehaviour
{
    [SerializeField]
    public SpriteShapeController spriteShape;
    [SerializeField]
    public Transform[] points;
    [SerializeField]
    public RectTransform internals;

    private float shrinkDenumerator;
    private float initialPoles;

    private void Awake()
    {   
        float _v0 = (points[6].position - points[0].position).magnitude;
        float _v1 = (points[6].position - points[1].position).magnitude;
        float _v2 = (points[6].position - points[2].position).magnitude;
        float _v3 = (points[6].position - points[3].position).magnitude;
        float _v4 = (points[6].position - points[4].position).magnitude;
        float _v5 = (points[6].position - points[5].position).magnitude;
       
        shrinkDenumerator = Mathf.Min(_v0,_v1,_v2,_v3,_v4,_v5);
        
        UpdateVerticies();
    }

    private void Update()
    {
        UpdateVerticies();
    }

    private void UpdateVerticies()
    {
        //iterate all points except the center one
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 _centerPoint = Vector2.zero;
            Vector2 _vertex = points[i].localPosition;

            Vector2 _towardsCenter = (_centerPoint - _vertex).normalized;

            Quaternion _newAngle = points[i].rotation;

            float _colliderRadius = points[i].gameObject.GetComponent<CircleCollider2D>().radius;

            try
            {
                spriteShape.spline.SetPosition(i, (_vertex - _towardsCenter * _colliderRadius ));
            }
            catch
            {
                //Debug.Log("sline points too close recalculating...");
                spriteShape.spline.SetPosition(i, (_vertex - _towardsCenter * (_colliderRadius + 0.5f)));
            }
            

            //Here I get the tangents of the point and rotate them

            Vector2 _lt = spriteShape.spline.GetLeftTangent(i);

            Vector2 _newRt = Vector2.Perpendicular(_towardsCenter) * _lt.magnitude;
            Vector2 _newLt = -(_newRt);

            spriteShape.spline.SetRightTangent(i, _newRt);
            spriteShape.spline.SetLeftTangent(i, _newLt);
        }
        //Ajust the internals so that they don't go out of the body
        //TODO: Refactor this !
        float _v0 = (points[6].position - points[0].position).magnitude;
        float _v1 = (points[6].position - points[1].position).magnitude;
        float _v2 = (points[6].position - points[2].position).magnitude;
        float _v3 = (points[6].position - points[3].position).magnitude;
        float _v4 = (points[6].position - points[4].position).magnitude;
        float _v5 = (points[6].position - points[5].position).magnitude;

        float _shrinkEnumerator = Mathf.Min(_v0,_v1,_v2,_v3,_v4,_v5);
        
        float _shrinkFactor = _shrinkEnumerator / shrinkDenumerator;

        if(_shrinkFactor < 1)
            internals.localScale = new Vector3(_shrinkFactor , _shrinkFactor);

    }

}
