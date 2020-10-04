using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FishFin : MonoBehaviour
{
    [Header("Fold inside when moving")]
    [SerializeField] private bool enableMovementInfluence = true;
    [Tooltip("How much does movement affect bone")] [SerializeField] float externalInfluenceRatio = 0.5f;
    [SerializeField] float boneLength = 10.0f;

    [Header("Restore bone orientation when static")]
    [SerializeField] private bool enableRestore = true;
    [Tooltip("How fast does bone bends back to initial bone direction?")][SerializeField] float restoreInterpolateRatio = 0.1f;
    private Vector2 prevPos;
    private Quaternion boneLocalRotation;

    [Header("Flip Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("debug")]
    [SerializeField] private bool debugOn = true;
    [SerializeField] private Color debugColor_bone = Color.red;
    [SerializeField] private Color debugColor_influence = Color.green;

    // Start is called before the first frame update
    void Start()
    {
        prevPos = transform.position;
        boneLocalRotation = transform.rotation;
        Assert.IsTrue(spriteRenderer.gameObject != gameObject, "sprite renderer must be separated for sprite flip & 180 rotation");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get movement delta
        Vector2 thisPos = transform.position;
        Vector2 deltaPos = (thisPos - prevPos);// / Time.deltaTime;
        prevPos = thisPos;

        // Restore
        if (enableRestore)
        {
            Quaternion velocityQuat = Quaternion.FromToRotation(Vector2.right, deltaPos);
            Quaternion boneRotation = boneLocalRotation;
            if (deltaPos.x < 0.0f)
            {
                boneRotation = Quaternion.Inverse(boneLocalRotation);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, velocityQuat * boneRotation, restoreInterpolateRatio);
        }

        // External Influence
        if (enableMovementInfluence && deltaPos.magnitude > 0.1f)
        {
            Vector2 bendToDir = (-deltaPos).normalized;
            Quaternion bendToQuat = Quaternion.FromToRotation(Vector2.right, bendToDir);
            float movementInfluence = Mathf.Clamp(externalInfluenceRatio * deltaPos.magnitude / boneLength, 0.0f, 1.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, bendToQuat, movementInfluence);
            //Debug.DrawLine((Vector2)(bendToQuat * Vector3.right * movementInfluence * boneLength) + thisPos, thisPos, debugColor_influence);
        }

        // Flip sprite
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
