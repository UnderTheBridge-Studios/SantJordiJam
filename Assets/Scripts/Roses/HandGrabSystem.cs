using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandGrabSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform grabPoint;
    [SerializeField] private LayerMask grabbableLayer;
    [SerializeField] private float grabRadius = 0.5f;
    private float interactionRadius = 1.5f;

    [Header("Configuración")]
    [SerializeField] private string billeteTag = "Billete";
    [SerializeField] private string rosaTag = "Rosa";
    [SerializeField] private string clienteTag = "Cliente";
    [SerializeField] private string registradoraTag = "CajaRegistradora";

    // Estado actual del sistema
    private GameObject heldObject = null;
    private bool isHolding = false;

    // Referencia al administrador de clientes
    private ClientManager clientManager;

    private void Start()
    {
        clientManager = FindFirstObjectByType<ClientManager>();
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isHolding)
                GrabObject();
            else
                DropObject();
        }
    }

    private void GrabObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(grabPoint.position, grabRadius, grabbableLayer);

        if (hitColliders.Length > 0)
        {
            GameObject objectToGrab = hitColliders[0].gameObject;
            isHolding = true;
            heldObject = objectToGrab;

            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            heldObject.transform.SetParent(grabPoint);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;

            if (heldObject.CompareTag(billeteTag))
            {
                Client nearestClient = clientManager.FindNearestClientInState(
                    transform.position,
                    1500f
                );
                if (nearestClient != null)
                {
                    nearestClient.BilleteTomado();
                }
            }

            // Efectos de audio opcionales
            // AudioSource.PlayClipAtPoint(grabSound, transform.position);
        }
    }

    private void DropObject()
    {
        if (heldObject == null)
            return;

        Collider[] hitColliders = Physics.OverlapSphere(grabPoint.position, interactionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (heldObject.CompareTag(billeteTag) && collider.CompareTag(registradoraTag))
            {
                Destroy(heldObject);
                heldObject = null;
                isHolding = false;

                Debug.Log("Pago recibido en caja registradora");
                break;
            }
            else if (heldObject.CompareTag(rosaTag) && collider.CompareTag(clienteTag))
            {
                Client targetClient = collider.GetComponent<Client>();
                if (targetClient != null && targetClient.CurrentState == Client.ClientState.Waiting)
                {
                    targetClient.RosaEntregada();

                    // Cambiar para que entregue la rosa en vez de eliminarla
                    // Eliminar la rosa
                    Destroy(heldObject);
                    heldObject = null;
                    isHolding = false;

                    Debug.Log("Rosa entregada al cliente");
                    break;
                }
            }
        }

        // Logica para no bloquear al jugador con la rosa, cambiable
        if (isHolding && heldObject.CompareTag(rosaTag))
        {
            heldObject.transform.SetParent(null);
            heldObject = null;
            isHolding = false;
        }
    }
}

