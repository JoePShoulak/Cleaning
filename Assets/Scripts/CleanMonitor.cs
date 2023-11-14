using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CleanMonitor : MonoBehaviour
{
    [SerializeField] private float pollingRate = 0.1f;
    [SerializeField] private int samplingFactor = 10;

    private TextureManager texManager;
    private List<Vector2Int> dirtyPoints;
    private int totalDirt;
    private bool clean;
    
    private void Start()
    {
        texManager = GetComponent<TextureManager>();
        dirtyPoints = TextureManager.SamplePoints(texManager.mask, samplingFactor);
        totalDirt = dirtyPoints.Count;

        StartCoroutine(MonitorCleanStatus());
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
        dirtyPoints.RemoveAll(point => texManager.mask.GetPixel(point.x, point.y).r == 0);
        var dirtFraction = dirtyPoints.Count / (float)totalDirt;
        
        Debug.Log(dirtFraction);
        return dirtFraction < .01;
    }

    private void Cleanup()
    {
        Painter.Fill(texManager, Color.black);
        Debug.Log("Item is clean!");
    }
}
