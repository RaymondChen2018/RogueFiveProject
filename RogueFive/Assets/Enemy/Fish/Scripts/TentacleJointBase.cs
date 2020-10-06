using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TentacleJointBase : MonoBehaviour
{
    [Header("Update Line")]
    [SerializeField] private int lineRendererVertexIndex = 0;
    [SerializeField] private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TentacleJointUpdate();

        Vector2 vertexPosition = transform.position - lineRenderer.transform.position;
        lineRenderer.SetPosition(lineRendererVertexIndex, vertexPosition);
    }

    virtual protected void TentacleJointUpdate() { }

    public void setRenderer(int newIndex, LineRenderer renderer)
    {
        lineRenderer = renderer;
        lineRendererVertexIndex = newIndex;
    }
}
