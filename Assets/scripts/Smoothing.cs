using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    // The target (usually the XR rig’s tracking space or head)
    public Transform target;
    // How quickly the camera should follow (lower value = smoother, but slower updates)
    public float positionSmoothTime = 0.1f;
    public float rotationSmoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero;
    private Quaternion angularVelocity;

    void LateUpdate()
    {
        if(target == null)
            return;

        // Smooth the position with Vector3.SmoothDamp.
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, positionSmoothTime);

        // Smooth the rotation using Quaternion.Lerp (or SmoothDamp rotation if you have an implementation).
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime / rotationSmoothTime);
    }
}
