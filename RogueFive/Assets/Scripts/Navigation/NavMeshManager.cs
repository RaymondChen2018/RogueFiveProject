using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    private NavMeshManager singleton;


    // Cell collision layer for the use of detecting existing filled cells
    [SerializeField] private string navMeshLayerName = "NavMesh";
    [SerializeField] private string terrainTestLayerName = "terrainTest";

    // The cell prefab
    [SerializeField] private GameObject NavMeshTemplate;

    // The viewport zone to generate cells within
    [SerializeField] private NavMeshViewPort viewPort;

    
    private List<NavMeshCell> cells = new List<NavMeshCell>();
    private LayerMask cellLayerMask;
    private LayerMask terrainLayerMask;
    private int cellDim = 64;
    private const float CELL_DETECT_GAP_TOLERANCE = 0.05f;

    // Previous grid-aligned boundaries
    private float vp_rightAlignPrev;
    private float vp_leftAlignPrev;
    private float vp_topAlignPrev;
    private float vp_bottomAlignPrev;

    void Awake()
    {
        if(singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogError("Duplicate NavMeshManager!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cellLayerMask = LayerMask.GetMask(navMeshLayerName);
        terrainLayerMask = LayerMask.GetMask(terrainTestLayerName);
        vp_rightAlignPrev = viewPort.GetRightAlign(cellDim);
        vp_leftAlignPrev = viewPort.GetLeftAlign(cellDim);
        vp_topAlignPrev = viewPort.GetTopAlign(cellDim);
        vp_bottomAlignPrev = viewPort.GetBottomAlign(cellDim);
        privFillCells();
    }

    // Update is called once per frame
    void Update()
    {
        float vp_rightAlign = viewPort.GetRightAlign(cellDim);
        float vp_leftAlign = viewPort.GetLeftAlign(cellDim);
        float vp_topAlign = viewPort.GetTopAlign(cellDim);
        float vp_bottomAlign = viewPort.GetBottomAlign(cellDim);

        // Whether or not the viewport has change its zone area
        if (vp_rightAlignPrev != vp_rightAlign ||
            vp_leftAlignPrev != vp_leftAlign ||
            vp_topAlignPrev != vp_topAlign ||
            vp_bottomAlignPrev != vp_bottomAlign)
        {
            privFillCells();
            vp_rightAlignPrev = vp_rightAlign;
            vp_leftAlignPrev = vp_leftAlign;
            vp_topAlignPrev = vp_topAlign;
            vp_bottomAlignPrev = vp_bottomAlign;
        }
    }

    private void privFillCells()
    {
        // Get viewport
        float viewPortBottom = viewPort.GetBottom();
        float viewPortTop = viewPort.GetTop();
        float viewPortLeft = viewPort.GetLeft();
        float viewPortRight = viewPort.GetRight();

        // Clear out-of-bound cells
        for (int i = 0; i < cells.Count; i++)
        {
            if (!cells[i].IsContained(viewPortTop, viewPortBottom, viewPortLeft, viewPortRight))
            {
                Destroy(cells[i].gameObject);
                cells.RemoveAt(i);
                i--;
            }
        }

        // Fill cells
        float cellDimWorld = cellDim / 100.0f;
        float vp_rightAlign = viewPort.GetRightAlign(cellDim);
        float vp_leftAlign = viewPort.GetLeftAlign(cellDim);
        float vp_topAlign = viewPort.GetTopAlign(cellDim);
        float vp_bottomAlign = viewPort.GetBottomAlign(cellDim);
        int numCell_width = Mathf.RoundToInt((vp_rightAlign - vp_leftAlign) / cellDimWorld);
        int numCell_height = Mathf.RoundToInt((vp_topAlign - vp_bottomAlign) / cellDimWorld);
        int totalNumCells = numCell_height * numCell_width;
        for (int i = 0; i < numCell_width; i++)
        {
            for(int j = 0; j < numCell_height; j++)
            {
                // Cell position in world
                Vector2 cellPos = new Vector2(vp_leftAlign + i * cellDimWorld + cellDimWorld / 2.0f, vp_topAlign - j * cellDimWorld - cellDimWorld / 2.0f);
                Vector2 cellSize = new Vector2(cellDimWorld - CELL_DETECT_GAP_TOLERANCE, cellDimWorld - CELL_DETECT_GAP_TOLERANCE);

                // Spawn a new cell if not present
                Collider2D hitCell = Physics2D.OverlapBox(cellPos, cellSize, 0.0f, cellLayerMask);
                if (hitCell == null)
                {
                    // Instantiate color
                    Color cellColor = Color.white;

                    // Detect terrain collision
                    Collider2D hitTerrain = Physics2D.OverlapBox(cellPos, cellSize, 0.0f, terrainLayerMask);
                    if(hitTerrain != null)
                    {
                        cellColor = Color.red;
                    }

                    // Instantiate cell
                    GameObject newCell = Instantiate(NavMeshTemplate, cellPos, Quaternion.identity);
                    newCell.GetComponent<SpriteRenderer>().color = cellColor;
                    cells.Add(newCell.GetComponent<NavMeshCell>());
                }
            }
        }
    }
}
