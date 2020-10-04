using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion_underwater : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private float damage = 100.0f;
    [SerializeField] private LayerMask damageMask;
    [SerializeField] private float damageRadius = 10.0f;

    [Header("Oscillation")]
    [Tooltip("How long does force oscillates?")][SerializeField] private float oscillateDuration = 0.5f;
    [Tooltip("How many oscillations (#expand or #contract)?")][SerializeField] private float numOscillation = 3;
    [Tooltip("Starts with outward force")][SerializeField] private float initialForce = 20.0f;
    [Tooltip("Oscillation damp ratio per oscillation")] [SerializeField] private float oscillationDamp = 0.8f;

    [Header("Sub Components")]
    [SerializeField] Transform bubbleToDetach;
    [SerializeField] private ParticleSystemForceField forceField;

    // Misc
    private float timeToBounce = 0;
    private float timeToDestroy = 0;
    private float oscillateInterval = 1.0f;
    private float currForce = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Explode
        Vector2 thisPos = transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(thisPos, damageRadius, damageMask);
        for(int i = 0; i < hits.Length; i++)
        {
            // Calculate fallout damage
            Collider2D hit = hits[i];
            float distFromEpicenter = Vector2.Distance(thisPos, hit.ClosestPoint(thisPos));
            float damageDealt = Mathf.Clamp(1.0f - (distFromEpicenter / damageRadius), 0.0f, 1.0f) * damage;

            // subtract victim health
            Health victimHealth = hit.GetComponent<Health>();
            victimHealth.damage(damageDealt);
        }

        // misc
        timeToDestroy = Time.time + oscillateDuration;
        oscillateInterval = oscillateDuration / numOscillation;
        timeToBounce = Time.time + oscillateInterval;
        currForce = initialForce;
        forceField.gravity = currForce;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timeToBounce)
        {
            currForce *= -oscillationDamp;
            forceField.gravity = currForce;

            timeToBounce = Time.time + oscillateInterval;
        }

        // Remove
        if(Time.time > timeToDestroy)
        {
            bubbleToDetach.parent = null;
            Destroy(forceField.gameObject);
            Destroy(gameObject);
        }
    }
}
