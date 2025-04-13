using UnityEngine;

public class TriggerChangeDayTime : MonoBehaviour
{
    [SerializeField] DayCycleAnimation m_DayCycle;
    [SerializeField] DayTime m_DayTime;



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Drac")
            return;

        //GameManager.Instance.ChangeDayNight(m_DayTime);
        GameManager.Instance.ChangeToDay();
    }
}
