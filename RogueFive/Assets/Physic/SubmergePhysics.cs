using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DiveState
{
    onLand,
    halfInWater,
    fullyInWater
}

public class SubmergePhysics : MonoBehaviour
{
    [Header("Dive settings")]
    [SerializeField] private float gravityWater = 0.07f;
    [SerializeField] private float dragAir = 1.0f;
    [SerializeField] private float dragWater = 1.7f;

    [Header("Bubble effect (Optional)")]
    [SerializeField] private ParticleSystem bubbleSystem;
    float baseBubbleRate = 3.0f;
    float prevSubmergeRatio = -1.0f;

    [Header("Output")]
    [SerializeField] private UnityEvent OnSurface = new UnityEvent();
    [SerializeField] private UnityEvent OnFullySubmerge = new UnityEvent();
    [SerializeField] private DiveState diveState = DiveState.onLand;
    Rigidbody2D RB;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getSubmergeRatio()
    {
        return prevSubmergeRatio;
    }

    /// <summary>
    /// Larger than 1.0f: Fully in water
    /// 0.0f - 1.0f: half in water
    /// Less than 0.0f: On land (full gravity)
    /// </summary>
    /// <param name="ratio"></param>
    public void submergeAmount(float ratio)
    {
        if (ratio < 0.0f)
        {
            diveState = DiveState.onLand;
        }
        else if (ratio < 1.0f)
        {
            if (diveState == DiveState.fullyInWater)
            {
                OnSurface.Invoke();
            }
            diveState = DiveState.halfInWater;
        }
        else
        {
            if (diveState != DiveState.fullyInWater)
            {
                OnFullySubmerge.Invoke();
            }
            diveState = DiveState.fullyInWater;
        }
        RB.gravityScale = Mathf.Lerp(1.0f, gravityWater, ratio);
        RB.drag = Mathf.Lerp(dragAir, dragWater, ratio);

        // Calculate bubble effect
        if(bubbleSystem != null)
        {
            ParticleSystem.EmissionModule emission = bubbleSystem.emission;
            emission.rateOverTime = baseBubbleRate;

            if (diveState == DiveState.halfInWater)
            {
                float submergeDelta = ratio - prevSubmergeRatio;
                if (submergeDelta > 0.0f)
                {
                    emission.rateOverTime = baseBubbleRate + submergeDelta * 5000;
                }

                prevSubmergeRatio = ratio;
            }
        }
    }

    public DiveState getDiveState()
    {
        return diveState;
    }
}
