using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField] UnityEvent OnPressed = new UnityEvent();

    [Header("Debug")]
    [SerializeField] private bool debugOn = true;
    [SerializeField] private Color debugColor_link = Color.yellow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(debugOn)
        {
            Vector2 thisPos = transform.position;
            for(int i=0;i<OnPressed.GetPersistentEventCount();i++)
            {
                GameObject targetObject = OnPressed.GetPersistentTarget(i) as GameObject;
                Debug.DrawLine(thisPos, targetObject.transform.position, debugColor_link);
            }
        }
    }
}
