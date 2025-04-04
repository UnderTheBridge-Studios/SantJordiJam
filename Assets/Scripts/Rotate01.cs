using UnityEngine;

public class Rotate01 : MonoBehaviour
{
    [Tooltip("Set the axis around which the object will rotate (normalized internally)")]
    public Vector3 rotationAxis = Vector3.up;

    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 30.0f;

    [Tooltip("Use local space instead of world space for rotation")]
    public bool useLocalRotation = false;

    private void Start()
    {
        // Normalize the rotation axis to ensure consistent rotation speed
        if (rotationAxis != Vector3.zero)
            rotationAxis.Normalize();
        else
            rotationAxis = Vector3.up; // Default to up if zero vector provided
    }

    private void Update()
    {
        // Calculate rotation amount based on frame time
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Perform rotation based on the selected space
        if (useLocalRotation)
        {
            transform.Rotate(rotationAxis, rotationAmount, Space.Self);
        }
        else
        {
            transform.Rotate(rotationAxis, rotationAmount, Space.World);
        }
    }
}