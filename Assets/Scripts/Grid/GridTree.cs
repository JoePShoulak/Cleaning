using System;
using UnityEngine;

namespace Grid
{
    public class GridTree
    {
        private readonly GridBox[,] gridBoxes;
        private Vector2Int gridSize;

        public GridTree(Texture tex, Texture brush)
        {
            var cellSize = new Vector2Int(brush.width / 2, brush.height / 2);
        
            gridSize.x = (int)Math.Ceiling(tex.width / (float)cellSize.x);
            gridSize.y = (int)Math.Ceiling(tex.height / (float)cellSize.y);
        
            gridBoxes = new GridBox[gridSize.x,gridSize.y];

            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    gridBoxes[x,y] = new GridBox(x, y,cellSize.x, cellSize.y);
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
                    var boxIsDirty = gridBoxes[x, y].CheckIfClean(_maskTex);
                    if (!boxIsDirty) dirtyCells++;
                }
            }

            return (float)dirtyCells / (gridSize.x * gridSize.y);
        }
    }

}