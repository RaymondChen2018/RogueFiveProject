using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleJoint : TentacleJointBase
{
    [Header("Contraction")]
    [SerializeField] private Transform rootEnd;
    [SerializeField] private float rootNormalLength = 10.0f;
    [SerializeField] private float rootCapLength = 20.0f;
    [SerializeField] private float contractForceScaleRoot = 60.0f;
    [SerializeField] private float expandForceScaleRoot = 0.1f;

    [SerializeField] private Transform tipEnd;
    [SerializeField] private float tipNormalLength = 10.0f;
    [SerializeField] private float tipCapLength = 20.0f;
    [SerializeField] private float contractForceScaleTip = 100.0f;
    [SerializeField] private float expandForceScaleTip = 0.1f;

    [Header("Move Joint")]
    [SerializeField] private float interpolateRatio = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool debugOn = false;
    [SerializeField] private bool debugDrawGradient = false;
    [SerializeField] private Color debugColor_rootEnd = Color.cyan;
    [SerializeField] private Color debugColor_tipEnd = Color.blue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    override protected void TentacleJointUpdate()
    {
        if(rootEnd == null || tipEnd == null)
        {
            return;
        }

        // Calculate contract force
        Vector2 thisPos = transform.position;
        Vector2 rootPos = rootEnd.position;
        Vector2 tipPos = tipEnd.position;
        float rootDistance = Vector2.Distance(thisPos, rootPos);
        float tipDistance = Vector2.Distance(thisPos, tipPos);

        // Cap length, if exceed length, generate no force
        float rootForce = (rootDistance - rootNormalLength);
        float tipForce = (tipDistance - tipNormalLength);

        rootForce = Mathf.Clamp(rootForce, -rootCapLength, rootCapLength);
        tipForce = Mathf.Clamp(tipForce, -tipCapLength, tipCapLength);

        float rootContractForceMag = getScaleForce(rootForce, contractForceScaleRoot, expandForceScaleRoot);
        float tipContractForceMag = getScaleForce(tipForce, contractForceScaleTip, expandForceScaleTip);

        Vector2 rootContractForce = (rootPos - thisPos).normalized * rootContractForceMag;
        Vector2 tipContractForce = (tipPos - thisPos).normalized * tipContractForceMag;

        Vector2 combinedContractForce = rootContractForce + tipContractForce;

        // Move this tentacle
        //Debug.DrawLine(thisPos, thisPos + combinedContractForce, Color.red);
        transform.position += (Vector3)combinedContractForce * interpolateRatio * Time.deltaTime;
    }

    private float getScaleForce(float force, float contractScale, float expandScale)
    {
        if(force > 0.0f)
        {
            return force * contractScale;
        }
        return force * expandScale;
    }

    private void OnDrawGizmos()
    {
        if(debugOn)
        {
            Vector2 thisPos = transform.position;

            // Draw left
            if (rootEnd != null)
            {
                Vector2 leftPos = rootEnd.position;
                if(!debugDrawGradient)
                {
                    Debug.DrawLine(thisPos, leftPos, debugColor_rootEnd);
                }
                else
                {
                    DebugDraw.DrawGradientLine(thisPos, leftPos, debugColor_rootEnd, Color.clear);
                }

                // Draw arc
                DebugDraw.DrawArc(thisPos, rootNormalLength, leftPos, 30.0f, debugColor_rootEnd);
            }

            // Draw right
            if (tipEnd != null)
            {
                Vector2 rightPos = tipEnd.position;
                if (!debugDrawGradient)
                {
                    Debug.DrawLine(thisPos, rightPos, debugColor_tipEnd);
                }
                else
                {
                    DebugDraw.DrawGradientLine(thisPos, rightPos, debugColor_tipEnd, Color.clear);
                }

                // Draw arc
                DebugDraw.DrawArc(thisPos, tipNormalLength, rightPos, 30.0f, debugColor_tipEnd);
            }
        }

    }

    public void initialize(Transform newRootEnd, Transform newTipEnd, 
        float length, float capLength,
        float contractScaleRoot,
        float expandScaleRoot,
        float contractScaleTip,
        float expandScaleTip)
    {
        rootEnd = newRootEnd;
        tipEnd = newTipEnd;
        rootNormalLength = length;
        rootCapLength = capLength;
        tipNormalLength = length;
        tipCapLength = capLength;

        contractForceScaleRoot = contractScaleRoot;
        expandForceScaleRoot = expandScaleRoot;

        contractForceScaleTip = contractScaleTip;
        expandForceScaleTip = expandScaleTip;
    }
}
