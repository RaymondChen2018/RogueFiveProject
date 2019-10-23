using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    private static NavMeshManager singleton;
    public const int DEFAULT_CELL_DIMENSION = 64;

    // Cell collision layer for the use of detecting existing filled cells
    [SerializeField] private string navMeshLayerName = "NavMesh";
    [SerializeField] private string terrainTestLayerName = "Terrain";

    // The cell prefab
    [SerializeField] private GameObject NavMeshTemplate;

    // The viewport zone to generate cells within
    private NavMeshViewPort viewPort;

    // Pool objects' parent branch
    [SerializeField] private Transform poolBranch;

    
    private List<NavMeshCell> cells = new List<NavMeshCell>();
    private List<NavMeshCell> cellsPool = new List<NavMeshCell>();
    private LayerMask cellLayerMask;
    private LayerMask terrainLayerMask;
    [SerializeField] private int baseCellDim = 64;
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
        viewPort = NavMeshViewPort.GetSingleton();
        cellLayerMask = LayerMask.GetMask(navMeshLayerName);
        terrainLayerMask = LayerMask.GetMask(terrainTestLayerName);
        vp_rightAlignPrev = viewPort.GetRightAlign(baseCellDim);
        vp_leftAlignPrev = viewPort.GetLeftAlign(baseCellDim);
        vp_topAlignPrev = viewPort.GetTopAlign(baseCellDim);
        vp_bottomAlignPrev = viewPort.GetBottomAlign(baseCellDim);
        privFillCells();
    }

    // Update is called once per frame
    void Update()
    {
        float vp_rightAlign = viewPort.GetRightAlign(baseCellDim);
        float vp_leftAlign = viewPort.GetLeftAlign(baseCellDim);
        float vp_topAlign = viewPort.GetTopAlign(baseCellDim);
        float vp_bottomAlign = viewPort.GetBottomAlign(baseCellDim);

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

    void OnDestroy()
    {
        singleton = null;
    }


    public static int getBaseDimension()
    {
        return singleton.baseCellDim;
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
                //Destroy(cells[i].gameObject);
                cells[i].gameObject.SetActive(false);
                cellsPool.Add(cells[i]);
                cells.RemoveAt(i);
                i--;
            }
        }

        // Fill cells
        float cellDimWorld = baseCellDim / 100.0f;
        float vp_rightAlign = viewPort.GetRightAlign(baseCellDim);
        float vp_leftAlign = viewPort.GetLeftAlign(baseCellDim);
        float vp_topAlign = viewPort.GetTopAlign(baseCellDim);
        float vp_bottomAlign = viewPort.GetBottomAlign(baseCellDim);
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

                    // Spawn cell
                    GameObject newCell = null;
                    NavMeshCell newCell_cell = null;
                    if (cellsPool.Count > 0) // Fetch from pool
                    {
                        newCell_cell = cellsPool[0];
                        newCell = newCell_cell.gameObject;
                        newCell.SetActive(true);
                        newCell.transform.position = cellPos;
                        cellsPool.RemoveAt(0);
                    }
                    else // Pool dry
                    {
                        newCell = Instantiate(NavMeshTemplate, cellPos, Quaternion.identity, poolBranch);
                        newCell_cell = newCell.GetComponent<NavMeshCell>();
                    }
                    cells.Add(newCell_cell);
                    newCell_cell.SetDimension(baseCellDim);
                    newCell.GetComponent<SpriteRenderer>().color = cellColor;
                }
            }
        }
    }
}
