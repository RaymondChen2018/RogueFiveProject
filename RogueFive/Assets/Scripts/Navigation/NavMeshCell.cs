using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshCell : MonoBehaviour
{
    [SerializeField] private int dimension = NavMeshManager.DEFAULT_CELL_DIMENSION;

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

    public int GetCurrDimension()
    {
        return dimension;
    }

    public void SetDimension(int _dimension)
    {
        Vector2 scaleVec;
        if (_dimension >= NavMeshManager.DEFAULT_CELL_DIMENSION)
        {
            scaleVec.x = _dimension / NavMeshManager.DEFAULT_CELL_DIMENSION;
            scaleVec.y = _dimension / NavMeshManager.DEFAULT_CELL_DIMENSION;
        }
        else
        {
            scaleVec.x = 1.0f / (NavMeshManager.DEFAULT_CELL_DIMENSION / _dimension);
            scaleVec.y = 1.0f / (NavMeshManager.DEFAULT_CELL_DIMENSION / _dimension);
        }
        transform.localScale = scaleVec;
        dimension = _dimension;
    }
}
