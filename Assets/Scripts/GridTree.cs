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
        
        gridSize.x = (int)Math.Ceiling(tex.width / (float)brush.width);
        gridSize.y = (int)Math.Ceiling(tex.height / (float)brush.height);
        
        gridBoxes = new GridBox[gridSize.x,gridSize.y];
        Debug.Log("Height " + gridSize.y + ", Width " + gridSize.x);

        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                gridBoxes[x,y] = new GridBox(x, y, brush.width, brush.height);
                gridBoxes[x,y].PaintBox(tex, new Color(x/(float)gridSize.x, y/(float)gridSize.y, 0.5f));
            }
        }
    }

    public bool CheckIfClean(Texture2D _maskTex, Texture2D _displayTex)
    {
        var anyDirty = false;
        
        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                if (!gridBoxes[x, y].CheckIfClean(_maskTex, _displayTex)) anyDirty = true;
            }
        }

        return !anyDirty;
    }
}

public class GridBox
{
    private Vector2Int[,] points;
    private Vector2Int position;
    private Vector2Int size;
    private bool clean;
    
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

    public bool CheckIfClean(Texture2D _maskTex, Texture2D _displayTex)
    {
        var anyDirty = false;
        
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                var point = points[x, y];
                
                if (_maskTex.GetPixel(point.x, point.y).r > 0) anyDirty = true;
            }
        } 
        
        clean = !anyDirty;

        if (clean)
        {
            PaintBox(_displayTex, Color.white);
            Debug.Log("Cell is clean!");
        }
        
        return clean;
    }

 
}
