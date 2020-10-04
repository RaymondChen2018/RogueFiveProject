using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowFish : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100.0f;

    float targetDistTolerance = 1.0f;
    Rigidbody2D RB;
    CircleCollider2D circleCollider;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        Vector2 thisPos = transform.position;

        // move toward mouse
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 moveToward = targetPos - thisPos;
        if (moveToward.magnitude > circleCollider.radius * 2)
        {
            RB.AddForce(moveToward.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {

        }
    }
        
        
}
