using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform grabPoint;
    [SerializeField] private LayerMask grabbableLayer;
    [SerializeField] private float grabRadius = 0.5f;

    [Header("Gestion del Estado del Juego")]
    [SerializeField] private bool clienteHaPagado = false;
    [SerializeField] private bool clienteOfreciendoBillete = false;

    // Referencias a objetos del juego
    [SerializeField] private GameObject billete;
    [SerializeField] private GameObject rosa;

    // Estado del sistema
    private GameObject heldObject = null;
    private bool isHolding = false;

    //Test mientras no haya clientes normales
    private void Start()
    {
        StartCoroutine(EsperarYLlamarCliente());
    }

    private IEnumerator EsperarYLlamarCliente()
    {
        yield return new WaitForSeconds(5f);
        ClienteOfreceUnBillete();
    }

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

            // Verificamos si podemos agarrar este objeto según el estado del juego
            if (!PuedeAgarrarObjeto(objectToGrab))
                return;

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

            // Si el objeto es un billete, ya no está siendo ofrecido por el cliente
            if (heldObject.CompareTag("Billete"))
            {
                clienteOfreciendoBillete = false;
            }

            // AudioSource.PlayClipAtPoint(sonidoAgarre, transform.position);
        }
    }

    bool PuedeAgarrarObjeto(GameObject obj)
    {
        if (clienteOfreciendoBillete)
        {
            return obj.CompareTag("Billete");
        }

        if (clienteHaPagado && !clienteOfreciendoBillete)
        {
            return obj.CompareTag("Rosa");
        }

        return false;
    }

    void DropObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(grabPoint.position, grabRadius);
        foreach (Collider collider in hitColliders) 
        {
            if (heldObject != null)
            {
                // Billete entregado a la caja
                if (heldObject.CompareTag("Billete") && collider.CompareTag("CajaRegistradora"))
                {
                    // AudioSource.PlayClipAtPoint(sonidoDinero, transform.position);
                    // Añadir una animacion o algo
                    Destroy(heldObject);

                    // Activar agarrar rosa
                    clienteHaPagado = true;
                    if (rosa != null && !rosa.activeSelf)
                    {
                        rosa.SetActive(true);
                    }

                    isHolding = false;
                    heldObject = null;
                }
                // Rosa entregada al cliente
                else if (heldObject.CompareTag("Rosa") && collider.CompareTag("Cliente"))
                {
                    // AudioSource.PlayClipAtPoint(sonidoRosa, transform.position);
                    // Añadir una animacion o algo
                    Destroy(heldObject);

                    clienteHaPagado = false;
                    // esto seguramente se cambie cuando se implementen los clientes
                    Invoke("OfrecerNuevoBillete", 3f);

                    isHolding = false;
                    heldObject = null;
                }
            }
        }
    }

    public void ClienteOfreceUnBillete()
    {
        Debug.Log("billete");
        clienteOfreciendoBillete = true;

        if (billete != null && !billete.activeSelf)
        {
            billete.SetActive(true);
        }
    }

    void OfrecerNuevoBillete()
    {
        ClienteOfreceUnBillete();
    }
}