using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] private GameObject NavMeshTemplate;
    [SerializeField] private NavMeshViewPort viewPort;
    [SerializeField] private string navMeshLayerName = "NavMesh";
    private List<NavMeshCell> cells = new List<NavMeshCell>();
    private LayerMask cellLayer;
    private int cellDim = 64;
    // Start is called before the first frame update
    void Start()
    {
        // Cell collision layer
        cellLayer = LayerMask.NameToLayer(navMeshLayerName);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void privFillCells()
    {
        // Get viewport
        float viewPortBottom = viewPort.getViewPortBottom();
        float viewPortTop = viewPort.getViewPortTop();
        float viewPortLeft = viewPort.getViewPortLeft();
        float viewPortRight = viewPort.getViewPortRight();

        // Clear out-of-bound cells
        for (int i = 0; i < cells.Count; i++)
        {
            if (!cells[i].IsContained(viewPortTop, viewPortBottom, viewPortLeft, viewPortRight))
            {
                cells.RemoveAt(i);
                i--;
            }
        }

        // Fill cells
        //float remainder;

        //float viewPortRightInclusive;
        //remainder = Mathf.Abs(viewPortRight - originRefX) % cellDim;
        //if (remainder > 0.0f)// If right bound steps onto the grid cell
        //{
        //    viewPortRightInclusive = viewPortRight + (cellDim - remainder);
        //}
        

        //int cellWidth = 
    }
}
