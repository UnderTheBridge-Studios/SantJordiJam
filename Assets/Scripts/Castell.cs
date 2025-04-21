using DG.Tweening;
using UnityEngine;

public class Castell : MonoBehaviour
{
    [SerializeField] private float m_JumpHeight = 5;
    [SerializeField] private Transform m_PortaE;
    [SerializeField] private Transform m_PortaD;

    private Tween m_JumpTween;

    private bool m_IsJumping;
    private float m_InitialVerticalPosition;

    void Start()
    {
        m_IsJumping = false;
        m_InitialVerticalPosition = transform.localPosition.y;
        GameManager.Instance.SetCastellReference(this);

    }

    public void Jump(bool value)
    {
        if (!m_IsJumping)
        {
            m_JumpTween = transform.DOLocalMoveY(transform.localPosition.y + m_JumpHeight, 0.2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.OutQuad);
            m_IsJumping = true;
        }
        else
        {
            m_JumpTween.OnStepComplete(() =>
            {
                if (m_JumpTween.CompletedLoops() % 2 == 0)
                {
                    m_JumpTween.Kill();
                    transform.position = new Vector3(transform.position.x, m_InitialVerticalPosition, transform.position.z);
                    m_JumpTween = null;
                }
            });
        }
    }

    public void OpenDoorsTween()
    {
        m_PortaE.DOLocalRotate(new Vector3(0, 45, 0), 1f);
        m_PortaD.DOLocalRotate(new Vector3(0, -225, 0), 1f);
    }

}
