using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static class ExtensionMethods
{
    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static int LayerMaskToLayer(this LayerMask layerMask)
    {
        return Mathf.Clamp((int)Mathf.Log((int)layerMask, 2), 0, 31);
    }

    public static Mesh copyMesh(this Mesh mesh)
    {
        Mesh newMesh = new Mesh();

        // Copy vertices
        Vector3[] vertices = new Vector3[mesh.vertices.Length];
        for(int i=0;i< mesh.vertices.Length; i++)
        {
            vertices[i] = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z);
        }

        // Copy triangle
        int[] indices = new int[mesh.triangles.Length];
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            indices[i] = mesh.triangles[i];
        }

        newMesh.vertices = vertices;
        newMesh.triangles = indices;
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        return newMesh;
    }

    public static float readAlpha(this Gradient gradient, float time)
    {
        // before first key
        if(time < gradient.alphaKeys[0].time)
        {
            return gradient.alphaKeys[0].alpha;
        }
        // after first key
        else if (time > gradient.alphaKeys[gradient.alphaKeys.Length - 1].time)
        {
            return gradient.alphaKeys[gradient.alphaKeys.Length - 1].alpha;
        }

        for (int i = 0; i < gradient.alphaKeys.Length - 1; i++)
        {
            GradientAlphaKey keyCurr = gradient.alphaKeys[i];
            GradientAlphaKey keyNext = gradient.alphaKeys[i + 1];
            if (time <= keyNext.time)
            {
                return Mathf.Lerp(keyCurr.alpha, keyNext.alpha, (time - keyCurr.time) / (keyNext.time - keyCurr.time));
            }
        }

        return 1.0f;
    }

    public static Mesh polyCollider2DToMesh(this PolygonCollider2D polyCollider)
    {
        Mesh newMesh;
        newMesh = new Mesh();
        newMesh.name = "Convert_Poly_Mesh";

        Assert.IsTrue(polyCollider.transform.lossyScale == Vector3.one);
        Assert.IsTrue(polyCollider.transform.rotation == Quaternion.identity);
        Vector2 positionOffset = polyCollider.transform.position;
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
        newMesh.vertices = vertices;
        newMesh.triangles = indices;
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        return newMesh;
    }

    public static void keepOff(this ParticleSystem particleSystem)
    {
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.enabled = false;
    }

    public static void keepOn(this ParticleSystem particleSystem)
    {
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.enabled = true;
    }
}

public class Triangulator
{
    private List<Vector2> m_points = new List<Vector2>();

    public Triangulator(Vector2[] points)
    {
        m_points = new List<Vector2>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}