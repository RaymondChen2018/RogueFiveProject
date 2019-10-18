using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager_Helper : MonoBehaviour
{
    /// <summary>
    /// Pause action
    /// </summary>
    /// <param name="pause"> Pause = true; Unpause = false.</param>
    public void Pause(bool pause)
    {
        PauseManager.Pause(pause);
    }
}
