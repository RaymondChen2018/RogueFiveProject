using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnSensor : MonoBehaviour
{
    [Header("Radius")]
    [Tooltip("If zone is closer than this, start spawning")][SerializeField] private float outterRadius = 200.0f;
    [Tooltip("If zone is too close, stop spawning")] [SerializeField] private float innerRadius = 100.0f;

    [Header("Touch zone")]
    [SerializeField] private LayerMask zoneLayer;
    List<Collider2D> zones = new List<Collider2D>();

    [Header("debug")]
    [SerializeField] private bool debugOn = false;
    [SerializeField] private Color debugColor = Color.red;

    CircleCollider2D circleCollider;

    // Start is called before the first frame update
    void Start()
    {
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = outterRadius;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 thisPos = transform.position;

        foreach(Collider2D zone in zones)
        {
            // zone too close, disable
            if (Vector2.SqrMagnitude(thisPos - zone.ClosestPoint(thisPos)) < innerRadius * innerRadius)
            {
                zone.GetComponent<SpawnZone_fish>().markTooClose();
            }
            // zone within outter & inner radius, keep enable
            else
            {
                zone.GetComponent<SpawnZone_fish>().enableZone();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(debugOn)
        {
            Vector2 thisPos = transform.position;
            DebugDraw.DrawEllipse(thisPos, outterRadius, debugColor);
            DebugDraw.DrawEllipse(thisPos, innerRadius, debugColor);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // wait analyze inner radius in Update()

        // add to collection
        zones.Add(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // remove from collection
        zones.Remove(collision);

        // disable zone
        collision.GetComponent<SpawnZone_fish>().disableZone();
    }
}
