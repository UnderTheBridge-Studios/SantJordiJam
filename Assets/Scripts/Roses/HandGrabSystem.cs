using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform grabPoint;
    [SerializeField] private LayerMask grabbableLayer;
    [SerializeField] private float grabRadius = 0.5f;

    // Estado del sistema
    private GameObject heldObject = null;
    private bool isHolding = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isHolding)
                GrabObject();
            else
                DropObject();
        }
    }

    void GrabObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(grabPoint.position, grabRadius, grabbableLayer);

        if (hitColliders.Length > 0)
        {
            GameObject objectToGrab = hitColliders[0].gameObject;
            isHolding = true;
            heldObject = objectToGrab;

            // Esto posiblemente no haga falta, sirve para desactivar física y colisiones si las tienen
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // Hacer que el objeto sea hijo de la mano y posicionarlo correctamente
            heldObject.transform.SetParent(grabPoint);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;

            // AudioSource.PlayClipAtPoint(sonidoAgarre, transform.position);
        }
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(grabPoint.position, grabRadius);

            bool objectDelivered = false;

            foreach (Collider collider in hitColliders)
            {
                // Billete
                if (heldObject.CompareTag("Billete") && collider.CompareTag("CajaRegistradora"))
                {
                    // AudioSource.PlayClipAtPoint(sonidoDinero, transform.position);

                    // Añadir una animacion o algo
                    Destroy(heldObject);
                    objectDelivered = true;
                    break;
                }
                // Rosa
                else if (heldObject.CompareTag("Rosa") && collider.CompareTag("Cliente"))
                {
                    // AudioSource.PlayClipAtPoint(sonidoRosa, transform.position);

                    // Añadir una animacion o algo
                    Destroy(heldObject);
                    objectDelivered = true;
                    break;
                }
            }

            // Quizas se borre en el futuro, sirve para dejar el objeto en la mesa si no es una mano o la caja
            if (!objectDelivered)
            {
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
                heldObject.transform.SetParent(null);

                // AudioSource.PlayClipAtPoint(sonidoSoltar, transform.position);
            }

            isHolding = false;
            heldObject = null;
        }
    }
}