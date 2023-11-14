using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CleanMonitor : MonoBehaviour
{
    [SerializeField] private float pollingRate = 0.1f;

    private List<Vector2Int> dirtyPoints;
    private int totalDirt;
    private bool clean;

    private TextureManager _textureManager;
    
    public void Start()
    {
        _textureManager = GetComponent<TextureManager>();
        
        dirtyPoints = _textureManager.SamplePoints(_textureManager.mask);
        totalDirt = dirtyPoints.Count;

        StartCoroutine(MonitorCleanStatus());

        RaycastListener.onRaycastHit += point => { Painter.PaintHit(point, _textureManager); };
    }

    private IEnumerator MonitorCleanStatus()
    {
        do
        {
            clean = CheckIfClean();
            yield return new WaitForSecondsRealtime(pollingRate);
        } while (clean == false);
    }

    private bool CheckIfClean()
    {
        dirtyPoints.RemoveAll(point => _textureManager.mask.GetPixel(point.x, point.y).r == 0);
        var dirtFraction = dirtyPoints.Count / (float)totalDirt;
        
        Debug.Log(dirtFraction);
        return dirtFraction < .01;
    }
}
