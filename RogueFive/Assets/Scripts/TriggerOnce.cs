using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventGameObject: UnityEvent<GameObject>
{

}

[RequireComponent(typeof(Collider2D))]
public class TriggerOnce : MonoBehaviour
{
    [SerializeField] private UnityEvent OnStartTouch = new UnityEvent();
    [SerializeField] private string triggerTag = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!(triggerTag != "" && collision.tag != triggerTag))
        {
            OnStartTouch.Invoke();
        }
    }
}
