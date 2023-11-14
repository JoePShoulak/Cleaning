using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CleaningController : MonoBehaviour
{
    private Camera cam;
    
    [SerializeField] private float pollingRate = 0.1f;
    [SerializeField] private int samplingFactor = 10;
    [SerializeField] private Texture2D brush;
    
    private static readonly int MaskID = Shader.PropertyToID("_mask");
    private Texture2D dirtMaskTexture;
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
        dirtyPoints = SamplePoints(dirtMaskTexture);
        totalDirt = dirtyPoints.Count;

        StartCoroutine(MonitorCleanStatus());
    }

    private List<Vector2Int> SamplePoints(Texture tex)
    {
        var points = new List<Vector2Int>();

        for (var x = 0; x < tex.width; x+=samplingFactor)
        {
            for (var y = 0; y < tex.height; y+=samplingFactor)
            {
                points.Add(new Vector2Int(x, y));
            }
        }

        return points;
    }

    private void Update()
    {
        if (clean) return;
        
        var hit = CheckForHit();
        if (hit == null) return;

        PaintHit((Vector2Int)hit);
    }

    private IEnumerator MonitorCleanStatus()
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
        
        var xOff = hit.x - brush.width / 2 * brushScale;
        var yOff = hit.y - brush.height / 2 * brushScale;

        for (var x = 0; x < brush.width * brushScale; x++)
        {
            for (var y = 0; y < brush.width * brushScale; y++)
            {
                PaintPixel(x, y, xOff, yOff, brushScale);
            }
        }
        
        dirtMaskTexture.Apply();
    }

    private void PaintPixel(int x, int y, int xOff, int yOff, int brushScale)
    {
        var pX = Math.Clamp(xOff + x, 0, dirtMaskTexture.width);
        var pY = Math.Clamp(yOff + y, 0, dirtMaskTexture.height);
                
        var maskColor = dirtMaskTexture.GetPixel(pX, pY);
        if (maskColor.r == 0) return;
                
        var dirtColor = brush.GetPixel(x/brushScale, y/brushScale);
        var channelValue = dirtColor.r * maskColor.r;
        var pixelColor = new Color(channelValue, channelValue, channelValue);
                
        dirtMaskTexture.SetPixel(pX, pY, pixelColor);
    }

    private Vector2Int? CheckForHit()
    {
        Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var raycastHit);
        if (raycastHit.collider == null) return null;
            
        var textureCoord = raycastHit.textureCoord;
        var pixelX = (int)(textureCoord.x * dirtMaskTexture.width);
        var pixelY = (int)(textureCoord.y * dirtMaskTexture.height);
            
        return new Vector2Int(pixelX, pixelY);
    }
}
