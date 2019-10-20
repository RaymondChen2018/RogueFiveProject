using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshCell : MonoBehaviour
{
    [SerializeField] private int dimension = 64;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool IsContained(float top, float bottom, float left, float right)
    {
        float _x = gameObject.transform.position.x;
        float _y = gameObject.transform.position.y;
        float _dimWorld = dimension / 100.0f;
        return (_x + _dimWorld / 2) > left
            && (_x - _dimWorld / 2) < right
            && (_y + _dimWorld / 2) > bottom
            && (_y - _dimWorld / 2) < top;
    }
}
