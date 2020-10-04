using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTail : MonoBehaviour
{
    [Header("Head")]
    [SerializeField] private Transform frontSegment;

    [Header("Elastic")]
    [SerializeField] private float tailLength = 10.0f;
    [SerializeField] private float interpolateRatio = 0.3f;

    [Header("debug")]
    [SerializeField] private bool debugOn = false;
    [SerializeField] private Color debugColor = Color.red;
    [SerializeField] private Color debugToleranceColor = Color.blue;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 thisPos = transform.position;
        Vector2 frontPos = frontSegment.position;
        Vector2 frontSegmentDir = frontPos - thisPos;

        float distFrontSegment = frontSegmentDir.magnitude;
        // drag too long, contract
        if(distFrontSegment > tailLength)
        {
            transform.position = Vector2.Lerp(thisPos, frontPos - frontSegmentDir.normalized * tailLength, interpolateRatio);
        }
        else
        {

        }
    }

    private void OnDrawGizmos()
    {
        if(debugOn)
        {
            if(frontSegment == null)
            {
                return;
            }
            Vector2 thisPos = transform.position;
            Vector2 frontPos = frontSegment.position;
            Vector2 frontSegmentDir = frontPos - thisPos;
            Vector2 contractToPos = frontPos - frontSegmentDir.normalized * tailLength;
            Debug.DrawLine(thisPos, contractToPos, debugColor);
            DebugDraw.DrawEllipse(contractToPos, 1.0f, debugColor);
            DebugDraw.DrawEllipse(frontSegment.position, tailLength, debugToleranceColor);
        }
    }
}
