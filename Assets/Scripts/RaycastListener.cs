using UnityEngine;

public class RaycastListener : MonoBehaviour
{
    public delegate void OnRaycastHit(Vector2 hitVector2);
    public static OnRaycastHit onRaycastHit;
    
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update() 
    {
        Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var raycastHit);
        if (raycastHit.collider == null) return;
            
        onRaycastHit.Invoke(raycastHit.textureCoord);
    }
}
