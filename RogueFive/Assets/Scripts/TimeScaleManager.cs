using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour, Pausible
{
    private static TimeScaleManager singleton = null;
    private float timeScale = 1.0f;
    private bool GamePaused = false;

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
    void Start()
    {
        privOnAwakeRegisterPausible();
    }

    void OnDestroy()
    {
        privOnDestroyRegisterPausible();
        singleton = null;
    }

    /// <summary>
    /// Set time scale
    /// </summary>
    /// <param name="scale">Time scale</param>
    public static void SetTimeScale(float scale)
    {
        singleton.timeScale = scale;
        if (!singleton.GamePaused)
        {
            Time.timeScale = scale;
        }
    }

    


    public void Pause()
    {
        GamePaused = true;
        Time.timeScale = 0.0f;
    }

    public void Unpause()
    {
        GamePaused = false;
        Time.timeScale = timeScale;
    }

    public void privOnAwakeRegisterPausible()
    {
        PauseManager.Subscribe(this);
    }

    public void privOnDestroyRegisterPausible()
    {
        PauseManager.Unsubscribe(this);
    }
}
