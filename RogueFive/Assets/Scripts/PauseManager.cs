using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private static PauseManager singleton = null;
    private List<Pausible> subscribers = new List<Pausible>();
    public List<string> subscribersNames = new List<string>();

    // Awake
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
            if(singleton.subscribers[i] == null)
            {
                Debug.LogWarning("Subscriber" + singleton.subscribersNames[i] + " forgot to unsubscribe!");
                singleton.privRemoveSubscriber(i);
                i--;
                continue;
            }

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
            singleton.privAddSubscriber(subscriber);
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
            singleton.privRemoveSubscriber(unsubscriber);
        }
        else
        {
            Debug.LogError("Attempting to unsub but subscriber: " + unsubscriber + " is not subscribed!");
        }
    }

    private void privAddSubscriber(Pausible subscriber)
    {
        subscribers.Add(subscriber);
        subscribersNames.Add(subscriber.GetType().ToString());
    }
    private void privRemoveSubscriber(Pausible subscriber)
    {
        subscribers.Remove(subscriber);
        subscribersNames.Remove(subscriber.GetType().ToString());
    }
    private void privRemoveSubscriber(int index)
    {
        subscribers.RemoveAt(index);
        subscribersNames.RemoveAt(index);
    }
}
