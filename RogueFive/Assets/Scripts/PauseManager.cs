using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Every pausible is responsible for subscribing to this manager on spawn (OnStart())
/// 
/// A pausible is not required to unsub by itself on death/destroy because the manager can clear them
/// off easily; additionally unsubbing onDestroy can create hazard where the scene changes with this 
/// manager destroyed before its subscribers and calling unsub will generates error due to manager missing.
/// </summary>
public class PauseManager : MonoBehaviour
{
    private static PauseManager singleton = null;
    private List<Pausible> subscribers = new List<Pausible>();

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogError("Duplicate PauseManager!");
        }
    }

    void OnDestroy()
    {
        singleton = null;
    }

    /// <summary>
    /// Pause action
    /// </summary>
    /// <param name="pause"> Pause = true; Unpause = false.</param>
    public static void Pause(bool pause)
    {
        for (int i = 0; i < singleton.subscribers.Count; i++)
        {
            // Removing unsubbers
            if(singleton.subscribers[i] == null)
            {
                singleton.subscribers.RemoveAt(i);
                i--;
                continue;
            }

            // Pause action
            if (pause)
            {
                singleton.subscribers[i].Pause();
                TimeScaleManager.SetTimeScale(0.0f);
            }
            else
            {
                singleton.subscribers[i].Unpause();
                TimeScaleManager.SetTimeScale(0.0f);
            }
        }
    }

    public static void Subscribe(Pausible subscriber)
    {
        if (!singleton.subscribers.Contains(subscriber))
        {
            singleton.subscribers.Add(subscriber);
        }
        else
        {
            Debug.LogError("Duplicate subscriber: " + subscriber + "!");
        }
    }

    public static void Unsubscribe(Pausible unsubscriber)
    {
        if (singleton.subscribers.Contains(unsubscriber))
        {
            singleton.subscribers.Remove(unsubscriber);
        }
        else
        {
            Debug.LogError("Attempting to unsub but subscriber: " + unsubscriber + " was not subscribed!");
        }
    }
}
