using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float alertRadius = 30.0f;
    [SerializeField] private LayerMask playerMask;

    [Header("Tentacles")]
    [SerializeField] private Transform[] tentacleTips;
    [SerializeField] private float tentacleLength = 15.0f;
    [SerializeField] private float tentacleMoveSpeed = 2.0f;
    [SerializeField] private Transform mouth;

    [Header("Debug")]
    [SerializeField] private bool debugOn = false;
    [SerializeField] private Color debugColor_alertRadius = Color.red;
    [SerializeField] private Color debugColor_tentalce = Color.cyan;
    [SerializeField] private Color debugColor_tentalceLength = Color.blue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 thisPos = transform.position;
        Collider2D playerCollider = Physics2D.OverlapCircle(thisPos, alertRadius, playerMask);

        // Player in range, grab with tentacle
        if(playerCollider != null)
        {
            Vector2 playerPos = playerCollider.transform.position;

            // Find closest tentacle
            float closestDist = alertRadius;
            Transform closestTentacle = null;
            foreach(Transform tentacle in tentacleTips)
            {
                float tentacleDist = Vector2.Distance(tentacle.position, playerPos);
                if (tentacleDist < closestDist)
                {
                    closestDist = tentacleDist;
                    closestTentacle = tentacle;
                }
            }

            // Dispatch tentacle
            Vector2 playerDir = playerPos - (Vector2)closestTentacle.position;
            closestTentacle.transform.position += (Vector3)playerDir.normalized * tentacleMoveSpeed * Time.deltaTime;
        }

        // Constraint tentacle length
        foreach(Transform tentacle in tentacleTips)
        {
            Vector2 tentacleDir = (Vector2)tentacle.position - thisPos;
            float tentacleDist = tentacleDir.magnitude;
            if(tentacleDist >= tentacleLength)
            {
                tentacle.position = thisPos + tentacleDir.normalized * tentacleLength;
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
