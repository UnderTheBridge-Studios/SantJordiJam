using UnityEngine;
using DG.Tweening;


public class DayCycleAnimation : MonoBehaviour
{
    [SerializeField] private float m_ChangeCyclyTime = 2.0f;


    public void ChangeDayNight()
    {
        transform.DOLocalRotate(new Vector3(0, 0, 180) + transform.localEulerAngles, m_ChangeCyclyTime, RotateMode.FastBeyond360)
            .SetEase(Ease.OutExpo);
    }

}
