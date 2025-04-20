using System.Collections;
using DG.Tweening;
using Unity.Behavior;
using UnityEngine;

public class OvellaController : IEdable
{
    [SerializeField] private Transform m_OvellaModel;
    [SerializeField] private BehaviorGraphAgent m_BehaviorAgent;
    [SerializeField] private ParticleSystem m_ParticleSystem;
    [SerializeField] private SphereCollider m_Collider;
    
    private float m_InitialVerticalPosition;
    private Tween m_JumpTween;
    private BlackboardVariable m_IsMoving;

    private void Start()
    {
        m_InitialVerticalPosition = m_OvellaModel.transform.position.y;
        m_BehaviorAgent.BlackboardReference.GetVariable("IsMoving", out m_IsMoving);
    }

    // Update is called once per frame
    private void Update()
    {
        if ((bool)m_IsMoving.ObjectValue)
            MoveAnimation(0.5f, 0.1f);
        else
            StopAnimation();
    }

    private void MoveAnimation(float jumpHeight, float jumptDuration)
    {
        if (m_JumpTween == null)
        {
            m_JumpTween = m_OvellaModel.DOLocalMoveY(m_OvellaModel.localPosition.y + jumpHeight, jumptDuration)
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
        StartCoroutine(EatenAnimation());
        GameManager.Instance.AnimalEaten();
    }

    private IEnumerator EatenAnimation()
    {
        m_ParticleSystem.Play();
        m_JumpTween.Kill();
        m_Collider.enabled = false;
        m_OvellaModel.GetChild(0).gameObject.SetActive(false);
        yield return new WaitWhile(m_ParticleSystem.IsAlive);
        Destroy(gameObject);
    }
}
