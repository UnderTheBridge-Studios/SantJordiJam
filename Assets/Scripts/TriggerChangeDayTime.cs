using UnityEngine;

public class TriggerChangeDayTime : MonoBehaviour
{
    [SerializeField] DayCycle m_DayCycle;
    [SerializeField] DayTime m_DayTime;



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if (other.tag != "Drac")
            return;

        m_DayCycle.ChangeDayNight(m_DayTime);
    }
}
