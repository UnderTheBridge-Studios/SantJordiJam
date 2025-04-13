using DG.Tweening;
using UnityEngine;

public class OvellaController : IEdable
{
    [SerializeField] private Transform m_OvellaModel;

    private float m_InitialVerticalPosition;
    private Tween m_JumpTween;
    private bool m_isMoving;

    private void Start()
    {
        m_InitialVerticalPosition = m_OvellaModel.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isMoving)
            JumpAnimation();
        else
            StopAnimation();
    }

    private void JumpAnimation()
    {
        if (m_JumpTween == null)
        {
            m_JumpTween = m_OvellaModel.DOLocalMoveY(m_OvellaModel.localPosition.y + 0.5f, 0.1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            m_JumpTween.OnStepComplete(() =>
            {
                if (m_JumpTween.CompletedLoops() % 2 == 0)
                {
                    m_JumpTween.Kill();
                    m_OvellaModel.position = new Vector3(m_OvellaModel.position.x, m_InitialVerticalPosition, m_OvellaModel.position.z);
                    m_JumpTween = null;
                }
            });
        }
    }

    private void StopAnimation()
    {
        if (m_JumpTween != null)
        {
            m_JumpTween.Kill();
            m_JumpTween = null;
        }
    }

    public override void OnEat()
    {
        m_JumpTween.Kill();
        GameManager.Instance.AnimalEaten();
        Destroy(gameObject);
    }
}
