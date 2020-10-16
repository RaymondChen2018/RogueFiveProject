using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_fishgun : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private KeyCode keyShoot;
    [SerializeField] private KeyCode keyRelease;
    [SerializeField] private int numShots = 3;

    [Header("Projectile")]
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private GameObject prefabSpike;
    [SerializeField] private float spikeSpeed = 50.0f;
    private Transform spike;

    [Header("Rope")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Pull")]
    [SerializeField] private float pullForce = 100.0f;
    private Transform pin;

    [Header("Player")]
    [SerializeField] Rigidbody2D playerRB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerPos = playerRB.position;
        Vector2 ropeEndPos = playerPos;

        // Update spike position
        if(spike != null)
        {
            ropeEndPos = spike.position;
            Vector2 movementDirection = spike.rotation * Vector3.right;
            float movementDistance = spikeSpeed * Time.deltaTime;
            Vector2 movementDelta = movementDirection * movementDistance;

            // Test collision
            RaycastHit2D hit = Physics2D.Raycast(spike.position, movementDirection, movementDistance, terrainMask);
            if(hit.collider != null)
            {
                spike.position = hit.point;

                // Become pin
                pin = spike;
                spike = null;
            }
            else
            {
                // Move spike
                spike.position += (Vector3)movementDelta;
            }
        }
        // Pull player
        else if(pin != null)
        {
            Vector2 pinPos = pin.position;
            ropeEndPos = pinPos;
            Vector2 pullDirection = (pinPos - playerPos).normalized;
            playerRB.AddForce(pullDirection * pullForce);
        }

        // Update rope line renderer
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, ropeEndPos - playerPos);

        // Can shoot
        if (numShots > 0 && Input.GetKeyDown(keyShoot))
        {
            // Remove prior pin or spike
            privRelease();

            // Mouse position
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Projectile direction
            Vector2 direction = (mousePos - playerPos).normalized;
            Quaternion spikeRotation = Quaternion.FromToRotation(Vector3.right, direction);

            // Create spike projectile
            GameObject spikeObj = Instantiate(prefabSpike, playerPos, spikeRotation);
            spike = spikeObj.transform;
            //spike.parent = transform;

            // Decrement shot
            numShots--;
        }
        else if(Input.GetKeyDown(keyRelease))
        {
            privRelease();
        }
    }

    private void privRelease()
    {
        if (pin != null)
        {
            Destroy(pin.gameObject);
        }
        if (spike != null)
        {
            Destroy(spike.gameObject);
        }
    }
}
