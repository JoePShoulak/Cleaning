using UnityEngine;

public class RaycastListener : MonoBehaviour
{
    private Camera cam;
    private TextureManager texManager;
    private Vector2 previousCoord;

    private void Start()
    {
        cam = Camera.main;
        texManager = GetComponent<TextureManager>();
    }

    private void Update() 
    {
        Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var raycastHit);
        if (raycastHit.collider == null) return;
        
        var textureCoord = raycastHit.textureCoord;
        if (textureCoord == previousCoord) return;
            
        Painter.PaintHit(textureCoord, texManager.brush, texManager.mask);

        previousCoord = textureCoord;
    }
}
