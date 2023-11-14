using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaningController : MonoBehaviour
{
    private Camera cam;
    private Texture2D dirtMaskTexture;
    [SerializeField] private Texture2D brush;
    private static readonly int MaskID = Shader.PropertyToID("_mask");
    [SerializeField] private float pollingRate = 0.1f;
    [SerializeField] private int samplingFactor = 10;
    private List<Vector2Int> dirtyPoints;
    private int totalDirt;
    private bool clean;

    public void Start()
    {
        cam = Camera.main;
        
        var material = GetComponent<MeshRenderer>().material;
        var originalTexture = (Texture2D)material.GetTexture(MaskID);
        CopyTexture(originalTexture, out dirtMaskTexture);
        
        material.SetTexture(MaskID, dirtMaskTexture);

        dirtyPoints = new List<Vector2Int>();

        for (var x = 0; x < dirtMaskTexture.width; x+=samplingFactor)
        {
            for (var y = 0; y < dirtMaskTexture.height; y+=samplingFactor)
            {
                dirtyPoints.Add(new Vector2Int(x, y));
            }
        }

        totalDirt = dirtyPoints.Count;

        StartCoroutine(Polling());
    }

    private void Update()
    {
        if (clean) return;
        
        var hit = CheckForHit();
        if (hit == null) return;

        PaintHit((Vector2Int)hit);
    }

    IEnumerator Polling()
    {
        do
        {
            clean = CheckIfClean();
            yield return new WaitForSecondsRealtime(pollingRate);
        } while (clean == false);
    }

    private void CopyTexture(Texture2D source, out Texture2D destination)
    {
        destination = new Texture2D(source.width, source.height);
        destination.SetPixels(source.GetPixels());
        destination.Apply();
    }

    private bool CheckIfClean()
    {
        dirtyPoints.RemoveAll(point => dirtMaskTexture.GetPixel(point.x, point.y).r == 0);
        var dirtFraction = dirtyPoints.Count / (float)totalDirt;
        
        Debug.Log(dirtFraction);
        return dirtFraction < .01;
    }

    private void PaintHit(Vector2Int hit)
    {
        const int brushScale = 3;
        
        var pixelXOffset = hit.x - brush.width / 2 * brushScale;
        var pixelYOffset = hit.y - brush.height / 2 * brushScale;

        for (var x = 0; x < brush.width * brushScale; x++)
        {
            for (var y = 0; y < brush.width * brushScale; y++)
            {
                

                var pX = Math.Clamp(pixelXOffset + x, 0, dirtMaskTexture.width);
                var pY = Math.Clamp(pixelYOffset + y, 0, dirtMaskTexture.height);
                
                var dirtColor = brush.GetPixel(x/brushScale, y/brushScale);
                var maskColor = dirtMaskTexture.GetPixel(pX, pY);
                var channelValue = dirtColor.r * maskColor.r;
                if (maskColor.r == 0) continue;
                
                PaintPixel(pX, pY, channelValue);
            }
        }
        
        dirtMaskTexture.Apply();
    }

    private void PaintPixel(int x, int y, float color)
    {
        var pixelColor = new Color(color, color, color);
                
        dirtMaskTexture.SetPixel(x, y, pixelColor);
    }

    private Vector2Int? CheckForHit()
    {
        Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var raycastHit);
        if (raycastHit.collider == null) return null;
        
        var textureCoord = raycastHit.textureCoord;
            
        var pixelX = (int)(textureCoord.x * dirtMaskTexture.width);
        var pixelY = (int)(textureCoord.y * dirtMaskTexture.height);
            
        var paintPixelPosition = new Vector2Int(pixelX, pixelY);

        return paintPixelPosition;
    }
}
