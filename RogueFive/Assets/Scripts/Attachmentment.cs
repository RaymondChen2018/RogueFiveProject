using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachmentment : MonoBehaviour {
    [SerializeField] Transform parent = null;
    [Tooltip("Only follows position when true")][SerializeField] private bool fixRotation = false;
    [Tooltip("Only follow X Y coordinates when true")][SerializeField] private bool fixZ = false;
    [SerializeField] bool dontAttachEditor = false;

    [Header("Interpolation")]
    [Tooltip("Snap to if false")][SerializeField] bool interpolate = false;
    [SerializeField] float interpolateRatio = 0.2f;
    [Tooltip("When this distance away from parent, snap to parent")][SerializeField] float interpolateToleranceDistance = 0.1f;
    [Tooltip("When this angle away from parent rotation, snap to parent")] [SerializeField] float interpolateToleranceAngle = 0.1f;
    private bool interpolatingPos = false;
    private bool interpolatingRot = false;
    [Tooltip("Use regular Update() if false")][SerializeField] bool useFixedUpdate = true;

    // Use this for initialization
    void Start () {
        if(interpolate)
        {
            interpolatingPos = true;
            interpolatingRot = true;
        }
    }
    private void Update()
    {
        if(!useFixedUpdate)
        {
            privUpdateTransform();
        }
    }
    // Update is called once per frame
    void FixedUpdate ()
    {
        if(useFixedUpdate)
        {
            privUpdateTransform();
        }
    }

    virtual protected void privUpdateTransform()
    {
        if(parent != null)
        {
            Vector3 parentPos = parent.position;
            if (fixZ)
            {
                parentPos.z = transform.position.z;
            }

            if(!interpolate)
            {
                transform.position = parentPos;
                if (!fixRotation)
                {
                    transform.rotation = parent.rotation;
                }
            }
            else // interpolate
            {
                if(interpolatingPos)// interpolating
                {
                    transform.position = Vector3.Lerp(transform.position, parentPos, interpolateRatio);
                    if(((Vector2)(transform.position - parentPos)).sqrMagnitude < interpolateToleranceDistance * interpolateToleranceDistance)
                    {
                        interpolatingPos = false;
                    }
                }
                else// finished
                {
                    transform.position = parentPos;
                }

                if (!fixRotation)
                {
                    if(interpolatingRot) // interpolating
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, parent.rotation, interpolateRatio);
                        if(Mathf.Pow(transform.rotation.z - parent.rotation.z, 2) < interpolateToleranceAngle * interpolateToleranceAngle)
                        {
                            interpolatingRot = false;
                        }
                    }
                    else // finished
                    {
                        transform.rotation = parent.rotation;
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if(!dontAttachEditor)
        {
            privUpdateTransform();
        }
        
    }

    public void SetAttachment(Transform newParent)
    {
        parent = newParent;
        interpolatingPos = true;
        interpolatingRot = true;
    }
}
