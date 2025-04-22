using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PrincesaController : MonoBehaviour
{
    [SerializeField] private float m_PrincesaSpeed = 3;
    [SerializeField] private Transform m_PrincesaModel;
    [SerializeField] private ParticleSystem m_ParticleSystem;
    [SerializeField] private Transform m_DebugTransform;

    private Vector3 m_AutoMovement;
    private CharacterController m_CharacterMovement;

    private bool m_IsMoving;
    private float m_Angle;
    private float m_InitialVerticalPosition;
    private Tween m_JumpTween;

    private void Start()
    {
        m_InitialVerticalPosition = m_PrincesaModel.transform.position.y;
        m_CharacterMovement = gameObject.GetComponent<CharacterController>();
        GameManager.Instance.SetPrincesaReference(this);
        //GetOnDrac(m_DebugTransform);
    }

    void Update()
    {
        //Animation
        Boing();

        if (m_AutoMovement != Vector3.zero)
            AutoMovement();
        else
            m_IsMoving = false;
    }

    public void AutoMovement()
    {
        m_IsMoving = true;
        m_CharacterMovement.SimpleMove(m_AutoMovement * m_PrincesaSpeed);

        Vector2 pos2D = new Vector2(m_AutoMovement.x, m_AutoMovement.z);
        m_Angle = Vector2.SignedAngle(pos2D, Vector2.up);

        m_PrincesaModel.DOLocalRotate(new Vector3(0, m_Angle, 0), 0.3f);
    }

    private void Boing()
    {
        //Is moving and the animation hasnt start
        if (m_IsMoving && m_JumpTween == null)
            m_JumpTween = m_PrincesaModel.DOLocalMoveY(m_PrincesaModel.localPosition.y + 1, 0.3f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);

        //Is not moving but the animation havent end 
        else if (!m_IsMoving && m_JumpTween != null)
            m_JumpTween.OnStepComplete(() =>
            {
                if (m_JumpTween.CompletedLoops() % 2 == 0)
                {
                    m_JumpTween.Kill();
                    m_PrincesaModel.position = new Vector3(m_PrincesaModel.position.x, m_InitialVerticalPosition, m_PrincesaModel.position.z);
                    m_JumpTween = null;
                }
            });
    }

    public float MoveToPoints(Vector3 pointPos)
    {
        float time = Vector3.Distance(pointPos, transform.position) / m_PrincesaSpeed;
        StartCoroutine(MoveToPoint(pointPos, time));
        return time;
    }

    private IEnumerator MoveToPoint(Vector3 pointPos, float time)
    {
        m_AutoMovement = (pointPos - transform.position).normalized;
        yield return new WaitForSeconds(time);
        m_AutoMovement = Vector3.zero;
    }

    public void PlayHeartsAnimation()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.hearts, this.transform.position);
        m_ParticleSystem.Play();
        m_PrincesaModel.DOLocalMoveY(m_PrincesaModel.localPosition.y + 1, 0.1f)
                .SetLoops(12, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);
    }

    public void JumpOnDrac(Transform dracTransform)
    {
        //float movementZ = dracTransform.position.z - transform.position.z + 0.7f;
        transform.SetParent(dracTransform);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(7, 1f).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOLocalMoveY(1, 1f).SetEase(Ease.OutQuad));

        transform.DOLocalMoveZ(0.7f, 1.8f).SetEase(Ease.OutQuad);
        transform.DOLocalRotate(new Vector3(0, 180, 0), 1.5f);
    }
}
