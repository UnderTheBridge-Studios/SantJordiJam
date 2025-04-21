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
    [SerializeField] private GameObject ManoAbierta;
    [SerializeField] private GameObject ManoCerrada;

    private float interactionRadius = 5f;

    [Header("Configuración")]
    [SerializeField] private string billeteTag = "Billete";
    [SerializeField] private string rosaTag = "Rosa";
    [SerializeField] private string clienteTag = "Cliente";
    [SerializeField] private string registradoraTag = "CajaRegistradora";
    private float minXPosition = -90f;
    private float maxXPosition = 35f;

    // Estado actual del sistema
    [SerializeField] private GameObject heldObject;
    private bool isHolding = true;

    // Referencia al administrador de clientes
    [SerializeField] private ClientManager clientManager;
    [SerializeField] private Mesh billeteMesh2;

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

    private void accionMano()
    {
        bool manoActiva = ManoAbierta.activeSelf;

        ManoAbierta.SetActive(!manoActiva);
        ManoCerrada.SetActive(manoActiva);
    }

    private void GrabObject()
    {
        Vector3 boxHalfExtents = new Vector3(4f, 200f, 4f);
        Vector3 boxCenter = grabPoint.position + new Vector3(0, 100f, 0);
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents,
                                               Quaternion.identity, grabbableLayer);

        if (hitColliders.Length > 0)
        {
            System.Array.Sort(hitColliders, (a, b) =>
            Vector3.Distance(a.transform.position, grabPoint.position)
            .CompareTo(Vector3.Distance(b.transform.position, grabPoint.position)));

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
            if (heldObject.CompareTag(rosaTag))
            {
                heldObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            }
            else
            {
                heldObject.transform.localRotation = Quaternion.identity;
            }

            accionMano();

            if (heldObject.CompareTag(billeteTag))
            {
                MeshFilter meshFilter = heldObject.GetComponent<MeshFilter>();
                if (meshFilter != null && billeteMesh2 != null)
                {
                    meshFilter.mesh = billeteMesh2;
                }
                if (clientManager == null)
                {
                    clientManager = FindFirstObjectByType<ClientManager>();
                }

                Client nearestClient = clientManager.FindNearestClientInState(
                    transform.position,
                    1500f
                );
                if (nearestClient != null)
                {
                    nearestClient.BilleteTomado();
                }
                else
                {
                    nearestClient = clientManager.FindNearestClientInState(
                        transform.position,
                        1500f
                    );
                    if (nearestClient != null)
                    {
                        nearestClient.BilleteTomado();
                    }
                }
            }
        }
    }

    private void DropObject()
    {
        if (heldObject == null)
        {
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(grabPoint.position, interactionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (heldObject.CompareTag(billeteTag) && collider.CompareTag(registradoraTag))
            {
                if (isHolding)
                {
                    accionMano();
                }
                GameManager.Instance.HideTuto(Tutorial.click_caixa);
                Destroy(heldObject);
                heldObject = null;
                isHolding = false;

                break;
            }
            else if (heldObject.CompareTag(rosaTag) && collider.CompareTag(clienteTag))
            {
                Client targetClient = collider.GetComponent<Client>();
                if ((targetClient != null && targetClient.CurrentState == Client.ClientState.Waiting) || (targetClient != null && targetClient.billeteTaken()))
                {
                    targetClient.RosaEntregada();

                    if (isHolding)
                    {
                        accionMano();
                        targetClient.accionMano();
                    }

                    Transform clientGrabPoint = collider.transform.Find("GrabPoint");
                    if (clientGrabPoint != null)
                    {
                        heldObject.transform.SetParent(clientGrabPoint);
                        heldObject.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        heldObject.transform.SetParent(collider.transform);
                        heldObject.transform.localPosition = Vector3.zero;
                    }

                    heldObject = null;
                    isHolding = false;

                    break;
                }
                ClientTutorial targetClientTutorial = collider.GetComponent<ClientTutorial>();
                if ((targetClientTutorial != null) && ((targetClientTutorial.CurrentState == ClientTutorial.ClientState.Waiting) || (targetClientTutorial.CurrentState == ClientTutorial.ClientState.Walking)))
                {
                    targetClientTutorial.RosaEntregada();

                    if (isHolding)
                    {
                        accionMano();
                        targetClientTutorial.accionMano();
                    }

                    Transform clientGrabPoint = collider.transform.Find("GrabPoint");
                    if (clientGrabPoint != null)
                    {
                        heldObject.transform.SetParent(clientGrabPoint);
                        heldObject.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        heldObject.transform.SetParent(collider.transform);
                        heldObject.transform.localPosition = Vector3.zero;

                    }
                    heldObject = null;
                    isHolding = false;

                    break;
                }
            }
        }

        // Logica para no bloquear al jugador con la rosa, cambiable
        if (isHolding && heldObject.CompareTag(rosaTag) && clientManager.getTotalClients() > 0)
        {
            if (grabPoint.position.x > minXPosition && grabPoint.position.x < maxXPosition)
            {
                accionMano();
                heldObject.transform.SetParent(null);
                heldObject.transform.localPosition = new Vector3(grabPoint.transform.position.x, 103f, grabPoint.transform.position.z);
                heldObject = null;
                isHolding = false;
            }
        }
    }
}

