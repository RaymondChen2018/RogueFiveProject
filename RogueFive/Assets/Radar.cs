using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField] private float radius = 100.0f;
    [SerializeField] private int numDivision = 100;


    // Start is called before the first frame update
    void Start()
    {
        float angle = 0f;
        int numVertices = numDivision + 1;
        Vector2 thisPos = transform.position;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;
        Vector3[] vertices = new Vector3[numVertices];
        for (int i = 0; i < numVertices; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            vertices[i] = thisPoint;

            lastPoint = thisPoint;
            angle += 360f / numDivision;
        }

        LineRenderer lineRender = GetComponent<LineRenderer>();
        lineRender.positionCount = vertices.Length;
        lineRender.SetPositions(vertices);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
