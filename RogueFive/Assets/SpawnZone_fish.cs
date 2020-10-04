using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnState
{
    TooClose,
    Inactive,
    Active
}

public class SpawnZone_fish : MonoBehaviour
{
    [SerializeField] private GameObject fishPrefab;
    [Tooltip("How many fishes should this zone spawn & track?")][SerializeField] private int maxFishCount = 10;

    [Header("debug")]
    [SerializeField] private bool debugOn = false;
    [SerializeField] private Color debugColor = Color.cyan;

    private int fishCount = 0;
    private List<GameObject> fishes = new List<GameObject>();
    private SpawnState zoneState = SpawnState.Inactive;
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        // Set spawn zone shape
        ParticleSystem.ShapeModule shapeModule = ps.shape;
        shapeModule.mesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        // Clean up null links
        for(int i = 0;i < fishes.Count; i++)
        {
            if(fishes[i] == null)
            {
                fishes.RemoveAt(i);
                i--;
            }
        }
        fishCount = fishes.Count;

        // Switch on/off particle system
        if(fishCount >= maxFishCount || zoneState != SpawnState.Active)
        {
            ps.Pause();
            ps.Clear();
        }
        else
        {
            ps.Play();
        }

        // Spawn fish
        if(zoneState == SpawnState.Active)
        {
            // Get current particles
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
            ps.GetParticles(particles);

            // 
            for(int i = 0; i < particles.Length && fishCount < maxFishCount; i++)
            {
                // Get particle info
                ParticleSystem.Particle particleInfo = particles[i];

                // Create fish
                fishes.Add(Instantiate(fishPrefab, particleInfo.position, Quaternion.identity));

                // Increment count
                fishCount++;
            }
        }
    }

    public void enableZone()
    {
        zoneState = SpawnState.Active;
    }
    public void disableZone()
    {
        zoneState = SpawnState.Inactive;
    }
    public void markTooClose()
    {
        zoneState = SpawnState.TooClose;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(zoneState == SpawnState.Inactive && collision.tag == "Fish")
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if(debugOn)
        {
            PolygonCollider2D polyCollider = GetComponent<PolygonCollider2D>();
            if(polyCollider != null)
            {
                DebugDraw.DrawPolygon(polyCollider, debugColor);
            }
        }
    }
}
