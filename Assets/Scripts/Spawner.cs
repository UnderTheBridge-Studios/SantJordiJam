using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pair<T1, T2>
{
    public T1 first;
    public T2 second;
}

[System.Serializable]
public struct SpawnerInfo
{
    [Tooltip("Array of how many animals will spawn, with a reference to the prefab, and the number of each prefab")]
    public Pair<GameObject, int>[] animals;
    [Tooltip("The radius within the aniamls will spawn")]
    public float radius;
}

public class Spawner : MonoBehaviour
{

    [SerializeField]
    private Transform m_AnimalsContainer;

    private List<Vector3> m_Positions; //List of the position of each animal

    private int maxTrys; //Max try to find a valid spawn point

    private void Awake()
    {
        GameManager.Instance.SetSpawnerReference(this);
        m_Positions = new List<Vector3>();
    }

    public void Spawn(SpawnerInfo info)
    {
        var minDistance = GameManager.Instance.minDistance;
        m_Positions.Clear();

        foreach (Pair<GameObject, int> animalType in info.animals)
        {
            maxTrys = animalType.second * 10;
            int spawnedCount = 0;
            while (spawnedCount < animalType.second)
            {
                var pos2D = Random.insideUnitCircle * info.radius;
                var pos3D = new Vector3(pos2D.x, 0, pos2D.y) + transform.localPosition;
                if (pos3D.z > transform.position.z)
                {
                    bool valid = true;
                    foreach(Vector3 pos in m_Positions)
                    {
                        float distance = Vector3.Distance(pos, pos3D);
                        if (distance < minDistance)
                            valid = false;
                    }


                    if (valid)
                    {
                        GameObject animalSpawned = Instantiate(animalType.first, pos3D, Quaternion.identity);
                        animalSpawned.transform.SetParent(m_AnimalsContainer, false);

                        m_Positions.Add(animalSpawned.transform.localPosition);
                        spawnedCount++;
                    }


                    maxTrys--;
                    if (maxTrys <= 0)
                    {
                        GameManager.Instance.SetAnimalCounter(m_Positions.Count);
                        return; //Avoid infinite loop
                    }
                }
            }
        }

    }
}