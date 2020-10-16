using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRotate : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float pitchMaxAngle = 70.0f;
    [SerializeField] private float pitchMinAngle = -70.0f;
    [Tooltip("How fast interpolate to target rotation?")] [SerializeField] private float slerpRatio = 0.1f;

    Vector2 prevPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get movement delta
        Vector2 thisPos = transform.position;
        Vector2 deltaPos = thisPos - prevPos;
        prevPos = thisPos;

        // Only rotate when delta isnt zero
        // Because if it were it reset fish rotation to zero degree
        if(deltaPos != Vector2.zero)
        {
            // Rotate sprite
            // clamp rotation between upper & lower angle
            float movementRightAngle = Mathf.Atan2(deltaPos.y, Mathf.Abs(deltaPos.x)) * 180.0f / 3.14f;
            float rotation = Mathf.Clamp(movementRightAngle, pitchMinAngle, pitchMaxAngle);
            if (deltaPos.x < 0.0f)
            {
                rotation = 180.0f - rotation;
            }
            Quaternion quat = Quaternion.Euler(0, 0, rotation);
            Quaternion slerpResult = Quaternion.Slerp(transform.rotation, quat, slerpRatio);
            transform.rotation = slerpResult;
        }
    }
}
