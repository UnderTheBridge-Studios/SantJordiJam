using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class DracController : MonoBehaviour
{
    private Vector3 m_InputVector;
    private Vector3 m_Movement;
    private CharacterController m_CharacterMovement;

    private bool m_CanMove;
    private float m_angle;
    private float m_initialVerticalPosition;

    private Tween jumpTween;


    [SerializeField] private float m_dracSpeed = 15;
    [SerializeField] private Transform m_dracModel;


    private void Start()
    {
        m_initialVerticalPosition = m_dracModel.transform.position.y;
        m_CanMove = true;
        m_CharacterMovement = gameObject.GetComponent<CharacterController>();

    }

    void Update()
    {
        if (m_InputVector != Vector3.zero && m_CanMove)
            Movement();
        else if (jumpTween != null)
        {
            jumpTween.OnStepComplete(() =>
            {
                if(jumpTween.CompletedLoops() % 2 == 0)
                {
                    jumpTween.Kill();
                    m_dracModel.position = new Vector3(m_dracModel.position.x, m_initialVerticalPosition, m_dracModel.position.z);
                    jumpTween = null;
                }                    
            });
        }
    }

    public void Movement()
    {
        m_Movement = new Vector3(-m_InputVector.y, 0, m_InputVector.x) * m_dracSpeed;
        m_CharacterMovement.SimpleMove(m_Movement);

        //Crida animacio aquí
        m_angle = Vector2.SignedAngle(m_InputVector, Vector2.up);
        m_dracModel.DOLocalRotate(new Vector3(0, m_angle, 0), 0.3f);

        if (jumpTween == null)
            jumpTween = m_dracModel.DOLocalMoveY(m_dracModel.localPosition.y + 1, 0.1f)
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
