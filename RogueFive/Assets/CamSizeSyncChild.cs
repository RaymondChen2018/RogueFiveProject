using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSizeSyncChild : MonoBehaviour
{
    Camera thisCam;
    Camera[] childrenCams;

    // Start is called before the first frame update
    void Start()
    {
        thisCam = GetComponent<Camera>();
        childrenCams = GetComponentsInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i< childrenCams.Length;i++)
        {
            childrenCams[i].orthographicSize = thisCam.orthographicSize;
        }
    }
}
