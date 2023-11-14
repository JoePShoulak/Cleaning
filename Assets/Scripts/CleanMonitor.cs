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
    
    [SerializeField] private MeshRenderer treeDisplayRenderer;
    private Texture2D treeDisplayTex;
    private GridTree gridTree;
    
    private void Start()
    {
        texManager = GetComponent<TextureManager>();
        dirtyPoints = TextureManager.SamplePoints(texManager.mask, samplingFactor);
        totalDirt = dirtyPoints.Count;

        treeDisplayTex = (Texture2D)treeDisplayRenderer.material.mainTexture;
        gridTree = new GridTree(treeDisplayTex, texManager.brush);

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
        Debug.Log("Tree Check: " + gridTree.CheckIfClean(texManager.mask, treeDisplayTex));
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
