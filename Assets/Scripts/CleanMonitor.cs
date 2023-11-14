using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CleanMonitor : MonoBehaviour
{
    [SerializeField] private float pollingRate = 0.1f;
    [SerializeField] private int samplingFactor = 10;

    private TextureManager textureManager;
    private List<Vector2Int> dirtyPoints;
    private int totalDirt;
    private bool clean;
    
    public void Start()
    {
        textureManager = GetComponent<TextureManager>();
        dirtyPoints = TextureManager.SamplePoints(textureManager.mask, samplingFactor);
        totalDirt = dirtyPoints.Count;

        StartCoroutine(MonitorCleanStatus());

        RaycastListener.onRaycastHit += point => { Painter.PaintHit(point, textureManager); };
    }

    private IEnumerator MonitorCleanStatus()
    {
        for (;;)
        {
            clean = CheckIfClean();
            if (clean) break;
            
            yield return new WaitForSecondsRealtime(pollingRate);
        }
        
        Cleanup();
    }

    private bool CheckIfClean()
    {
        dirtyPoints.RemoveAll(point => textureManager.mask.GetPixel(point.x, point.y).r == 0);
        var dirtFraction = dirtyPoints.Count / (float)totalDirt;
        
        Debug.Log(dirtFraction);
        return dirtFraction < .01;
    }

    private void Cleanup()
    {
        Painter.Fill(textureManager, Color.black);
        Debug.Log("Item is clean!");
    }
}
