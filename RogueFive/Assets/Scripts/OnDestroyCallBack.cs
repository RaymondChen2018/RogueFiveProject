using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDestroyCallBack : MonoBehaviour
{
    [SerializeField] private UnityEvent OnDestroyed = new UnityEvent();

    private void OnDestroy()
    {
        OnDestroyed.Invoke();
    }
}
