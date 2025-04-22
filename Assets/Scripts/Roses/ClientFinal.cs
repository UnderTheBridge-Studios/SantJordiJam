using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientFinal : MonoBehaviour
{
    public enum ClientFinalState
    {
        Walking,         // Caminando hasta la posición de espera
        Waiting,         // Esperando su turno
        MovingToTable,   // Moviéndose hacia la mesa & Dejando el libro en la mesa
        MovingAside,     // Moviéndose hacia un lado para dejar ver el libro
        WaitingRose,     // Esperando su rosa     // 
        Leaving          // Lo que sea que ocurre al final      
    }

    [Header("Referencias")]
    [SerializeField] private GameObject libroObject;
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private GameObject ManoAbierta;
    [SerializeField] private GameObject ManoCerrada;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private float sideOffset = 10f; 

    // Referencias
    private ClientManager clientManager;
    private Transform currentPosition;
    private Transform waitingPosition; 
    private Transform tablePosition;
    private Vector3 sidePosition;
    private ClientFinalState currentState;

    // Propiedades
    public ClientFinalState CurrentState => currentState;
    public Transform CurrentPosition => currentState == ClientFinalState.Waiting ? waitingPosition : tablePosition;

    public void Initialize(ClientManager manager, Transform position1, Transform position2)
    {
        clientManager = manager;
        waitingPosition = position1;
        tablePosition = position2;
        SetState(ClientFinalState.Walking);
    }

    private void Update()
    {
        switch (currentState)
        {
            case ClientFinalState.Walking:
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    waitingPosition.position,
                    walkSpeed * Time.deltaTime
                );
                if (Vector3.Distance(transform.position, waitingPosition.position) < 0.1f)
                {
                    SetState(ClientFinalState.Waiting);
                }
                break;

            case ClientFinalState.MovingToTable:
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    tablePosition.position,
                    walkSpeed * Time.deltaTime
                );
                if (Vector3.Distance(transform.position, tablePosition.position) < 0.1f)
                {
                    DejarLibro();
                }
                break;

            case ClientFinalState.MovingAside:
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    sidePosition,
                    walkSpeed * Time.deltaTime
                );

                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(-1, 90, 0));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

                if (Vector3.Distance(transform.position, sidePosition) < 0.1f)
                {
                    SetState(ClientFinalState.WaitingRose);
                }
                break;

            case ClientFinalState.Leaving:
                Vector3 exitPosition = new Vector3(-230f, transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    exitPosition,
                    walkSpeed * Time.deltaTime
                );
                if (transform.position.x <= -229f)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    public void SetState(ClientFinalState newState)
    {
        currentState = newState;
    }

    private void DejarLibro()
    {
        if (libroObject && currentState == ClientFinalState.MovingToTable)
        {
            if (libroObject)
            {
                libroObject.transform.SetParent(null);
                libroObject.transform.localPosition = new Vector3(grabPoint.transform.position.x, 103f, grabPoint.transform.position.z);
                libroObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }

            AccionMano();

            sidePosition = new Vector3(
                tablePosition.position.x,
                tablePosition.position.y - 13f,
                tablePosition.position.z + sideOffset
            );

            SetState(ClientFinalState.MovingAside);
        }
    }

    public void RosaEntregada()
    {
        if (currentState == ClientFinalState.WaitingRose)
        {
            SetState(ClientFinalState.Leaving);
            GameManager.Instance.FinalScreen();
            //StartCoroutine(LeaveAfterDelay(0.2f));
        }
    }

    public void AccionMano()
    {
        bool manoActiva = ManoAbierta.activeSelf;
        ManoAbierta.SetActive(!manoActiva);
        ManoCerrada.SetActive(manoActiva);
    }
}