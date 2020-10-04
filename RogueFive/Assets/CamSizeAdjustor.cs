using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSizeAdjustor : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private float maxSize = 25;
    [SerializeField] private float minSize = 10;
    [SerializeField] private float scaleSpeed = 5;
    [Tooltip("If 1.0f, stick to exactly the room size")][SerializeField] private float extraSpaceRatio = 1.5f;
    private float currSize = 10;
    private bool prevZoomDown = false;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float scaleAmount = Time.deltaTime * scaleSpeed;

        // Scan room
        Collider2D hit = Physics2D.OverlapCircle(transform.position, currSize / extraSpaceRatio, terrainLayer);
        // Zoom down
        if(hit)
        {
            currSize -= scaleAmount;
            prevZoomDown = true;
        }
        // Zoom up
        else
        {
            currSize += scaleAmount;
            prevZoomDown = false;
        }
        currSize = Mathf.Clamp(currSize, minSize, maxSize);

        cam.orthographicSize = currSize;
    }
}
