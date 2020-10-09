using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    enum aquaMode
    {
        intoWater,
        outtaWater,
        belowWater,
        aboveWater
    }
    [SerializeField] private float maxDistance = 100.0f;
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask waterSurfaceMask;
    [Tooltip("Layers laser can hit")][SerializeField] private LayerMask obstacleMask;
    [SerializeField] LineRenderer lineRenderer;

    [Header("Particle")]
    [SerializeField] private GameObject prefabLaserBubbleParticle;
    private ParticleSystem laserParticle;

    [Header("Debug")]
    [SerializeField] private bool debugOn = true;
    [SerializeField] private Color debugColor_laser = Color.green;

    [SerializeField] aquaMode state = aquaMode.aboveWater;
    private bool touchedWater = false;
    private float castWaterY;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        GameObject laserBubble = Instantiate(prefabLaserBubbleParticle);
        laserBubble.transform.parent = transform;
        laserBubble.transform.localPosition = Vector3.zero;
        laserParticle = laserBubble.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            Debug.LogWarning("this laser has no target");
            return;
        }

        LayerMask collisionMask = waterSurfaceMask | obstacleMask;
        Vector2 thisPos = transform.position;
        Vector2 targetPos = target.position;
        Vector2 directionDelta = targetPos - thisPos;
        Vector2 direction = directionDelta.normalized;

        float laserDistanceCast = Mathf.Clamp(maxDistance, 0.0f, directionDelta.magnitude);
        

        bool hitWater = false;
        Vector2 waterCollisionPos = Vector3.zero;
        bool hitObstacle = false;
        Vector2 obstaclePos = Vector3.zero;
        int waterSurfaceLayer = waterSurfaceMask.LayerMaskToLayer();

        RaycastHit2D[] hits = Physics2D.RaycastAll(thisPos, direction, laserDistanceCast, collisionMask);
        foreach (RaycastHit2D hit in hits)
        {
            // Skip water surface
            if (hit.collider.gameObject.layer == waterSurfaceLayer)
            {
                hitWater = true;
                waterCollisionPos = hit.point;
            }
            // Terrain, body, stop extending
            else
            {
                hitObstacle = true;
                obstaclePos = hit.point;
                break;
            }
        }

        // Laser end point
        Vector2 laserEndPos = Vector2.zero;
        if(hitObstacle)
        {
            laserEndPos = obstaclePos;
        }
        else
        {
            laserEndPos = thisPos + direction * laserDistanceCast;
        }

        // Set laser position
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, laserEndPos - thisPos);

        // Whether hit water
        if (!hitWater)
        {
            if (!touchedWater || thisPos.y >= castWaterY)
            {
                state = aquaMode.aboveWater;
            }
            else
            {
                state = aquaMode.belowWater;
            }
        }
        else
        {
            touchedWater = true;
            castWaterY = waterCollisionPos.y;
            if (direction.y < 0.0f)
            {
                state = aquaMode.intoWater;
            }
            else
            {
                state = aquaMode.outtaWater;
            }
        }

        // Set underwater bubble emitter
        if (state != aquaMode.aboveWater)
        {
            laserParticle.keepOn();
        }
        else
        {
            laserParticle.keepOff();
        }
        Vector2 waterBubbleStartPos = Vector2.zero;
        Vector2 waterBubbleEndPos = Vector2.zero;
        switch (state)
        {
            case aquaMode.aboveWater:
                
                break;
            case aquaMode.belowWater:
                waterBubbleStartPos = thisPos;
                waterBubbleEndPos = laserEndPos;
                break;
            case aquaMode.intoWater:
                waterBubbleStartPos = waterCollisionPos;
                waterBubbleEndPos = laserEndPos;
                break;
            case aquaMode.outtaWater:
                waterBubbleStartPos = thisPos;
                waterBubbleEndPos = waterCollisionPos;
                break;
        }
        ParticleSystem.ShapeModule laserParticleShape = laserParticle.shape;
        // Calculate particle emitter angle
        float directionAngle = Mathf.Atan2(direction.y, direction.x) * 180.0f / 3.14f;
        laserParticleShape.rotation = Vector3.forward * directionAngle;
        laserParticleShape.radius = Vector2.Distance(waterBubbleStartPos, waterBubbleEndPos) / 2.0f;
        laserParticleShape.position = (waterBubbleStartPos + waterBubbleEndPos)/ 2.0f - thisPos;
    }

    private void OnDrawGizmos()
    {
        if(debugOn && target != null)
        {
            Vector2 thisPos = transform.position;
            Vector2 targetPos = target.position;

            Debug.DrawLine(thisPos, targetPos, debugColor_laser);
        }
    }
}
