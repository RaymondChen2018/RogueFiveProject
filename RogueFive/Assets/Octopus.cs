using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float alertRadius = 30.0f;
    [SerializeField] private LayerMask playerMask;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3.0f;

    [Header("Tentacles")]
    [SerializeField] private Transform[] tentacleTips;
    [SerializeField] private float tentacleLength = 15.0f;
    [SerializeField] private float tentacleMoveSpeed = 3.0f;
    [SerializeField] private Transform mouth;

    [Header("Tentacle random movement")]
    [SerializeField] private float tentacleRandomMoveSpeed = 2.0f;
    [SerializeField] private float moveRandomIntervalMin = 0.5f;
    [SerializeField] private float moveRandomIntervalMax = 1.0f;
    private Vector2[] randomTargetPositions;
    private float[] timesToRandomize;

    [Header("Debug")]
    [SerializeField] private bool debugOn = false;
    [SerializeField] private Color debugColor_alertRadius = Color.red;
    [SerializeField] private Color debugColor_tentalce = Color.cyan;
    [SerializeField] private Color debugColor_tentalceLength = Color.blue;

    // Start is called before the first frame update
    void Start()
    {
        timesToRandomize = new float[tentacleTips.Length];
        randomTargetPositions = new Vector2[tentacleTips.Length];
        for (int i=0;i<timesToRandomize.Length;i++)
        {
            timesToRandomize[i] = 0.0f;
            randomTargetPositions[i] = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 thisPos = transform.position;
        Collider2D playerCollider = Physics2D.OverlapCircle(thisPos, alertRadius, playerMask);

        // Player in range
        if(playerCollider != null)
        {
            Vector2 playerPos = playerCollider.transform.position;

            //////////////////////Chase////////////////////////
            transform.position += (Vector3)(playerPos - thisPos).normalized * movementSpeed * Time.deltaTime;

            ///////////////// Grab//////////////////
            foreach(Transform tentacle in tentacleTips)
            {
                TentacleTip tip = tentacle.GetComponent<TentacleTip>();

                // Not active
                if (!tip.isActive())
                {
                    continue;
                }

                // Drag to mouth
                if (tip.hasGrabbedPlayer())
                {
                    Vector2 mouthDir = mouth.position - tentacle.position;
                    tentacle.transform.position += (Vector3)mouthDir.normalized * tentacleMoveSpeed * Time.deltaTime;
                }
                // Dispatch to player
                else
                {
                    
                    Vector2 playerDir = playerPos - (Vector2)tentacle.position;
                    tentacle.transform.position += (Vector3)playerDir.normalized * tentacleMoveSpeed * Time.deltaTime;
                }
            }
        }

        // Tentacle random movement
        for(int i=0;i<timesToRandomize.Length;i++)
        {
            // Time to randomize
            if(Time.time > timesToRandomize[i])
            {
                // Randomize position within tentacle reach
                float randomAngle = Random.Range(0.0f, 360.0f);
                float randomDistance = Random.Range(0.0f, tentacleLength);
                Vector2 randomDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * randomAngle), Mathf.Sin(Mathf.Deg2Rad * randomAngle));
                Vector2 randomPosition = randomDirection.normalized * randomDistance + (Vector2)transform.position;
                randomTargetPositions[i] = randomPosition;

                // Randomize time
                timesToRandomize[i] = Time.time + Random.Range(moveRandomIntervalMin, moveRandomIntervalMax);
            }
        }

        // Tentacle reach random target
        for(int i=0;i<tentacleTips.Length;i++)
        {
            Vector2 moveDirection = randomTargetPositions[i] - (Vector2)tentacleTips[i].position;
            tentacleTips[i].position += (Vector3)moveDirection.normalized * tentacleRandomMoveSpeed * Time.deltaTime;
        }

        // Constraint tentacle length
        foreach(Transform tentacle in tentacleTips)
        {
            Vector2 tentacleDir = (Vector2)tentacle.position - thisPos;
            float tentacleDist = tentacleDir.magnitude;
            if(tentacleDist >= tentacleLength)
            {
                tentacle.transform.position = thisPos + tentacleDir.normalized * tentacleLength;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(debugOn)
        {
            Vector2 thisPos = transform.position;

            // Alert radius
            DebugDraw.DrawEllipse(thisPos, alertRadius, debugColor_alertRadius);

            // Tentacles
            DebugDraw.DrawEllipse(thisPos, tentacleLength, debugColor_tentalceLength);
            foreach (Transform tentacle in tentacleTips)
            {
                if(tentacle == null)
                {
                    continue;
                }

                Vector2 tentaclePos = tentacle.position;
                Debug.DrawLine(thisPos, tentaclePos, debugColor_tentalce);
                DebugDraw.DrawEllipse(tentaclePos, 1.0f, debugColor_tentalce);
            }
        }
    }
}
