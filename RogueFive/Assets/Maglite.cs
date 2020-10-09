using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maglite : MonoBehaviour
{
    [Tooltip("Target to shine toward, eg. mouse position gameobject")][SerializeField] private Transform target;

    [Tooltip("View cone within which maglite can shine toward")]
    [SerializeField]
    [Range(0, 360.0f)] private float freedomCone = 40.0f;

    [Tooltip("Player direction reference")][SerializeField] private Transform playerRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            // Player rotation
            float playerRotationAngle = playerRotation.rotation.eulerAngles.z;

            // Target direction
            Vector2 thisPos = transform.position;
            Vector2 targetDirection = (Vector2)target.position - thisPos;
            float targetDirAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * 180.0f / 3.14f;

            // Constraint angle
            float angleFromPlayerDirection = Mathf.DeltaAngle(targetDirAngle, playerRotationAngle);
            float targetDirAngleConstrainted = targetDirAngle;

            // Counter-clock wise/left
            if (angleFromPlayerDirection < -freedomCone / 2.0f)
            {
                targetDirAngleConstrainted = playerRotationAngle + freedomCone / 2.0f;
            }
            // Clock wise/right
            else if(angleFromPlayerDirection > freedomCone / 2.0f)
            {
                targetDirAngleConstrainted = playerRotationAngle - freedomCone / 2.0f;
            }

            transform.rotation = Quaternion.Euler(0, 0, targetDirAngleConstrainted);
        }
    }
}
