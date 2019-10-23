using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NavMeshViewPort : MonoBehaviour
{
    private static NavMeshViewPort singleton;

    [SerializeField] private float width = 15.0f;
    [SerializeField] private float height = 10.0f;

    // Grid Origin 
    private float originRefX;
    private float originRefY;
    private const float BOUND_MATH_TOLERANCE = 0.00001f;

    // Start is called before the first frame update
    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogError("Duplicate NavMeshViewPort!");
        }
        originRefX = gameObject.transform.position.x;
        originRefY = gameObject.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Boundaries debug
        float viewPortBottom = GetBottom();
        float viewPortTop = GetTop();
        float viewPortLeft = GetLeft();
        float viewPortRight = GetRight();
        Vector2 topright = new Vector2(viewPortRight, viewPortTop);
        Vector2 topleft = new Vector2(viewPortLeft, viewPortTop);
        Vector2 bottomright = new Vector2(viewPortRight, viewPortBottom);
        Vector2 bottomleft = new Vector2(viewPortLeft, viewPortBottom);
        Debug.DrawLine(topright, topleft, Color.blue);
        Debug.DrawLine(topright, bottomright, Color.blue);
        Debug.DrawLine(bottomright, bottomleft, Color.blue);
        Debug.DrawLine(bottomleft, topleft, Color.blue);
        // Grid-aligned Boundaries debug
        float viewPortBottomAlign = GetBottomAlign(NavMeshManager.getBaseDimension());
        float viewPortTopAlign = GetTopAlign(NavMeshManager.getBaseDimension());
        float viewPortLeftAlign = GetLeftAlign(NavMeshManager.getBaseDimension());
        float viewPortRightAlign = GetRightAlign(NavMeshManager.getBaseDimension());
        Vector2 toprightAlign = new Vector2(viewPortRightAlign, viewPortTopAlign);
        Vector2 topleftAlign = new Vector2(viewPortLeftAlign, viewPortTopAlign);
        Vector2 bottomrightAlign = new Vector2(viewPortRightAlign, viewPortBottomAlign);
        Vector2 bottomleftAlign = new Vector2(viewPortLeftAlign, viewPortBottomAlign);
        Debug.DrawLine(toprightAlign, topleftAlign, Color.green);
        Debug.DrawLine(toprightAlign, bottomrightAlign, Color.green);
        Debug.DrawLine(bottomrightAlign, bottomleftAlign, Color.green);
        Debug.DrawLine(bottomleftAlign, topleftAlign, Color.green);
    }

    void OnDestroy()
    {
        singleton = null;
    }

    public static NavMeshViewPort GetSingleton()
    {
        return singleton;
    }

    // Boundaries
    public float GetBottom() { return gameObject.transform.position.y - height / 2; }

    public float GetTop() { return gameObject.transform.position.y + height / 2; }

    public float GetLeft() { return gameObject.transform.position.x - width / 2; }

    public float GetRight() { return gameObject.transform.position.x + width / 2; }

    // Grid-aligned boundaries
    public float GetBottomAlign(int cellDimension) {
        float cellDimensionWorld = cellDimension / 100.0f;
        float Bottom = GetBottom();
        float viewPortBottomInclusive = Bottom;

        // If bound steps onto the grid cell
        if (Mathf.Abs(Bottom - originRefY) % cellDimensionWorld > BOUND_MATH_TOLERANCE)// If Bottom bound steps onto the grid cell
        {
            viewPortBottomInclusive = ((int)((Bottom - originRefY) / cellDimensionWorld)) * cellDimensionWorld + originRefY;
        }

        // Rounding adjustment
        if (Bottom < originRefY)
        {
            viewPortBottomInclusive -= cellDimensionWorld;
        }

        return viewPortBottomInclusive;
    }
    
    public float GetTopAlign(int cellDimension)
    {
        float cellDimensionWorld = cellDimension / 100.0f;
        float Top = GetTop();
        float viewPortTopInclusive = Top;

        // If bound steps onto the grid cell
        if (Mathf.Abs(Top - originRefY) % cellDimensionWorld > BOUND_MATH_TOLERANCE)// If Top bound steps onto the grid cell
        {
            viewPortTopInclusive = ((int)((Top - originRefY) / cellDimensionWorld)) * cellDimensionWorld + originRefY;
        }

        // Rounding adjustment
        if (Top > originRefY)
        {
            viewPortTopInclusive += cellDimensionWorld;
        }

        return viewPortTopInclusive;
    }

    public float GetLeftAlign(int cellDimension)
    {
        float cellDimensionWorld = cellDimension / 100.0f;
        float Left = GetLeft();
        float viewPortLeftInclusive = Left;

        // If bound steps onto the grid cell
        if (Mathf.Abs(Left - originRefX) % cellDimensionWorld > BOUND_MATH_TOLERANCE)// If Left bound steps onto the grid cell
        {
            viewPortLeftInclusive = ((int)((Left - originRefX) / cellDimensionWorld)) * cellDimensionWorld + originRefX;
        }

        // Rounding adjustment
        if (Left < originRefX)
        {
            viewPortLeftInclusive -= cellDimensionWorld;
        }

        return viewPortLeftInclusive;
    }

    public float GetRightAlign(int cellDimension)
    {
        float cellDimensionWorld = cellDimension / 100.0f;
        float Right = GetRight();
        float viewPortRightInclusive = Right;

        // If bound steps onto the grid cell
        if (Mathf.Abs(Right - originRefX) % cellDimensionWorld > BOUND_MATH_TOLERANCE)
        {
            viewPortRightInclusive = ((int)((Right - originRefX) / cellDimensionWorld)) * cellDimensionWorld + originRefX;
        }

        // Rounding adjustment
        if(Right > originRefX)
        {
            viewPortRightInclusive += cellDimensionWorld;
        }

        return viewPortRightInclusive;
    }
}
