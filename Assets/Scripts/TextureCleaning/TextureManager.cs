using UnityEngine;

namespace TextureCleaning
{
    public class TextureManager : MonoBehaviour
    {   
        [HideInInspector] public Texture2D mask;
        public Texture2D brush;
    
        private static readonly int MaskID = Shader.PropertyToID("_mask");

        public void Awake()
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
    }
}
