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
        var gridTreeProgress = gridTree.GetCleanFraction(texManager.mask, treeDisplayTex);
        Debug.Log("Tree Check: " + gridTreeProgress);
        
        return gridTreeProgress == 0;
    }

    private void Cleanup()
    {
        Painter.Fill(texManager, Color.black);
        Debug.Log("Item is clean!");
    }
}
