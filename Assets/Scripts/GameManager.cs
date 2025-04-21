using System.Collections;
using System.Threading;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    //private PrincesaController m_PrincesaReference;
    private Castell m_CastellReference;
    private Cova m_CovaReference;
    private bool m_DracGameHasStarted = false;
    private bool m_IsLastDay = false;

    [Header("Roses")]
    [SerializeField] private ClientManager m_clientManagerRef;
    [SerializeField] [Tooltip("El nombre de clients que ha de atendre abans de que començi el drac")] 
    private int m_ClientsBeforeDrac = 5;
    [SerializeField] private float m_MinTimeBetweenClients = 5f;
    [SerializeField] private float m_MaxTimeBetweenClients = 15f;
    private int m_MaxClients;
    private bool m_TutoBilleHasShown = false;
    private bool m_StopRoseLoop = false;

    [Space]
    //Shader
    [SerializeField] private float m_LerpSpeed = 1;
    private float m_TarjetShaderValue;
    private float m_CurrentShaderValue;

     [Header("Tutos")]
    [SerializeField] private TutoPopUP m_wasdRef;
    [SerializeField] private TutoPopUP m_EspaiRef;
    [SerializeField] private TutoPopUP m_ClickRef_Caixa;
    [SerializeField] private TutoPopUP m_ClickRef_Client;
    [SerializeField] private TutoPopUP m_ClickRef_Rosa;
    [Space]
    [SerializeField] private FullScreenPassRendererFeature renderPass;
    [Space]
    [SerializeField] private Texture2D m_Cursor;

    //Accesors
    public DayTime currentDayTime => m_CurrentDayTime;
    public float minDistance => m_MinDistance;
    public int maxClients => m_MaxClients;
    public DracController dracReference => m_DracReference;
    //public PrincesaController princesaReference => m_PrincesaReference;
    public bool isLastDay => m_IsLastDay;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        m_DracGameHasStarted = false;

        m_TarjetShaderValue = -1;
        renderPass.passMaterial.SetFloat("_SceneLerp", -1);
        m_DayCycleAnimation = FindAnyObjectByType<DayCycleAnimation>().GetComponent<DayCycleAnimation>();

        Cursor.SetCursor(m_Cursor, Vector2.zero, CursorMode.Auto);

        RosesTutorial();
    }

    #region Escena Drac
    private void Update()
    {
        if (m_DracGameHasStarted)
        {
            if (m_clientManagerRef.GetClientCount() == 4)
                m_TarjetShaderValue = 0.15f;
            else if (m_clientManagerRef.GetClientCount() == 3)
                m_TarjetShaderValue = 0.35f;
            else if (m_clientManagerRef.GetClientCount() == 2)
                m_TarjetShaderValue = 0.6f;
        }

        m_CurrentShaderValue = Mathf.Lerp(m_CurrentShaderValue, m_TarjetShaderValue, Time.deltaTime * m_LerpSpeed);
        renderPass.passMaterial.SetFloat("_SceneLerp", m_CurrentShaderValue);
    }

    private IEnumerator StartDracGame()
    {
        m_TarjetShaderValue = 1;
        m_DracGameHasStarted = true;
        ChangeToDay();
        yield return new WaitForSeconds(5f);
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
    #endregion

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

    #region Escena Roses
    private void RosesTutorial()
    {
        m_clientManagerRef.SpawnClientTutorial();
        //ShowTuto(Tutorial.click_rosa);
    }

    public void StartRosesGame()
    {
        m_MaxClients = 2;

        StartCoroutine(RosesLoop());
    }

    private IEnumerator RosesLoop()
    {
        HideTuto(Tutorial.click_rosa);
        yield return new WaitForSeconds(2f);
        m_clientManagerRef.TrySpawnClient();

        while (!m_StopRoseLoop)
        {
            yield return new WaitForSeconds(Random.Range(m_MinTimeBetweenClients, m_MaxTimeBetweenClients));
            if (m_clientManagerRef.GetClientCount() < m_MaxClients)
                m_clientManagerRef.TrySpawnClient();

       
            if (m_clientManagerRef.getTotalClients() == m_ClientsBeforeDrac)
                StartCoroutine(StartDracGame());


            if (m_DayCount == 3)
                m_MaxClients = 4;
            else if (m_DayCount == 1)
                m_MaxClients = 3;
        }
    }

    public void TutoBillete()
    {
        if (m_TutoBilleHasShown)
            return;
            
        ShowTuto(Tutorial.click_caixa);
        m_TutoBilleHasShown = true;
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

    //public void SetPrincesaReference(PrincesaController reference)
    //{
    //    m_PrincesaReference = reference;
    //}

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

    #region Ending
    /*Final!
    Triggers:
    - Última cova:
        - Max clients 1
    - Arrives al castell:
        - Bounce castle.
        - Stop spawn clients
    - Atès últim client:
        - Apareix llibre de fons
        - Spawn princesa
        - Start cinemàtica final imaginació
    - Acava cinemàtica imaginacióa
        - Deixa el llibre sobre la taula
        - Fade imaginació
    - Dones l'última rosa
        - Final screen
    */
    public void EndGame()
    {
        Debug.Log("Last Day");
        m_IsLastDay = true;
        m_MaxClients = 1;
        m_CastellReference.Jump(true);
    }

    public void StopRoseLoop()
    {
        m_StopRoseLoop = true;
        StartCoroutine(EndingCinematic());
    }

    private IEnumerator EndingCinematic()
    {
        // Move drac to position
        dracReference.EnableControl(false);
        float time = dracReference.MoveToPoints(new Vector3(-10000, 3, -6));
        yield return new WaitForSeconds(time);
        
        dracReference.MoveToPoints(new Vector3(-10000, 3, -7));
        yield return new WaitUntil(m_clientManagerRef.isLastClientDone);
        
        // Open doors
        m_CastellReference.Jump(false);
        yield return new WaitForSeconds(2f);
        
        Tween doorTween = m_CastellReference.OpenDoorTween();
        yield return new WaitForSeconds(1f);

        // Move princesa
        //princesaReference.MoveToPoints(new Vector3(-10000, 3, -10));
    }

    #endregion
}
