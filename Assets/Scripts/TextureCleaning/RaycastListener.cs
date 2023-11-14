using Grid;
using UnityEngine;

namespace TextureCleaning
{
    public class RaycastListener : MonoBehaviour
    {
        private Camera cam;
        private TextureManager texManager;
        private Vector2 previousCoord;
        private MeshCollider meshCollider;
        public GridTree gridTree;

        private void Start()
        {
            cam = Camera.main;

            texManager = GetComponent<TextureManager>();
            meshCollider = GetComponent<MeshCollider>();
        }

        private void Update()
        {
            if (gridTree == null) return;

            Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var raycastHit);
            if (raycastHit.collider != meshCollider) return;

            var textureCoord = raycastHit.textureCoord;
            if (textureCoord == previousCoord) return;

            gridTree.RegisterHit(textureCoord);
            Painter.PaintHit(textureCoord, texManager.brush, texManager.mask);

            previousCoord = textureCoord;
        }
    }
}
