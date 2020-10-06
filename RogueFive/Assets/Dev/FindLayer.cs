using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindLayer : MonoBehaviour
{
    [SerializeField] private string layerName = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int layer = LayerMask.NameToLayer(layerName);

        GameObject[] allGameObjects =  FindObjectsOfType<GameObject>();

        foreach(GameObject obj in allGameObjects)
        {
            if(obj.layer == layer)
            {
                Debug.Log(obj.name + " is on layer "+ layerName);
            }
        }
    }
}
