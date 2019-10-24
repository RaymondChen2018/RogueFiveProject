using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for generating/pooling cells;
/// Calling navmesh procedure component;
/// </summary>
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
    [SerializeField] private int splitIteration = 2;
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
        privGenerateCells(vp_topAlignPrev, vp_bottomAlignPrev, vp_leftAlignPrev, vp_rightAlignPrev);
    }

    public static ref List<NavMeshCell> GetCellListRef()
    {
        return ref singleton.cells;
    }

    // Update is called once per frame
    void FixedUpdate()
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
            privGenerateCells(vp_topAlign, vp_bottomAlign, vp_leftAlign, vp_rightAlign);


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

    private void privGenerateCells(float top, float bottom, float left, float right)
    {
        // Clear out-of-bound cells
        for (int i = 0; i < cells.Count; i++)
        {
            if (!cells[i].IsContained(top, bottom, left, right))
            {
                privDespawnCell(cells[i]);
                i--;
            }
        }

        // Fill cells
        float cellDimWorld = baseCellDim / 100.0f;
        int numCell_width = Mathf.RoundToInt((right - left) / cellDimWorld);
        int numCell_height = Mathf.RoundToInt((top - bottom) / cellDimWorld);
        int totalNumCells = numCell_height * numCell_width;
        for (int i = 0; i < numCell_width; i++)
        {
            for(int j = 0; j < numCell_height; j++)
            {
                // Check standard base cell across viewport
                Vector2 cellPos = new Vector2(left + i * cellDimWorld + cellDimWorld / 2.0f, top - j * cellDimWorld - cellDimWorld / 2.0f);
                Vector2 cellSize = new Vector2(cellDimWorld - CELL_DETECT_GAP_TOLERANCE, cellDimWorld - CELL_DETECT_GAP_TOLERANCE);

                // Empty cell area
                if (Physics2D.OverlapBox(cellPos, cellSize, 0.0f, cellLayerMask) == null)
                {
                    // Spawn cell
                    NavMeshCell newCell_cell = privSpawnCell(baseCellDim, cellPos);

                    // Cell block by terrain, split
                    if (Physics2D.OverlapBox(cellPos, cellSize, 0.0f, terrainLayerMask) != null)
                    {
                        InterativeSplit(newCell_cell, splitIteration);
                    }
                }
            }
        }

        // Split cells
        
    }

    private void InterativeSplit(NavMeshCell cellToSplit, int splitIterations)
    {
        if(splitIterations == 0 || cellToSplit.GetCurrDimension() <= 2)
        {
            // Can't split further, Mark red
            privDespawnCell(cellToSplit);
        }
        else
        {
            float cellX = cellToSplit.transform.position.x;
            float cellY = cellToSplit.transform.position.y;
            int currDimension = cellToSplit.GetCurrDimension();
            int childCellDimension = currDimension / 2;
            float posOffset = currDimension / 400.0f;
            Vector2 childCellPos = new Vector2();
            Vector2 boxCastSize = new Vector2(childCellDimension / 100.0f - CELL_DETECT_GAP_TOLERANCE, childCellDimension / 100.0f - CELL_DETECT_GAP_TOLERANCE);
            Collider2D hitCell;
            NavMeshCell cell1 = null, cell2 = null, cell3 = null, cell4 = null;

            privDespawnCell(cellToSplit);

            // To-do get rid of the rounding error by using different scale grid

            // Top-left
            childCellPos = new Vector2(cellX - posOffset, cellY + posOffset);
            cell1 = privSpawnCell(childCellDimension, childCellPos);
            hitCell = Physics2D.OverlapBox(childCellPos, boxCastSize, 0.0f, terrainLayerMask);
            if (hitCell != null){ InterativeSplit(cell1, splitIterations - 1); }

            // Top-right
            childCellPos = new Vector2(cellX + posOffset, cellY + posOffset);
            cell2 = privSpawnCell(childCellDimension, childCellPos);
            hitCell = Physics2D.OverlapBox(childCellPos, boxCastSize, 0.0f, terrainLayerMask);
            if (hitCell != null) { InterativeSplit(cell2, splitIterations - 1); }

            // Bottom-right
            childCellPos = new Vector2(cellX + posOffset, cellY - posOffset);
            cell3 = privSpawnCell(childCellDimension, childCellPos);
            hitCell = Physics2D.OverlapBox(childCellPos, boxCastSize, 0.0f, terrainLayerMask);
            if (hitCell != null) { InterativeSplit(cell3, splitIterations - 1); }

            // Bottom-left
            childCellPos = new Vector2(cellX - posOffset, cellY - posOffset);
            cell4 = privSpawnCell(childCellDimension, childCellPos);
            hitCell = Physics2D.OverlapBox(childCellPos, boxCastSize, 0.0f, terrainLayerMask);
            if (hitCell != null) { InterativeSplit(cell4, splitIterations - 1); }

        }
    }

    // Despawn a cell and put into pool
    private void privDespawnCell(NavMeshCell cell)
    {
        cell.gameObject.SetActive(false);
        cellsPool.Add(cell);
        cells.Remove(cell);
    }

    // Instantiate/unpool a cell for use
    private NavMeshCell privSpawnCell(int dimension, Vector2 cellPos)
    {
        NavMeshCell newCell = null;
        if (cellsPool.Count > 0) // Fetch from pool
        {
            newCell = cellsPool[0];
            newCell.gameObject.SetActive(true);
            newCell.transform.position = cellPos;
            cellsPool.RemoveAt(0);
        }
        else // Pool dry
        {
            GameObject newCellObj = Instantiate(NavMeshTemplate, cellPos, Quaternion.identity, poolBranch);
            newCell = newCellObj.GetComponent<NavMeshCell>();
        }
        newCell.SetDimension(dimension);
        cells.Add(newCell);

        newCell.GetComponent<SpriteRenderer>().color = Color.white;

        return newCell;
    }

    
}
