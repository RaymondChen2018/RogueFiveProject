using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHurt : MonoBehaviour
{
    [Tooltip("Per time span specified")]
    [SerializeField] private float damage = 10.0f;
    [Tooltip("Deals specified damage every this amount of seconds")]
    [SerializeField] private float damageTickSpan = 0.5f;

    private float time_to_deal_damage = 0.0f;
    List<Health> victimList = new List<Health>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > time_to_deal_damage)
        {
            time_to_deal_damage = Time.time + damageTickSpan;
            for(int i=0;i<victimList.Count;i++)
            {
                if(victimList[i] != null)
                {
                    victimList[i].damage(damage);
                }
                else
                {
                    victimList.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health victim = collision.gameObject.GetComponent<Health>();
        if(victim != null && !victimList.Contains(victim))
        {
            victimList.Add(victim);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Health victim = collision.gameObject.GetComponent<Health>();
        if (victim != null && victimList.Contains(victim))
        {
            victimList.Remove(victim);
        }
    }
}
