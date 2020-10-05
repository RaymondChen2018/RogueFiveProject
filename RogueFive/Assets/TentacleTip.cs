using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleTip : TentacleJointBase
{
    private bool active = true;
    private bool grabbed = false;
    private float reactivateDuration = 3.0f;
    private float grabForceRatio = 0.06f;
    private float timeToReactivate = 0;

    // Update is called once per frame
    protected override void TentacleJointUpdate()
    {
        if(!active && Time.time > timeToReactivate)
        {
            active = true;
        }
    }

    public bool isActive()
    {
        return active;
    }
    public bool hasGrabbedPlayer()
    {
        return grabbed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        grabbed = true;
        transform.position = collision.transform.position;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        grabbed = false;
        if(active)
        {
            active = false;
            timeToReactivate = Time.time + reactivateDuration;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(active)
        {
            collision.transform.position = Vector2.Lerp(collision.transform.position, transform.position, grabForceRatio);
        }
    }

    
}
