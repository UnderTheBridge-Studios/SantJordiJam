using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PrincesaController : MonoBehaviour
{
    private Vector3 m_AutoMovement;
    private CharacterController m_CharacterMovement;

    private bool m_IsMoving;
    private float m_Angle;
    private float m_InitialVerticalPosition;

    private Tween m_JumpTween;


    [SerializeField] private float m_DracSpeed = 3;
    [SerializeField] private float m_SphereEatRadius = 5;
    [SerializeField] private Transform m_DracModel;

    private void Start()
    {
        m_InitialVerticalPosition = m_DracModel.transform.position.y;
        m_CharacterMovement = gameObject.GetComponent<CharacterController>();
        GameManager.Instance.SetPrincesaReference(this);
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
        m_CharacterMovement.SimpleMove(m_AutoMovement * m_DracSpeed);

        Vector2 pos2D = new Vector2(m_AutoMovement.x, m_AutoMovement.z);
        m_Angle = Vector2.SignedAngle(pos2D, Vector2.up);

        m_DracModel.DOLocalRotate(new Vector3(0, m_Angle, 0), 0.3f);
    }

    //Moving animation, jumping
    private void Boing()
    {
        //Is moving and the animation hasnt start
        if (m_IsMoving && m_JumpTween == null)
            m_JumpTween = m_DracModel.DOLocalMoveY(m_DracModel.localPosition.y + 1, 0.1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);

        //Is not moving but the animation havent end 
        else if (!m_IsMoving && m_JumpTween != null)
            m_JumpTween.OnStepComplete(() =>
            {
                if (m_JumpTween.CompletedLoops() % 2 == 0)
                {
                    m_JumpTween.Kill();
                    m_DracModel.position = new Vector3(m_DracModel.position.x, m_InitialVerticalPosition, m_DracModel.position.z);
                    m_JumpTween = null;
                }
            });
    }

    public float MoveToPoints(Vector3 pointPos)
    {
        float time = Vector3.Distance(pointPos, transform.position) / m_DracSpeed;
        StartCoroutine(MoveToPoint(pointPos, time));
        return time;
    }

    private IEnumerator MoveToPoint(Vector3 pointPos, float time)
    {
        m_AutoMovement = (pointPos - transform.position).normalized;
        yield return new WaitForSeconds(time);
        m_AutoMovement = Vector3.zero;
    }
}
