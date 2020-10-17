using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    [Header("Laser properties")]
    [Tooltip("Target laser extends to")][SerializeField] private Transform target;
    [Tooltip("Layers laser can hit, eg. terrain,fish,watersurface")][SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float laserDamage = 10.0f;
    private LineRenderer lineRenderer;

    [Header("Flicker Pattern")]
    [Tooltip("Duration for one cycle")][SerializeField] private float patternDuration = 5.0f;
    [Tooltip("Laser intensity affected by only alpha value")][SerializeField] private Gradient pattern;
    private Gradient tempGradient;
    GradientAlphaKey[] tempAlphaKeys;

    [Header("Underwater Bubbles")]
    [SerializeField] bool isFullyUnderWater = false;
    [SerializeField] private LayerMask waterSurfaceMask;
    [SerializeField] private GameObject prefabLaserBubbleParticle;
    private ParticleSystem laserParticle;
    enum aquaMode
    {
        intoWater,
        outtaWater,
        belowWater,
        aboveWater
    }
    private aquaMode state = aquaMode.aboveWater;
    private bool touchedWater = false;
    private float castWaterY;

    [Header("Debug")]
    [SerializeField] private bool debugOn = true;
    [SerializeField] private Color debugColor_laser = Color.green;

    // Start is called before the first frame update
    void Start()
    {
        // If initially submerged
        if(isFullyUnderWater)
        {
            touchedWater = true;
            castWaterY = transform.position.z + 1.0f;
            state = aquaMode.belowWater;
        }

        lineRenderer = GetComponent<LineRenderer>();

        // Create bubble effect
        GameObject laserBubble = Instantiate(prefabLaserBubbleParticle);
        laserBubble.transform.parent = transform;
        laserBubble.transform.localPosition = Vector3.zero;
        laserParticle = laserBubble.GetComponent<ParticleSystem>();

        //
        tempGradient = new Gradient();
        tempAlphaKeys = new GradientAlphaKey[lineRenderer.colorGradient.alphaKeys.Length];
        for(int i=0;i< tempAlphaKeys.Length;i++)
        {
            tempAlphaKeys[i] = new GradientAlphaKey(1.0f, lineRenderer.colorGradient.alphaKeys[i].time);
        }
        tempGradient.SetKeys(lineRenderer.colorGradient.colorKeys, tempAlphaKeys);
        lineRenderer.colorGradient = tempGradient;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            Debug.LogWarning("this laser has no target");
            return;
        }

        // Read gradient for laser intensity
        float time = (Time.time % patternDuration) / patternDuration;
        float laserIntensity = pattern.readAlpha(time);

        // Override linerenderer alpha keys
        for (int i = 0;i < tempAlphaKeys.Length;i++)
        {
            tempAlphaKeys[i].alpha = laserIntensity;
        }
        tempGradient.SetKeys(tempGradient.colorKeys, tempAlphaKeys);
        lineRenderer.colorGradient = tempGradient;

        // Combine masks
        LayerMask collisionMask = waterSurfaceMask | obstacleMask;

        Vector2 thisPos = transform.position;
        Vector2 targetPos = target.position;
        Vector2 directionDelta = targetPos - thisPos;
        Vector2 direction = directionDelta.normalized;

        // Raycast distance
        float laserDistanceCast = directionDelta.magnitude;
        
        // Test water & terrain
        bool hitWater = false;
        Vector2 waterCollisionPos = Vector3.zero;
        bool hitObstacle = false;
        Vector2 obstaclePos = Vector3.zero;
        int waterSurfaceLayer = waterSurfaceMask.LayerMaskToLayer();
        RaycastHit2D[] hits = Physics2D.RaycastAll(thisPos, direction, laserDistanceCast, collisionMask);
        foreach (RaycastHit2D hit in hits)
        {
            // Extend through water surface
            if (hit.collider.gameObject.layer == waterSurfaceLayer)
            {
                hitWater = true;
                waterCollisionPos = hit.point;
            }
            // Terrain or body, stop extending
            else
            {
                hitObstacle = true;
                obstaclePos = hit.point;

                // Reduce health if has health
                Health targetHealth = hit.collider.GetComponent<Health>();
                if (targetHealth != null)
                {
                    float damage = laserDamage * laserIntensity * Time.deltaTime;
                    if(damage > 0.0f)
                    {
                        targetHealth.damage(damage);
                    }
                }

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

        // Set line renderer
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, laserEndPos - thisPos);

        // Evaluate if laser shoots into water or out of water
        // or completely submerged pr completely above water
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

        // Emit bubbles from submerged laser segment
        if (state != aquaMode.aboveWater && laserIntensity > 0.0f)
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

            LineRenderer line = GetComponent<LineRenderer>();
            line.SetPosition(0, Vector2.zero);
            line.SetPosition(1, targetPos - thisPos);
        }
    }
}
