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
                PaintPixel(x, y, xOff, yOff, brushScale, textureManager.mask, textureManager.brush);
            }
        }
        
        textureManager.mask.Apply();
    }

    private static void PaintPixel(int x, int y, int xOff, int yOff, int brushScale, Texture2D destination, Texture2D brush)
    {
        var pX = Math.Clamp(xOff + x, 0, destination.width - 1);
        var pY = Math.Clamp(yOff + y, 0, destination.height - 1);
                
        var maskColor = destination.GetPixel(pX, pY);
        if (maskColor.r == 0) return;
                
        var dirtColor = brush.GetPixel(x/brushScale, y/brushScale);
        var channelValue = dirtColor.r * maskColor.r;
        var pixelColor = new Color(channelValue, channelValue, channelValue);
                
        destination.SetPixel(pX, pY, pixelColor);
    }
}
