using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class DracController : MonoBehaviour
{
    private Vector3 m_InputVector;
    private Vector3 m_Movement;
    private CharacterController m_CharacterMovement;

    private bool m_CanMove;
    private float m_Angle;
    private float m_InitialVerticalPosition;

    private Tween m_JumpTween;


    [SerializeField] private float m_DracSpeed = 15;
    [SerializeField] private Transform m_DracModel;


    private void Start()
    {
        m_InitialVerticalPosition = m_DracModel.transform.position.y;
        m_CanMove = true;
        m_CharacterMovement = gameObject.GetComponent<CharacterController>();

    }

    void Update()
    {
        if (m_InputVector != Vector3.zero && m_CanMove)
            Movement();
        else if (m_JumpTween != null)
        {
            m_JumpTween.OnStepComplete(() =>
            {
                if(m_JumpTween.CompletedLoops() % 2 == 0)
                {
                    m_JumpTween.Kill();
                    m_DracModel.position = new Vector3(m_DracModel.position.x, m_InitialVerticalPosition, m_DracModel.position.z);
                    m_JumpTween = null;
                }                    
            });
        }
    }

    public void Movement()
    {
        m_Movement = new Vector3(-m_InputVector.y, 0, m_InputVector.x) * m_DracSpeed;
        m_CharacterMovement.SimpleMove(m_Movement);

        //Crida animacio aqu�
        m_Angle = Vector2.SignedAngle(m_InputVector, Vector2.up);
        m_DracModel.DOLocalRotate(new Vector3(0, m_Angle, 0), 0.3f);

        if (m_JumpTween == null)
            m_JumpTween = m_DracModel.DOLocalMoveY(m_DracModel.localPosition.y + 1, 0.1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_InputVector = context.ReadValue<Vector2>();
    }


    public void OnEat(InputAction.CallbackContext context)
    {
        if (context.performed)
            Debug.Log("Nyam!");
    }
}
