using UnityEngine;

public class ZoomController : MonoBehaviour
{
    public Camera mainCamera;   // Reference to the main camera
    public float zoomSpeed = 2f; // Speed of zoom
    public float minZoom = 20f; // Minimum zoom (field of view)
    public float maxZoom = 60f; // Maximum zoom (field of view)

    public void ZoomIn()
    {
        // Reduce the field of view for zooming in
        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView - zoomSpeed, minZoom, maxZoom);
    }

    public void ZoomOut()
    {
        // Increase the field of view for zooming out
        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + zoomSpeed, minZoom, maxZoom);
    }
}
