using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonChangeScene : MonoBehaviour
{
    public void OnButtonDown(string sceneName)
    {
        // Check inputs
        if (sceneName == null || sceneName == "" || GetComponent<Button>() == null)
        {
            Debug.LogError("Invalid parameter");
        }

        SceneManager.LoadScene(sceneName);
    }
}
