using UnityEngine;

namespace Grid
{
    public class GridBox
    {
        private readonly Vector2Int[,] points;
        private Vector2Int size;
        private bool clean;
        private const float dirtThreshold = 0.05f;
    
        public bool shouldCheckForClean;
    
        public GridBox(int xPos, int yPos, int width, int height)
        {
            size = new Vector2Int(width, height);
            points = new Vector2Int[size.x, size.y];

            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var gX = xPos * size.x + x;
                    var gY = yPos * size.y + y;
                    points[x, y] = new Vector2Int(gX, gY);
                }
            }
        }

        public bool CheckIfClean(Texture2D tex)
        {
            if (clean) return true;
            if (!shouldCheckForClean) return false;

            shouldCheckForClean = false;
            if (IsDirty(tex)) return false;

            clean = true;
            PaintCell(tex, Color.black); // TODO: This is noticeable, keep it?
            return true;
        }

        private bool IsDirty(Texture2D tex)
        {
            var dirtCount = 0;
            var dirtLimit = size.x * size.y * dirtThreshold;
        
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    if (dirtCount > dirtLimit) return true;

                    var point = points[x, y];
                    if (tex.GetPixel(point.x, point.y).r > 0) dirtCount++;
                }
            }

            return false;
        }

        private void PaintCell(Texture2D tex, Color color)
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
    }

}
