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


    public DayTime currentDayTime => m_CurrentDayTime;

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
    }

    #region DayCycle
    public void ChangeToDay()
    {
        m_DayCount++;
        ChangeDayNight(DayTime.day);
    }

    public void ChangeToNight()
    {
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
}
