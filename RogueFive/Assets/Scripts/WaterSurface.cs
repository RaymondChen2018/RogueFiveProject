using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define water line of which player resumes gravity above & enters water physic below
/// </summary>
public class WaterSurface : MonoBehaviour
{
    [Header("Surface Generation")]
    [SerializeField] private LayerMask terrainLayer;
    [Tooltip("If this is 50, scan width will be 100 wide")][SerializeField] private float scanDistance = 100.0f;

    [Header("Debug")]
    [SerializeField] private bool debugOn = true;
    [SerializeField] private Color debugColor = Color.cyan;
    [SerializeField] private Color debugErrorColor = Color.red;

    private float surfaceLevel;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 thisPos = transform.position;

        // Define water surface
        RaycastHit2D hitLeft = Physics2D.Raycast(thisPos, Vector2.left, scanDistance, terrainLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(thisPos, Vector2.right, scanDistance, terrainLayer);
        if(!(hitLeft && hitRight))
        {
            Debug.LogError("Water surface not touching terrain!");
        }

        // Subsurface effect
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        float lineYOffset = lineRenderer.GetPosition(0).y;
        Vector2 lineLeft = new Vector2(hitLeft.point.x - thisPos.x, lineYOffset);
        Vector2 lineRight = new Vector2(hitRight.point.x - thisPos.x, lineYOffset);
        lineRenderer.SetPosition(1, lineLeft);
        lineRenderer.SetPosition(0, lineRight);

        // Create collider
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        Vector2[] newEdgePositions = edgeCollider.points;
        newEdgePositions[0] = hitLeft.point - thisPos;
        newEdgePositions[1] = hitRight.point - thisPos;
        edgeCollider.points = newEdgePositions;

        // set surface level
        surfaceLevel = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(debugOn)
        {
            Vector2 thisPos = transform.position;

            // Draw lhs line
            RaycastHit2D hitLeft = Physics2D.Raycast(thisPos, Vector2.left, scanDistance, terrainLayer);
            if(hitLeft)
            {
                Debug.DrawLine(thisPos, hitLeft.point, debugColor);
            }
            else
            {
                Debug.DrawLine(thisPos, thisPos + Vector2.left * scanDistance, debugErrorColor);
            }

            // Draw rhs line
            RaycastHit2D hitRight = Physics2D.Raycast(thisPos, Vector2.right, scanDistance, terrainLayer);
            if (hitRight)
            {
                Debug.DrawLine(thisPos, hitRight.point, debugColor);
            }
            else
            {
                Debug.DrawLine(thisPos, thisPos + Vector2.right * scanDistance, debugErrorColor);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CircleCollider2D circleCollider = collision.GetComponent<CircleCollider2D>();

        float playerZ = circleCollider.transform.position.y;
        float playerRadius = circleCollider.radius;

        float submergeRatio = (surfaceLevel - (playerZ - playerRadius)) / (playerRadius * 2);
        SubmergePhysics submergePhysic = circleCollider.GetComponent<SubmergePhysics>();
        submergePhysic.submergeAmount(submergeRatio);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        CircleCollider2D circleCollider = collision.GetComponent<CircleCollider2D>();

        float playerZ = circleCollider.transform.position.y;
        SubmergePhysics submergePhysic = circleCollider.GetComponent<SubmergePhysics>();

        // Fully on land
        if (playerZ > surfaceLevel)
        {
            submergePhysic.submergeAmount(0.0f);
        }
        // Fully in water
        else
        {
            submergePhysic.submergeAmount(1.0f);
        }
    }
}
