using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CleanMonitor : MonoBehaviour
{
    [SerializeField] private float pollingRate = 0.1f;
    
    private TextureManager texManager;
    private List<Vector2Int> dirtyPoints;
    private bool clean;
    
    [SerializeField] private MeshRenderer treeDisplayRenderer;
    private Texture2D treeDisplayTex;
    private GridTree gridTree;

    private RaycastListener raycastListener;
    
    private void Start()
    {
        texManager = GetComponent<TextureManager>();
        raycastListener = GetComponent<RaycastListener>();

        treeDisplayTex = (Texture2D)treeDisplayRenderer.material.mainTexture;
        gridTree = new GridTree(treeDisplayTex, texManager.brush);
        raycastListener.gridTree = gridTree;

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
        var gridTreeProgress = 1-gridTree.GetDirtyFraction(texManager.mask);
        Debug.Log("Tree Check: " + gridTreeProgress);
        
        return gridTreeProgress >= 1f;
    }

    private void Cleanup()
    {
        Painter.Fill(texManager, Color.black);
        Debug.Log("Item is clean!");
        raycastListener.enabled = false;
    }
}
