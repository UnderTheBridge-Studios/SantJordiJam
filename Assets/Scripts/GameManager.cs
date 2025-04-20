using System.Collections;
using UnityEngine;
public enum DayTime
{
    day = 0,
    night = 1,
    none = -1
}
public enum Tutorial
{
    wasd,
    espai,
    click_rosa,
    click_caixa,
    click_client
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
    private Castell m_CastellReference;
    private Cova m_CovaReference;


    [Header("Roses")]
    [SerializeField] private ClientManager m_clientManagerRef;
    [SerializeField] private float minTimeBetweenClients = 5f;
    [SerializeField] private float maxTimeBetweenClients = 15f;
    private int m_MaxClients;



    [Header("Tutos")]
    [SerializeField] private TutoPopUP m_wasdRef;
    [SerializeField] private TutoPopUP m_EspaiRef;
    [SerializeField] private TutoPopUP m_ClickRef_Caixa;
    [SerializeField] private TutoPopUP m_ClickRef_Client;
    [SerializeField] private TutoPopUP m_ClickRef_Rosa;

    //Accesors
    public DayTime currentDayTime => m_CurrentDayTime;
    public float minDistance => m_MinDistance;
    public int maxClients => m_MaxClients;
    public DracController dracReference => m_DracReference;



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

        //Test
        Invoke("StartRosesGame", 3);
        StartCoroutine(StartDracGame());
    }

    #region Escena Drac


    private IEnumerator StartDracGame()
    {
        //Obri nuvol

        yield return new WaitForSeconds(6f);

        ChangeToDay();
        yield return new WaitForSeconds(1f);
        ShowTuto(Tutorial.wasd);
        m_DracReference.EnableControl(true);

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

        if (m_SpawnerAnimalsInfo.Length <= m_DayCount)
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

    #endregion

    #region Escena Roses

    private void StartRosesGame()
    {
        m_MaxClients = 4;

        m_clientManagerRef.TrySpawnClient();
        StartCoroutine(RosesLoop());
    }

    private IEnumerator RosesLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenClients, maxTimeBetweenClients));
            if (m_clientManagerRef.GetClientCount() < m_MaxClients)
                m_clientManagerRef.TrySpawnClient();

            if (m_DayCount > 3)
                m_MaxClients = 4;

            else if (m_DayCount > 1)
                m_MaxClients = 3;
        }
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

    public void SetCastellReference(Castell reference)
    {
        m_CastellReference = reference;
    }

    #endregion

    #region Tutos

    public void ShowTuto(Tutorial tuto)
    {
        switch (tuto)
        {
            case Tutorial.wasd:
                m_wasdRef.Show();
                break;
            case Tutorial.espai:
                m_EspaiRef.Show();
                break;
            case Tutorial.click_rosa:
                m_ClickRef_Rosa.Show();
                break;
            case Tutorial.click_client:
                m_ClickRef_Client.Show();
                break;
            case Tutorial.click_caixa:
                m_ClickRef_Caixa.Show();
                break;
        }
    }

    public void HideTuto(Tutorial tuto)
    {
        switch (tuto)
        {
            case Tutorial.wasd:
                m_wasdRef.Hide();
                break;
            case Tutorial.espai:
                m_EspaiRef.Hide();
                break;
            case Tutorial.click_rosa:
                m_ClickRef_Rosa.Hide();
                break;
            case Tutorial.click_client:
                m_ClickRef_Client.Hide();
                break;
            case Tutorial.click_caixa:
                m_ClickRef_Caixa.Hide();
                break;
        }
    }

    #endregion

    public void EndGame()
    {
        Debug.Log("Last Day");
        m_CastellReference.Jump(true);
    }
}
