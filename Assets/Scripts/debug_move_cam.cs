using UnityEngine;

public class debug_move_cam : MonoBehaviour
{
    [Tooltip("Movement speed in units per second")]
    public float moveSpeed = 5.0f;

    [Tooltip("Minimum Z position the object can reach")]
    public float minPosition = -10.0f;

    [Tooltip("Maximum Z position the object can reach")]
    public float maxPosition = 10.0f;

    private void Start()
    {
        // Ensure the object starts within the valid position range
        Vector3 currentPos = transform.position;
        currentPos.z = Mathf.Clamp(currentPos.z, minPosition, maxPosition);
        transform.position = currentPos;
    }

    private void Update()
    {
        // Get input from A and D keys
        float horizontalInput = 0;

        if (Input.GetKey(KeyCode.A))
            horizontalInput = -1;
        else if (Input.GetKey(KeyCode.D))
            horizontalInput = 1;

        // Calculate movement amount
        float movementAmount = horizontalInput * moveSpeed * Time.deltaTime;

        // Get current position
        Vector3 currentPosition = transform.position;

        // Update Z position
        currentPosition.z += movementAmount;

        // Clamp Z position between min and max values
        currentPosition.z = Mathf.Clamp(currentPosition.z, minPosition, maxPosition);

        // Apply the new position
        transform.position = currentPosition;
    }
}