using System.Collections;
using UnityEngine;
public enum DayTime
{
    day = 0,
    night = 1,
    none = -1
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Day Cycle")]
    private DayCycleAnimation m_DayCycleAnimation;
    private DayTime m_CurrentDayTime;
    private int m_DayCount = 0;

    [Header("Spawn Animals")]
    [SerializeField] [Tooltip("Witch animals will spawn each day")]
    private SpawnerInfo[] m_SpawnerAnimalsInfo;
    [SerializeField] [Tooltip("The minim distance between each animal when spawned(to avoid overlap)")]
    private float m_MinDistance;
    private int m_AnimalsCounter;
    private Spawner m_SpawnerReference;


    [Header("Drac")]
    private DracController m_DracReference;
    private Cova m_CovaReference;

    //Accesors
    public DayTime currentDayTime => m_CurrentDayTime;
    public float minDistance => m_MinDistance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        m_DayCycleAnimation = GameObject.FindAnyObjectByType<DayCycleAnimation>().GetComponent<DayCycleAnimation>();
        ChangeToDay();
    }

    public void EnterCave()
    {
        if (m_CurrentDayTime != DayTime.night)
            return;

        StartCoroutine(EnterCaveSequence());
    }

    private IEnumerator EnterCaveSequence()
    {
        m_DracReference.EnableControl(false);

        float time;
        time = m_DracReference.MoveToPoints(m_CovaReference.exteriorCova);
        yield return new WaitForSeconds(time);
        time = m_DracReference.MoveToPoints(m_CovaReference.interiorCova);
        yield return new WaitForSeconds(time);
        ChangeToDay();
        yield return new WaitForSeconds(1f);

        m_DracReference.MoveToPoints(m_CovaReference.exteriorCova);
        m_DracReference.EnableControl(true);
    }

    #region Animals

    public void AnimalEaten()
    {
        m_AnimalsCounter--;
        if (m_AnimalsCounter == 0)
            ChangeToNight();
    }

    public void SetAnimalCounter(int count)
    {
        m_AnimalsCounter = count;
    }

    #endregion

    #region DayCycle
    public void ChangeToDay()
    {
        ChangeDayNight(DayTime.day);

        if(m_SpawnerAnimalsInfo.Length <= m_DayCount)
        {
            EndGame();
            return;
        }

        //Save the total animals the dragon have to eat
        m_AnimalsCounter = 0;
        foreach (Pair<GameObject, int> animal in m_SpawnerAnimalsInfo[m_DayCount].animals)
        {
            m_AnimalsCounter += animal.second;
        }

        m_SpawnerReference.Spawn(m_SpawnerAnimalsInfo[m_DayCount]);
    }

    public void ChangeToNight()
    {
        m_DayCount++;
        ChangeDayNight(DayTime.night);
    }


    [ContextMenu("ChangeDayNight")]
    public void ChangeDayNight(DayTime timeToChange = DayTime.none)
    {
        if (m_CurrentDayTime == timeToChange)
            return;

        m_CurrentDayTime = timeToChange;
        m_DayCycleAnimation.ChangeDayNight();
    }
    #endregion

    #region References
    public void SetSpawnerReference(Spawner reference)
    {
        m_SpawnerReference = reference;
    }

    public void SetDracReference(DracController reference)
    {
        m_DracReference = reference;
    }
    public void SetCaveReference(Cova reference)
    {
        m_CovaReference = reference;
    }

    #endregion

    public void EndGame()
    {
        Debug.Log("Last Day");
    }
}
