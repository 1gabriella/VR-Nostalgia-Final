using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private float zCoord;

    // Called when the mouse button is pressed over the Collider
    void OnMouseDown()
    {
        // Capture the z coordinate of the object in the world space
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        // Calculate the offset between the object's world position and the mouse position
        offset = transform.position - GetMouseWorldPosition();
    }

    // Called when the mouse is dragged over the Collider
    void OnMouseDrag()
    {
        // Update the object's position to follow the mouse, maintaining the initial offset
        transform.position = GetMouseWorldPosition() + offset;
    }

    // Helper method to convert the current mouse position to a world position
    private Vector3 GetMouseWorldPosition()
    {
        // Get the current mouse position in screen coordinates
        Vector3 mousePoint = Input.mousePosition;
        // Assign the stored z coordinate so that the object remains at its original depth
        mousePoint.z = zCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}