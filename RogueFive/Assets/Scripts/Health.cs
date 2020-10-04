using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health points")]
    [SerializeField] private float health = 10.0f;
    [SerializeField] private float maxHealth = 10.0f;

    [Header("Death")]
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] UnityEvent OnDeath = new UnityEvent();
    private bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // On Death
        if(!isDead && health <= 0.0f)
        {
            isDead = true;
            OnDeath.Invoke();
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }

    public void damage(float damageAmount)
    {
        health -= damageAmount;
    }
}
