using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    protected Transform currTarget = null;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 100.0f;

    [Header("Detection")]
    [SerializeField] private float detectRadius = 100.0f;

    [Header("Debug")]
    [SerializeField] protected bool debugOn = false;
    [SerializeField] private Color debugColor_detectRadius = Color.red;

    private Rigidbody2D RB;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();

        OnAIStart();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 thisPos = transform.position;

        // Set target as player
        if (Player.player != null && Vector2.Distance(thisPos, Player.player.transform.position) < detectRadius)
        {
            currTarget = Player.player.transform;
        }

        // Move to target
        if(currTarget != null)
        {
            Vector2 targetPos = currTarget.position;
            Vector2 targetVec = targetPos - thisPos;
            RB.AddForce(targetVec.normalized * movementSpeed * Time.deltaTime);
        }

        OnAIUpdate();
    }

    virtual protected void OnAIStart()
    {

    }
    virtual protected void OnAIUpdate()
    {

    }

    private void OnDrawGizmos()
    {
        if(debugOn)
        {
            Vector2 thisPos = transform.position;

            DebugDraw.DrawEllipse(thisPos, detectRadius, debugColor_detectRadius);
        }
    }
}
