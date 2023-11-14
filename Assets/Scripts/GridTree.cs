using System;
using UnityEngine;

public class GridTree
{
    private GridBox[,] gridBoxes;
    private Texture2D tex;
    private Vector2Int gridSize;

    public GridTree(Texture2D _tex, Texture2D brush)
    {
        tex = _tex;
        
        gridSize.x = (int)Math.Ceiling(tex.width / (brush.width/2f));
        gridSize.y = (int)Math.Ceiling(tex.height / (brush.height/2f));
        
        gridBoxes = new GridBox[gridSize.x,gridSize.y];
        Debug.Log("Height " + gridSize.y + ", Width " + gridSize.x);

        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                gridBoxes[x,y] = new GridBox(x, y,brush.width/2, brush.height/2);
                gridBoxes[x,y].PaintBox(tex, new Color(x/(float)gridSize.x, y/(float)gridSize.y, 0.5f));
            }
        }
    }

    public void RegisterHit(Vector2 texCoords)
    {
        var x = (int)Math.Floor(texCoords.x * gridSize.x);
        var y = (int)Math.Floor(texCoords.y * gridSize.y);

        for (var _x = x - 1; _x <= x + 1; _x++)
        { 
            for (var _y = y - 1; _y <= y + 1; _y++)
            {
                if (_x >= 0 && _x < gridSize.x && _y >= 0 && _y < gridSize.y)
                {
                    gridBoxes[_x, _y].shouldCheckForClean = true;
                }
            }
        }
        
    }

    public float GetDirtyFraction(Texture2D _maskTex)
    {
        var dirtyCells = 0;
        
        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                var box = gridBoxes[x, y];
                
                if (!box.CheckIfClean(_maskTex, tex)) dirtyCells++;
            }
        }

        return (float)dirtyCells / (gridSize.x * gridSize.y);
    }
}

public class GridBox
{
    private Vector2Int[,] points;
    private Vector2Int position;
    private Vector2Int size;
    private bool clean;
    public bool shouldCheckForClean;
    
    private const float dirtThreshold = 0.05f;
    
    public GridBox(int _x, int _y, int _width, int _height)
    {
        position = new Vector2Int(_x, _y);
        size = new Vector2Int(_width, _height);
        points = new Vector2Int[size.x, size.y];

        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                points[x, y] = new Vector2Int(position.x * size.x + x, position.y * size.y + y);
            }
        }
    }

    public void PaintBox(Texture2D tex, Color color)
    {
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                var point = points[x, y];
                
                tex.SetPixel(point.x, point.y, color);
            }
        }
        
        tex.Apply();
    }

    private bool IsDirty(Texture2D _maskTex)
    {
        var dirtCount = 0;
        var dirtLimit = size.x * size.y * dirtThreshold;
        
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                if (dirtCount > dirtLimit) return true;

                var point = points[x, y];
                if (_maskTex.GetPixel(point.x, point.y).r > 0) dirtCount++;
            }
        }

        return false;
    }

    public bool CheckIfClean(Texture2D _maskTex, Texture2D _displayTex)
    {
        if (clean) return true;

        if (!shouldCheckForClean) return false;

        shouldCheckForClean = false;
        if (IsDirty(_maskTex)) return false;

        clean = true;
        PaintBox(_displayTex, Color.white);
        
        return true;
    }
}
