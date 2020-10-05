using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDraw : MonoBehaviour {

    //[SerializeField] private Color debugColor = Color.blue;
    //[SerializeField] private Color debugColor2 = Color.red;
    //[SerializeField] private float size = 5.0f;
    //[SerializeField] private Transform origin;
    //[SerializeField] private Transform target;
    //[SerializeField] private LayerMask mask;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //private void OnDrawGizmos()
    //{
    //    if(origin == null || target == null)
    //    {
    //        return;
    //    }

    //    Vector2 direction = target.position - origin.position;
    //    RaycastHit2D hit = Physics2D.CircleCast(origin.position, size, direction, direction.magnitude, mask);
    //    if (hit)
    //    {
    //        DrawCircleCast(origin.position, size, direction, direction.magnitude, debugColor);
    //    }
    //    else
    //    {
    //        DrawCircleCast(origin.position, size, direction, direction.magnitude, debugColor2);
    //    }
        
    //}

    public static void DrawCircleCast(Vector2 origin, Vector2 target, float radius, Color color, float duration = 0.0f)
    {
        Vector2 direction = target - origin;
        DrawEllipse(origin, radius, color, duration);
        DrawEllipse(target, radius, color, duration);
        Vector2 rightTangent = (Vector2)(Quaternion.AngleAxis(90, Vector3.forward) * direction).normalized * radius;
        Vector2 leftTangent = (Vector2)(Quaternion.AngleAxis(-90, Vector3.forward) * direction).normalized * radius;
        Vector2 rightTangentO = rightTangent + origin;
        Vector2 leftTangentO = leftTangent + origin;
        Vector2 rightTangentT = rightTangent + target;
        Vector2 leftTangentT = leftTangent + target;

        Debug.DrawLine(rightTangentO, rightTangentT, color, duration);
        Debug.DrawLine(leftTangentO, leftTangentT, color, duration);
    }
    public static void DrawCircleCast(Vector2 origin, float radius, Vector2 direction, float distance, Color color, float duration = 0.0f)
    {
        Vector2 targetLocation = origin + direction.normalized * distance;
        DrawCircleCast(origin, targetLocation, radius, color, duration);

        //DrawEllipse(origin, radius, color);
        //DrawEllipse(targetLocation, radius, color);
        //Vector2 rightTangent = (Vector2)(Quaternion.AngleAxis(90, Vector3.forward) * direction).normalized * radius;
        //Vector2 leftTangent = (Vector2)(Quaternion.AngleAxis(-90, Vector3.forward) * direction).normalized * radius;
        //Vector2 rightTangentO = rightTangent + origin;
        //Vector2 leftTangentO = leftTangent + origin;
        //Vector2 rightTangentT = rightTangent + targetLocation;
        //Vector2 leftTangentT = leftTangent + targetLocation;

        //Debug.DrawLine(rightTangentO, rightTangentT, color);
        //Debug.DrawLine(leftTangentO, leftTangentT, color);
    }

    public static void DrawEllipse(Vector3 pos, float radius, Color color, float duration = 0.0f)
    {
        int segments = 32;
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            if (i > 0)
            {
                Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }

    /// <summary>
    /// Draw an arc facing targetPos in coneAngle
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="radius"></param>
    /// <param name="targetPos">Target the arc is facing toward</param>
    /// <param name="coneAngle"></param>
    public static void DrawArc(Vector3 pos, float radius, Vector3 targetPos, float coneAngle, Color color, float duration = 0.0f)
    {
        int segments = 32;
        Vector3 targetDirection = targetPos - pos;
        float targetDirectionAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * 180.0f / 3.14f;
        float angle = targetDirectionAngle - coneAngle / 2.0f;

        Quaternion rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            thisPoint.y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            if (i > 0)
            {
                Debug.DrawLine(lastPoint + pos, thisPoint + pos, color, duration);
            }

            lastPoint = thisPoint;
            angle += coneAngle / segments;
        }
    }

    public static void DrawArrow(Vector2 fromPos, Vector2 toPos, float finAngle, float finLength, Color color)
    {
        Debug.DrawLine(fromPos, toPos, color);
        Vector2 negDirection = (fromPos - toPos).normalized;
        Vector2 fin1Pos = (Vector2)(Quaternion.AngleAxis(finAngle, Vector3.forward) * (negDirection) * finLength) + toPos;
        Vector2 fin2Pos = (Vector2)(Quaternion.AngleAxis(-finAngle, Vector3.forward) * (negDirection) * finLength) + toPos;
        Debug.DrawLine(toPos, fin1Pos, color);
        Debug.DrawLine(toPos, fin2Pos, color);
    }
    public static void DrawPolygon(PolygonCollider2D polyCollider, Color color)
    {
        Vector2 thisPos = polyCollider.transform.position;

        for (int i = 0; i < polyCollider.points.Length - 1; i++)
        {
            Debug.DrawLine(polyCollider.points[i] + thisPos, polyCollider.points[i + 1] + thisPos, color);
        }
        Debug.DrawLine(polyCollider.points[0] + thisPos, polyCollider.points[polyCollider.points.Length - 1] + thisPos, color);
    }

    public static void DrawGradientLine(Vector2 from, Vector2 to, Color colorFrom, Color colorTo, float duration = 0.0f)
    {
        int segments = 10;
        float ratio = 1.0f / segments;
        for(int i=0;i<segments - 1;i++)
        {
            float ratioFrom = i * ratio;
            float ratioTo = (i + 1) * ratio;
            Vector2 fromPos = Vector2.Lerp(from, to, ratioFrom);
            Vector2 toPos = Vector2.Lerp(from, to, ratioTo);

            Color lerpColor = Color.Lerp(colorFrom, colorTo, ratioFrom);

            Debug.DrawLine(fromPos, toPos, lerpColor, duration);
        }
    }
}
