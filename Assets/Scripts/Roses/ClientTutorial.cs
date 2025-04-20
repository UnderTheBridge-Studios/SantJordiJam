using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientTutorial : MonoBehaviour
{
    // Estados del cliente
    public enum ClientState
    {
        Walking,     // Caminando hasta la mesa
        Waiting,     // Esperando su rosa
        Served,      // Recibió su rosa
        Leaving      // Marchándose
    }

    [Header("Referencias")]
    [SerializeField] private float walkSpeed = 20f;

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
                    // Al llegar a la posición, cambia directamente a estado de espera
                    SetState(ClientState.Waiting);
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
                    clientManager.RemoveClientTutorial();
                }
                break;
        }
    }

    private void SetState(ClientState newState)
    {
        currentState = newState;
    }

    public void RosaEntregada()
    {
        if (currentState == ClientState.Waiting || currentState == ClientState.Walking)
        {
            GameManager.Instance.StartRosesGame();
            SetState(ClientState.Served);
            StartCoroutine(LeaveAfterDelay(0.2f));
        }
    }

    private IEnumerator LeaveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetState(ClientState.Leaving);
    }
}