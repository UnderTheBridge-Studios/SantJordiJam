using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    // Estados del cliente
    public enum ClientState
    {
        Walking,     // Caminando hasta la mesa
        Offering,    // Ofreciendo el billete
        Waiting,     // Esperando su rosa (billete ya tomado)
        Served,      // Recibió su rosa
        Leaving      // Marchándose
    }

    [Header("Referencias")]
    [SerializeField] private GameObject billeteObject;
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private GameObject ManoAbierta;
    [SerializeField] private GameObject ManoCerrada;

    // Referencias
    private ClientManager clientManager;
    private Transform currentPosition;
    private ClientState currentState;

    // Propiedades
    public ClientState CurrentState => currentState;
    public Transform CurrentPosition => currentPosition;

    public void Initialize(ClientManager manager, Transform position)
    {
        clientManager = manager;
        currentPosition = position;
        SetState(ClientState.Walking);
    }

    private void Update()
    {
        switch (currentState)
        {
            case ClientState.Walking:
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    currentPosition.position,
                    walkSpeed * Time.deltaTime
                );
                if (Vector3.Distance(transform.position, currentPosition.position) < 0.1f)
                {
                    OfrecerBillete();
                }
                break;
            case ClientState.Leaving:
                Vector3 exitPosition = new Vector3(-230f, transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    exitPosition,
                    walkSpeed * Time.deltaTime
                );

                // Cambiable
                if (transform.position.x <= -229f)
                {
                    clientManager.RemoveClient(this);
                    Destroy(gameObject);
                }
                break;
        }
    }

    private void SetState(ClientState newState)
    {
        currentState = newState;
    }

    private void OfrecerBillete()
    {
        if (billeteObject && currentState == ClientState.Walking)
        {
            SetState(ClientState.Offering);
        }
    }

    public void BilleteTomado()
    {
        SetState(ClientState.Waiting);
    }

    public void RosaEntregada()
    {
        if (currentState == ClientState.Waiting || (currentState == ClientState.Offering && billeteObject == null) || (currentState == ClientState.Walking && billeteObject == null))
        {
            SetState(ClientState.Served);
            StartCoroutine(LeaveAfterDelay(0.2f));
        }
    }

    private IEnumerator LeaveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetState(ClientState.Leaving);
    }

    public bool billeteTaken()
    {
        return (billeteObject == null);
    }

    public void accionMano()
    {
        bool manoActiva = ManoAbierta.activeSelf;

        ManoAbierta.SetActive(!manoActiva);
        ManoCerrada.SetActive(manoActiva);
    }
}