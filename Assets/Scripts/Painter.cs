using System;
using UnityEngine;

public static class Painter
{
    public static void PaintHit(Vector2 texCoord, TextureManager textureManager)
    {
        var pixelX = (int)(texCoord.x * textureManager.mask.width);
        var pixelY = (int)(texCoord.y * textureManager.mask.height);
        var hit = new Vector2Int(pixelX, pixelY);
        
        const int brushScale = 3;
        
        var xOff = hit.x - textureManager.brush.width / 2 * brushScale;
        var yOff = hit.y - textureManager.brush.height / 2 * brushScale;

        for (var x = 0; x < textureManager.brush.width * brushScale; x++)
        {
            for (var y = 0; y < textureManager.brush.width * brushScale; y++)
            {
                PaintPixel(x, y, xOff, yOff, brushScale, textureManager);
            }
        }
        
        textureManager.mask.Apply();
    }

    private static void PaintPixel(int x, int y, int xOff, int yOff, int brushScale, TextureManager textureManager)
    {
        var pX = Math.Clamp(xOff + x, 0, textureManager.mask.width - 1);
        var pY = Math.Clamp(yOff + y, 0, textureManager.mask.height - 1);

        var maskColor = textureManager.mask.GetPixel(pX, pY);
        if (maskColor.r == 0) return;

        var dirtColor = textureManager.brush.GetPixel(x/brushScale, y/brushScale);
        var newMaskAmount = dirtColor.r * maskColor.r;
        var pixelColor = new Color(newMaskAmount, newMaskAmount, newMaskAmount);

        textureManager.mask.SetPixel(pX, pY, pixelColor);
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
