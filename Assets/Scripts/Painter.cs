using System;
using UnityEngine;

public sealed class Painter : MonoBehaviour
{
    [SerializeField] private Texture2D brush;

    public void PaintHit(Vector2 texCoord, Texture2D destination)
    {
        var pixelX = (int)(texCoord.x * destination.width);
        var pixelY = (int)(texCoord.y * destination.height);
        var hit = new Vector2Int(pixelX, pixelY);
        
        const int brushScale = 3;
        
        var xOff = hit.x - brush.width / 2 * brushScale;
        var yOff = hit.y - brush.height / 2 * brushScale;

        for (var x = 0; x < brush.width * brushScale; x++)
        {
            for (var y = 0; y < brush.width * brushScale; y++)
            {
                PaintPixel(x, y, xOff, yOff, brushScale, destination);
            }
        }
        
        destination.Apply();
    }

    private void PaintPixel(int x, int y, int xOff, int yOff, int brushScale, Texture2D destination)
    {
        var pX = Math.Clamp(xOff + x, 0, destination.width);
        var pY = Math.Clamp(yOff + y, 0, destination.height);
                
        var maskColor = destination.GetPixel(pX, pY);
        if (maskColor.r == 0) return;
                
        var dirtColor = brush.GetPixel(x/brushScale, y/brushScale);
        var channelValue = dirtColor.r * maskColor.r;
        var pixelColor = new Color(channelValue, channelValue, channelValue);
                
        destination.SetPixel(pX, pY, pixelColor);
    }
}
