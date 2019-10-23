using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonToggle : MonoBehaviour
{
    [SerializeField] private GameObject ToggledObject = null;

    public void OnButtonDown()
    {
        if(ToggledObject == null)
        {

        }
        else
        {
            if (ToggledObject.activeSelf)
            {
                ToggledObject.SetActive(false);
            }
            else
            {
                ToggledObject.SetActive(true);
            }
        }
    }
}
