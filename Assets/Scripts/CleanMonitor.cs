using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CleanMonitor : MonoBehaviour
{
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
        var material = GetComponent<MeshRenderer>().material;
        var sourceTex = (Texture2D)material.GetTexture(MaskID);
        CopyTexture(sourceTex, out dirtMaskTexture);
        
        material.SetTexture(MaskID, dirtMaskTexture);
        dirtyPoints = SamplePoints(dirtMaskTexture);
        totalDirt = dirtyPoints.Count;

        StartCoroutine(MonitorCleanStatus());

        RaycastListener.onRaycastHit += point => { Painter.PaintHit(point, dirtMaskTexture, brush); };
    }

    private static void CopyTexture(Texture2D source, out Texture2D destination)
    {
        destination = new Texture2D(source.width, source.height);
        destination.SetPixels(source.GetPixels());
        destination.Apply();
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

    private void CleanUp()
    {
        GetComponent<RaycastListener>().enabled = false;
    }

    private IEnumerator MonitorCleanStatus()
    {
        do
        {
            clean = CheckIfClean();
            yield return new WaitForSecondsRealtime(pollingRate);
        } while (clean == false);
        
        CleanUp();
    }

    private bool CheckIfClean()
    {
        dirtyPoints.RemoveAll(point => dirtMaskTexture.GetPixel(point.x, point.y).r == 0);
        var dirtFraction = dirtyPoints.Count / (float)totalDirt;
        
        Debug.Log(dirtFraction);
        return dirtFraction < .01;
    }
}
