using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpoFish : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpIntervalMax = 10.0f;
    [SerializeField] private float jumpIntervalMin = 1.0f;
    [SerializeField] private float jumpSpeedMax = 200.0f;
    [SerializeField] private float jumpSpeedMin = 90.0f;
    private float timeNextJump = 0;

    Rigidbody2D RB;

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Jump
        if (Time.time > timeNextJump)
        {
            float jumpRadian = Random.Range(0.0f, 360.0f) * 3.14f / 180.0f;
            Vector2 direction = new Vector2(Mathf.Cos(jumpRadian), Mathf.Sin(jumpRadian)).normalized;
            float jumpSpeed = Random.Range(jumpSpeedMin, jumpSpeedMax);
            RB.AddForce(jumpSpeed * direction);
            timeNextJump = Time.time + Random.Range(jumpIntervalMin, jumpIntervalMax);
        }
    }
}
