using UnityEngine;
using DG.Tweening;


public enum DayTime
{
    day = 0,
    night = 1,
    none = -1
}

public class DayCycle : MonoBehaviour
{
    [SerializeField] private float m_ChangeCyclyTime = 2.0f;

    private DayTime m_CurrentDayTime;


    private void Start()
    {
        m_CurrentDayTime = DayTime.day;
        InvokeRepeating("ChangeDayNight", 3, 10f);
    }

    [ContextMenu ("ChangeDayNight")]
    public void ChangeDayNight(DayTime timeToChange = DayTime.none)
    {
        if (m_CurrentDayTime == timeToChange)
            return;

        m_CurrentDayTime = timeToChange;
        transform.DOLocalRotate(new Vector3(0, 0, 180) + transform.localEulerAngles, m_ChangeCyclyTime, RotateMode.FastBeyond360)
            .SetEase(Ease.OutExpo);
    }

}
