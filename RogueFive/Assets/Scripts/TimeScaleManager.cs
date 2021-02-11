using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This manager manages between in-game timescale and menu pause (by PauseManager);
/// 
/// It reserves a timescale ratio even when the game is paused by the menu and time
/// scale is set to 0.0f; 
/// 
/// It will resume its timescale when PauseManager signal unpause.
/// </summary>
public class TimeScaleManager : MonoBehaviour, IPausible
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
        privOnStartRegisterPausible();
    }

    void OnDestroy()
    {
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

    public void privOnStartRegisterPausible()
    {
        PauseManager.Subscribe(this);
    }
}
