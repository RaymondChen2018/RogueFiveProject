﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FishFlip : MonoBehaviour
{
    [Header("Orientation")]
    [SerializeField] private float pitchMaxAngle = 70.0f;
    [SerializeField] private float pitchMinAngle = -70.0f;
    [Tooltip("How fast interpolate to target rotation?")][SerializeField] private float slerpRatio = 0.1f;

    [Header("Flip Sprite")]
    [Tooltip("When this fish switch left to right or vice versa, it flip sprite to face that way")][SerializeField] private bool flipSpriteOtherWay = true;
    [Tooltip("Sprite separated to perform sprite flip effect alone")][SerializeField] private SpriteRenderer spriteRenderer;

    Vector2 prevPos;
    // Start is called before the first frame update
    void Start()
    {
        prevPos = transform.position;
        Assert.IsTrue(spriteRenderer.gameObject != gameObject, "sprite renderer must be separated for sprite flip & 180 rotation");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get movement delta
        Vector2 thisPos = transform.position;
        Vector2 deltaPos = thisPos - prevPos;
        prevPos = thisPos;

        // Orient sprite
        // Get movement angle
        float movementRightAngle = Mathf.Atan2(deltaPos.y, Mathf.Abs(deltaPos.x)) * 180.0f / 3.14f;
        float rotation = Mathf.Clamp(movementRightAngle, pitchMinAngle, pitchMaxAngle);
        if(deltaPos.x<0.0f)
        {
            rotation = 180.0f - rotation;
            //rotation *= -1;
        }
        Quaternion quat = Quaternion.Euler(0, 0, rotation);
        Quaternion slerpResult = Quaternion.Slerp(transform.rotation, quat, slerpRatio);
        transform.rotation = slerpResult;

        // flip sprite
        if(flipSpriteOtherWay)
        {
            if (deltaPos.x < 0.0f)
            {
                // flip sprite
                spriteRenderer.flipX = true;

                // flip rotation
                spriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, 180.0f);
            }
            else
            {
                // reset sprite
                spriteRenderer.flipX = false;

                // reset rotation
                spriteRenderer.transform.localRotation = Quaternion.identity;
            }
        }
    }
}
