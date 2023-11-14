using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{   
    private static readonly int MaskID = Shader.PropertyToID("_mask");
    [HideInInspector] public Texture2D mask;
    public Texture2D brush;
    [SerializeField] private int samplingFactor = 10;

    public void Start()
    {
        var material = GetComponent<MeshRenderer>().material;
        var sourceTex = (Texture2D)material.GetTexture(MaskID);
        CopyTexture(sourceTex, out mask);
        
        material.SetTexture(MaskID, mask);
    }

    private static void CopyTexture(Texture2D source, out Texture2D destination)
    {
        destination = new Texture2D(source.width, source.height);
        destination.SetPixels(source.GetPixels());
        destination.Apply();
    }

    internal List<Vector2Int> SamplePoints(Texture tex)
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
}
