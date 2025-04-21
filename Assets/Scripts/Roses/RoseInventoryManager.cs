using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

public class RoseInventoryManager : MonoBehaviour
{
    private CapsuleCollider detectionArea;
    [SerializeField] private List<GameObject> detectedRoses = new List<GameObject>();
    [SerializeField] private string roseTag = "Rosa";
    public delegate void RoseCountChanged(int count);
    public event RoseCountChanged OnRoseCountChanged;

    [Header("Refill Animation Settings")]
    [SerializeField] private float moveDistance = 100f; 
    [SerializeField] private float moveDuration = 1.5f; 
    [SerializeField] private GameObject rosePrefab; 
    [SerializeField] private int rosesToSpawn = 2;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(60f, 15f, 60f); 

    private bool isRefilling = false;
    private Vector3 initialPosition;

    private void Awake()
    {
        detectionArea = GetComponent<CapsuleCollider>();
        detectionArea.isTrigger = true;

        initialPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(roseTag) && !detectedRoses.Contains(other.gameObject))
        {
            detectedRoses.Add(other.gameObject);
            OnRoseCountChanged?.Invoke(detectedRoses.Count);
            //Debug.Log("Rosa detectada. Total: " + detectedRoses.Count);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(roseTag) && detectedRoses.Contains(other.gameObject))
        {
            detectedRoses.Remove(other.gameObject);
            OnRoseCountChanged?.Invoke(detectedRoses.Count);
            //Debug.Log("Rosa removida. Total: " + detectedRoses.Count);

            if (detectedRoses.Count == 0 && !isRefilling)
            {
                StartCoroutine(RefillRoses());
            }
        }
    }

    private IEnumerator RefillRoses()
    {
        isRefilling = true;

        Vector3 targetPosition = initialPosition + new Vector3(moveDistance, 0, 0);
        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(moveDuration);
        SpawnRoses();
        yield return new WaitForSeconds(0.5f);
        transform.DOMove(initialPosition, moveDuration).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(moveDuration);

        isRefilling = false;
    }

    private void SpawnRoses()
    {
        if (rosePrefab == null)
        {
            //Debug.LogError("Rose prefab not assigned in RoseInventoryManager!");
            return;
        }

        for (int i = 0; i < rosesToSpawn; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2) + 120f,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            GameObject newRose = Instantiate(rosePrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
            detectedRoses.Add(newRose);

            newRose.tag = roseTag;
        }
    }
}