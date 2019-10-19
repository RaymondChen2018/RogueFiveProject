using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshViewPort : MonoBehaviour
{
    [SerializeField] private float width = 15.0f;
    [SerializeField] private float height = 10.0f;

    private float originRefX;
    private float originRefY;
    // Start is called before the first frame update
    void Start()
    {
        // Cell grid reference origin
        originRefX = gameObject.transform.position.x;
        originRefY = gameObject.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Convert to boundaries

        float viewPortBottom = getViewPortBottom();
        float viewPortTop = getViewPortTop();
        float viewPortLeft = getViewPortLeft();
        float viewPortRight = getViewPortRight();

        float viewPortBottomAlign = getViewPortBottomInclusive(64);
        float viewPortTopAlign = getViewPortTopInclusive(64);
        float viewPortLeftAlign = getViewPortLeftInclusive(64);
        float viewPortRightAlign = getViewPortRightInclusive(64);

        // Debug
        Vector2 topright = new Vector2(viewPortRight, viewPortTop);
        Vector2 topleft = new Vector2(viewPortLeft, viewPortTop);
        Vector2 bottomright = new Vector2(viewPortRight, viewPortBottom);
        Vector2 bottomleft = new Vector2(viewPortLeft, viewPortBottom);
        Debug.DrawLine(topright, topleft, Color.blue);
        Debug.DrawLine(topright, bottomright, Color.blue);
        Debug.DrawLine(bottomright, bottomleft, Color.blue);
        Debug.DrawLine(bottomleft, topleft, Color.blue);

        Vector2 toprightAlign = new Vector2(viewPortRightAlign, viewPortTopAlign);
        Vector2 topleftAlign = new Vector2(viewPortLeftAlign, viewPortTopAlign);
        Vector2 bottomrightAlign = new Vector2(viewPortRightAlign, viewPortBottomAlign);
        Vector2 bottomleftAlign = new Vector2(viewPortLeftAlign, viewPortBottomAlign);
        Debug.DrawLine(toprightAlign, topleftAlign, Color.green);
        Debug.DrawLine(toprightAlign, bottomrightAlign, Color.green);
        Debug.DrawLine(bottomrightAlign, bottomleftAlign, Color.green);
        Debug.DrawLine(bottomleftAlign, topleftAlign, Color.green);
    }

    public float getViewPortBottom() { return gameObject.transform.position.y - height / 2; }
    public float getViewPortTop() { return gameObject.transform.position.y + height / 2; }
    public float getViewPortLeft() { return gameObject.transform.position.x - width / 2; }
    public float getViewPortRight() { return gameObject.transform.position.x + width / 2; }


    public float getViewPortBottomInclusive(int cellDimension) {
        float cellDimensionWorld = cellDimension / 100.0f;
        float Bottom = getViewPortBottom();
        float viewPortBottomInclusive = Bottom;
        if ((Bottom - originRefY) % cellDimensionWorld != 0.0f)// If Bottom bound steps onto the grid cell
        {
            viewPortBottomInclusive = ((int)((Bottom - originRefY) / cellDimensionWorld) - 1) * cellDimensionWorld + originRefY;
        }
        return viewPortBottomInclusive;
    }

    public float getViewPortTopInclusive(int cellDimension)
    {
        float cellDimensionWorld = cellDimension / 100.0f;
        float Top = getViewPortTop();
        float viewPortTopInclusive = Top;
        if ((Top - originRefY) % cellDimensionWorld != 0.0f)// If Top bound steps onto the grid cell
        {
            viewPortTopInclusive = ((int)((Top - originRefY) / cellDimensionWorld) + 1) * cellDimensionWorld + originRefY;
        }
        return viewPortTopInclusive;
    }

    public float getViewPortLeftInclusive(int cellDimension)
    {
        float cellDimensionWorld = cellDimension / 100.0f;
        float Left = getViewPortLeft();
        float viewPortLeftInclusive = Left;
        if ((Left - originRefX) % cellDimensionWorld != 0.0f)// If Left bound steps onto the grid cell
        {
            viewPortLeftInclusive = ((int)((Left - originRefX) / cellDimensionWorld) - 1) * cellDimensionWorld + originRefX;
        }
        return viewPortLeftInclusive;
    }
    public float getViewPortRightInclusive(int cellDimension)
    {
        float cellDimensionWorld = cellDimension / 100.0f;
        float Right = getViewPortRight();
        float viewPortRightInclusive = Right;
        if ((Right - originRefX) % cellDimensionWorld != 0.0f)// If right bound steps onto the grid cell
        {
            viewPortRightInclusive = ((int)((Right - originRefX) / cellDimensionWorld) + 1) * cellDimensionWorld + originRefX;
        }
        return viewPortRightInclusive;
    }

    
}
