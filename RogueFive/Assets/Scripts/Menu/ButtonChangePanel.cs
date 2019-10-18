using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChangePanel : MonoBehaviour
{
    [SerializeField] private GameObject SwitchToObject = null;
    [SerializeField] private GameObject TurnOffObject = null;
    public void OnButtonDown()
    {
        // Check inputs
        if(SwitchToObject == null || TurnOffObject == null || GetComponent<Button>() == null)
        {
            Debug.LogError("Invalid parameter");
        }

        SwitchToObject.SetActive(true);
        TurnOffObject.SetActive(false);
    }
}
