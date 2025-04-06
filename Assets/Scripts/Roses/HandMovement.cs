using UnityEngine;

public class HandMovement : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ConfigurableJoint configurableJoint;
    [SerializeField] private Camera mainCamera;

    [Header("Configuración")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxDistance = 500f;
    [SerializeField] private float yPosition; // Altura fija 

    private Vector3 targetPosition;
    private Plane movementPlane;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        yPosition = transform.position.y;
        movementPlane = new Plane(Vector3.up, new Vector3(0, yPosition, 0));

        // Configurable joint medio hecho para cuando haya mano
        //if (configurableJoint == null)
        //    configurableJoint = GetComponent<ConfigurableJoint>();

        //if (configurableJoint != null)
        //{
        //    JointDrive jointDrive = new JointDrive
        //    {
        //        positionSpring = 50f,
        //        positionDamper = 10f,
        //        maximumForce = 1000f
        //    };

        //    configurableJoint.xDrive = jointDrive;
        //    configurableJoint.zDrive = jointDrive;
        //}
    }

    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (movementPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            Vector3 direction = hitPoint - new Vector3(0, yPosition, 0);
            if (direction.magnitude > maxDistance)
            {
                direction = direction.normalized * maxDistance;
                hitPoint = new Vector3(0, yPosition, 0) + direction;
            }

            hitPoint.y = yPosition;
            targetPosition = hitPoint;
        }

        // Ahora mismo el configurableJoint no hace nada
        if (configurableJoint != null)
        {
            configurableJoint.targetPosition = targetPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);
        }
    }
}