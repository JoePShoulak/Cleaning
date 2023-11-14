using System;
using System.Collections;
using UnityEngine;

public class CleaningController : MonoBehaviour
{
    private Camera cam;
    private Texture2D dirtMaskTexture;
    [SerializeField] private Texture2D brush;
    private static readonly int MaskID = Shader.PropertyToID("_mask");
    [SerializeField] private float pollingRate = 0.1f;

    private float startingDirtAmount;

    public void Start()
    {
        cam = Camera.main;
        
        var material = GetComponent<MeshRenderer>().material;
        var originalTexture = (Texture2D)material.GetTexture(MaskID);
        CopyTexture(originalTexture, out dirtMaskTexture);
        
        material.SetTexture(MaskID, dirtMaskTexture);

        startingDirtAmount = GetDirtAmount(originalTexture);

        StartCoroutine(Polling());
    }

    IEnumerator Polling()
    {
        bool clean;
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

    private static float GetDirtAmount(Texture2D texture)
    {
        var dirt = 0f;
        for (var x = 0; x < texture.width; x++)
        {
            for (var y = 0; y < texture.height; y++)
            {
                dirt += texture.GetPixel(x, y).r;
            }
        }

        return dirt;
    }

    private void Update()
    {
        var hit = CheckForHit();
        if (hit == null) return;

        PaintHit((Vector2Int)hit);

        // CheckIfClean();
    }

    private bool CheckIfClean()
    {
        var currentDirt = GetDirtAmount(dirtMaskTexture);

        var dirtFraction = currentDirt / startingDirtAmount;
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
                var pixelDirt = brush.GetPixel(x/brushScale, y/brushScale);
                var pixelDirtMask = dirtMaskTexture.GetPixel(pixelXOffset + x, pixelYOffset + y);
                
                var channelValue = pixelDirt.g * pixelDirtMask.g;
                var pixelColor = new Color(channelValue, channelValue, channelValue);

                var pX = Math.Clamp(pixelXOffset + x, 0, dirtMaskTexture.width);
                var pY = Math.Clamp(pixelYOffset + y, 0, dirtMaskTexture.height);
                
                dirtMaskTexture.SetPixel(pX, pY, pixelColor);
            }
        }
        
        dirtMaskTexture.Apply();
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
