using UnityEngine;
using UnityEngine.InputSystem;

public class HandMovement : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera mainCamera;

    [Header("Configuración")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxDistance = 500f;
    [SerializeField] private float yPosition; // Altura fija

    private Vector3 targetPosition;
    private Plane movementPlane;
    private Vector2 mouseScreenPosition; // Input System
    private bool mousePositionInitialized = false;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        yPosition = transform.position.y;
        movementPlane = new Plane(Vector3.up, new Vector3(0, yPosition, 0));
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!mousePositionInitialized)
            return;

        Vector3 screenPoint = new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.WorldToScreenPoint(new Vector3(0, yPosition, 0)).z);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        worldPoint.y = yPosition;
        Vector3 direction = worldPoint - new Vector3(0, yPosition, 0);
        if (direction.magnitude > maxDistance)
        {
            direction = direction.normalized * maxDistance;
            worldPoint = new Vector3(0, yPosition, 0) + direction;
        }

        if (worldPoint.x < -80f)
        {
            worldPoint.x = -80f;
        }

        targetPosition = worldPoint;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mouseScreenPosition = context.ReadValue<Vector2>();
        mousePositionInitialized = true;
    }
}
