using System;
using UnityEngine;

namespace TextureCleaning
{
    public static class Painter
    {
        public static void PaintHit(Vector2 texCoord, Texture2D brush, Texture2D mask)
        {
            var pixelX = (int)(texCoord.x * mask.width);
            var pixelY = (int)(texCoord.y * mask.height);
            var hit = new Vector2Int(pixelX, pixelY);
        
            var xOff = hit.x - brush.width / 2;
            var yOff = hit.y - brush.height / 2;

            for (var x = 0; x < brush.width; x++)
            {
                for (var y = 0; y < brush.width; y++)
                {
                    PaintPixel(x, y, xOff, yOff, brush, mask);
                }
            }
        
            mask.Apply();
        }

        private static void PaintPixel(int x, int y, int xOff, int yOff, Texture2D brush, Texture2D mask)
        {
            var pX = Math.Clamp(xOff + x, 0, mask.width - 1);
            var pY = Math.Clamp(yOff + y, 0, mask.height - 1);

            var maskColor = mask.GetPixel(pX, pY);
            if (maskColor.r == 0) return;

            var dirtColor = brush.GetPixel(x, y);
            var newMaskAmount = dirtColor.r * maskColor.r;
            var pixelColor = new Color(newMaskAmount, newMaskAmount, newMaskAmount);

            mask.SetPixel(pX, pY, pixelColor);
        }

        public static void Fill(TextureManager textureManager, Color color)
        {
            for (var x = 0; x < textureManager.mask.width; x++)
            {
                for (var y = 0; y < textureManager.mask.height; y++)
                {
                    textureManager.mask.SetPixel(x, y, color);
                }
            }
        }
    }
}
