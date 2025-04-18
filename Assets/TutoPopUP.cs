using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class TutoPopUP : MonoBehaviour
{

    [SerializeField] private float m_HeightAnimation = 50;

    private Image popUp;

    private float m_OriginalHeight;
    private bool IsHidden;

    void Start()
    {
        popUp = GetComponent<Image>();
        popUp.DOFade(0, 0.0f);
        m_OriginalHeight = transform.localPosition.y;
        IsHidden = true;
    }

    public void Show()
    {
        if (!IsHidden)
            return;

        IsHidden = false;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - m_HeightAnimation, transform.localPosition.z);
        popUp.DOFade(1, 0.3f);
        transform.DOLocalMoveY(m_OriginalHeight, 0.3f);
    }

    public void Hide()
    {
        if (IsHidden)
            return;

        IsHidden = true;
        float height = transform.localPosition.y - m_HeightAnimation;
        transform.DOLocalMoveY(m_OriginalHeight - m_HeightAnimation, 0.3f);
        popUp.DOFade(0, 0.3f);
    }
}
