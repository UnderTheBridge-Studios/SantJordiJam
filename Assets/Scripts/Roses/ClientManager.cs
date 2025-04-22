using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private GameObject clientTutorialPrefab;
    [SerializeField] private GameObject clientFinalPrefab;
    [SerializeField] private Transform[] clientPositions;
    [SerializeField] private Transform clientTutorialPosition;
    [SerializeField] private Transform clientFinalPosition1;
    [SerializeField] private Transform clientFinalPosition2;
    private GameObject clientTutorial;
    private GameObject clientFinal;

    [Header("Configuración")]
    [SerializeField] private float minTimeBetweenClients = 5f;
    [SerializeField] private float maxTimeBetweenClients = 15f;
    [SerializeField] private int maxClients = 4;
    [SerializeField] private Vector3 clientSpawnPoint = new Vector3(-230f, 110f, 0f);

    [Header("Material Mans")]
    [SerializeField] private Material[] m_Materials;

    private List<Client> clients = new List<Client>();
    private float nextClientTime;
    private int totalClients = 0;

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

    private void Awake()
    {
        totalClients = 0;
    }

    public void SpawnClient()
    {
        if (clients.Count >= maxClients)
            return;

        Transform freePosition = GetFreePosition();
        if (freePosition == null)
            return;


        //get rand mat
        Material mat = m_Materials[UnityEngine.Random.Range(0, m_Materials.Length)];

        GameObject clientObject = Instantiate(clientPrefab, clientSpawnPoint, Quaternion.Euler(180, 180, 0)); 
        Client client = clientObject.GetComponent<Client>();
        clientObject.transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>().material = mat;
        clientObject.transform.GetChild(0).GetChild(3).GetComponent<MeshRenderer>().material = mat;
        totalClients++;
        client.Initialize(this, freePosition);
        clients.Add(client);
        //Debug.Log($"Cliente generado en posición {freePosition.name}. Total clientes: {clients.Count}");
    }

    public void SpawnClientTutorial()
    {
        if (clients.Count >= maxClients)
            return;

        clientTutorial = Instantiate(clientTutorialPrefab, clientSpawnPoint, Quaternion.Euler(90, 180, 90));
        ClientTutorial client = clientTutorial.GetComponent<ClientTutorial>();
        client.Initialize(this, clientTutorialPosition);
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

    public void RemoveClientTutorial()
    {
        if (clientTutorial)
        {
            Destroy(clientTutorial);
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

    public int getTotalClients()
    {
        return totalClients;
    }

    public bool isLastClientDone()
    {
        if (clients.Count <= 0)
            return true;

        return false;
    }
    public void SpawnClienteFinal()
    {
        clientFinal = Instantiate(clientFinalPrefab, new Vector3(-250f, 115f, 50f), Quaternion.Euler(90, 180, 90));
        ClientFinal client = clientFinal.GetComponent<ClientFinal>();
        client.Initialize(this, clientFinalPosition1, clientFinalPosition2);
    }
}