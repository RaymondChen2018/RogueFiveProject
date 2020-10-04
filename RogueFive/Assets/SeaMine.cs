using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMine : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    [Header("Detection")]
    [SerializeField] private float triggerRadius = 10.0f;
    private float alarmRadius = 40.0f;
    [SerializeField] private float beepIntervalMax = 5.0f;
    [SerializeField] private float beepIntervalMin = 0.2f;
    [SerializeField] private ParticleSystem redLight;
    private float timeSinceLastBeep;
    private List<Collider2D> targetsInRadius = new List<Collider2D>();

    [Header("Chain")]
    [SerializeField] private float chainMaxLength = 200.0f;
    [Tooltip("How chain fades when extended to max length?")][SerializeField] private Gradient chainMaxFadeGradient;
    [Tooltip("How far chain extend below anchor point? eg. insert into ground")][SerializeField] private float chainExtraLength = 10.0f;
    [SerializeField] private LineRenderer chainLineRenderer;
    [SerializeField] private LayerMask terrainMask;

    [Header("Debug")]
    [SerializeField] private bool debugOn = false;
    [SerializeField] private Color debugColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 thisPos = transform.position;

        RaycastHit2D bottom = Physics2D.Raycast(thisPos, Vector2.down, chainMaxLength, terrainMask);
        // can reach bottom
        if(bottom)
        {
            chainLineRenderer.SetPosition(1, bottom.point - thisPos + Vector2.down * chainExtraLength);
        }
        // cannot reach bottom, fade out
        else
        {
            // Extend to max length
            chainLineRenderer.SetPosition(1, Vector2.down * chainMaxLength);

            // Fade out ending
            chainLineRenderer.colorGradient = chainMaxFadeGradient;
        }

        // Beep
        // Create random beeping offset (when grouped together)
        timeSinceLastBeep = Random.Range(0.0f, beepIntervalMax);
        alarmRadius = GetComponent<CircleCollider2D>().radius;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 thisPos = transform.position;

        // Find closest target
        float closestDist = alarmRadius;
        foreach(Collider2D target in targetsInRadius)
        {
            
            Vector2 closestPointTarget = target.ClosestPoint(thisPos);
            float targetDist = Vector2.Distance(thisPos, closestPointTarget);
            if(targetDist < closestDist)
            {
                closestDist = targetDist;
            }
        }
        float gapBetweenTriggerTarget = Mathf.Max(0.0f, closestDist - triggerRadius);

        // Explode
        if(gapBetweenTriggerTarget == 0.0f)
        {
            // Create explosion
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Detach chain
            chainLineRenderer.transform.parent = transform.parent;

            // Clean up
            Destroy(gameObject);
        }
        // Alarm frequent as distance close in
        else
        {
            // Closer target is, more frequent the beep is
            float alarmIntervalMultiplier = Mathf.Clamp(gapBetweenTriggerTarget / alarmRadius, 0.0f, 1.0f);

            // Beep interval
            float beepInterval = Mathf.Clamp(beepIntervalMax * alarmIntervalMultiplier, beepIntervalMin, beepIntervalMax);

            // Time to beep
            if(Time.time > timeSinceLastBeep + beepInterval)
            {
                timeSinceLastBeep = Time.time;

                // Emit red light particle
                redLight.Play();
            }
        }
    }
    private void OnDrawGizmos()
    {
        if(debugOn)
        {
            Vector2 thisPos = transform.position;
            DebugDraw.DrawEllipse(thisPos, triggerRadius, debugColor);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        targetsInRadius.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        targetsInRadius.Remove(collision);
    }
}
