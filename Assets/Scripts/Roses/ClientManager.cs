using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private GameObject clientTutorialPrefab;
    [SerializeField] private Transform[] clientPositions;

    [Header("Configuración")]
    [SerializeField] private float minTimeBetweenClients = 5f;
    [SerializeField] private float maxTimeBetweenClients = 15f;
    [SerializeField] private int maxClients = 4;
    [SerializeField] private Vector3 clientSpawnPoint = new Vector3(-230f, 110f, 0f);

    private List<Client> clients = new List<Client>();
    private float nextClientTime;

    // GameManager
    private void Start()
    {
        //nextClientTime = Time.time + Random.Range(minTimeBetweenClients, maxTimeBetweenClients);
    }

    // GameManager
    private void Update()
    {
        //if (Time.time >= nextClientTime && clients.Count < maxClients)
        //{
        //    SpawnClient();
        //    nextClientTime = Time.time + Random.Range(minTimeBetweenClients, maxTimeBetweenClients);
        //}
    }

    public void SpawnClient()
    {
        if (clients.Count >= maxClients)
            return;

        Transform freePosition = GetFreePosition();
        if (freePosition == null)
            return;

        GameObject clientObject = Instantiate(clientPrefab, clientSpawnPoint, Quaternion.Euler(180, 180, 0)); 
        Client client = clientObject.GetComponent<Client>();

        client.Initialize(this, freePosition);
        clients.Add(client);
        //Debug.Log($"Cliente generado en posición {freePosition.name}. Total clientes: {clients.Count}");
    }

    public void SpawnClientTutorial()
    {
        if (clients.Count >= maxClients)
            return;

        Transform freePosition = GetFreePosition();
        if (freePosition == null)
            return;

        GameObject clientObject = Instantiate(clientTutorialPrefab, clientSpawnPoint, Quaternion.Euler(90, 180, 90));
        Client client = clientObject.GetComponent<Client>();

        client.Initialize(this, freePosition);
        clients.Add(client);
        Debug.Log($"Cliente generado en posición {freePosition.name}. Total clientes: {clients.Count}");
    }

    private Transform GetFreePosition()
    {
        List<Transform> availablePositions = new List<Transform>();

        foreach (Transform position in clientPositions)
        {
            bool isOccupied = false;
            foreach (Client client in clients)
            {
                if (client.CurrentPosition == position)
                {
                    isOccupied = true;
                    break;
                }
            }

            if (!isOccupied)
            {
                availablePositions.Add(position);
            }
        }

        if (availablePositions.Count == 0)
            return null;

        int randomIndex = Random.Range(0, availablePositions.Count);
        return availablePositions[randomIndex];
    }

    public void RemoveClient(Client client)
    {
        if (clients.Contains(client))
        {
            clients.Remove(client);
            //Debug.Log($"Cliente eliminado. Clientes restantes: {clients.Count}");
        }
    }

    public int GetClientCount()
    {
        return clients.Count;
    }

    public bool TrySpawnClient()
    {
        if (clients.Count < GameManager.Instance.maxClients)
        {
            SpawnClient();
            return true;
        }
        return false;
    }

    public Client FindNearestClientInState(Vector3 position, float maxDistance = 2000f)
    {
        Client nearestClient = null;
        float closestDistance = maxDistance;

        foreach (Client client in clients)
        {
            float distance = Vector3.Distance(position, client.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestClient = client;
            }
        }

        return nearestClient;
    }
}