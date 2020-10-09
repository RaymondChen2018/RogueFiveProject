using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ColliderToMesh : MonoBehaviour
{
    PolygonCollider2D polyCollider;
    MeshFilter roomMeshFilter;
    Mesh polyMesh;
    // Start is called before the first frame update
    void Start()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        roomMeshFilter = GetComponent<MeshFilter>();
        polyMesh = new Mesh();
        polyMesh.name = "Poly mesh";
        roomMeshFilter.mesh = polyMesh;

        Assert.IsTrue(transform.lossyScale == Vector3.one);
        Assert.IsTrue(transform.rotation == Quaternion.identity);
        Vector2 positionOffset = transform.position;
        Vector2[] vertices2D = polyCollider.GetPath(0);


        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        polyMesh.vertices = vertices;
        polyMesh.triangles = indices;
        polyMesh.RecalculateNormals();
        polyMesh.RecalculateBounds();
    }
}


