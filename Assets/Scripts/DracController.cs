using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections;

public class DracController : MonoBehaviour
{
    private Vector3 m_InputVector;
    private Vector3 m_Movement;
    private CharacterController m_CharacterMovement;

    private bool m_CanMove;
    private bool m_IsMoving;
    private float m_Angle;
    private float m_InitialVerticalPosition;

    private Tween m_JumpTween;


    [SerializeField] private float m_DracSpeed = 15;
    [SerializeField] private float m_SphereEatRadius = 5;
    [SerializeField] private Transform m_DracModel;

    [SerializeField] private LayerMask m_AnimalLayer;



    private void Start()
    {
        m_InitialVerticalPosition = m_DracModel.transform.position.y;
        m_CanMove = true;
        m_CharacterMovement = gameObject.GetComponent<CharacterController>();
        GameManager.Instance.SetDracReference(this);
    }

    void Update()
    {
        Boing();

        //If the player can has control, (the dragon con move alone)
        if (m_InputVector != Vector3.zero && m_CanMove)
            Movement();
        else
            m_IsMoving = false;
    }

    public void Movement()
    {
        m_IsMoving = true;
        m_Movement = new Vector3(-m_InputVector.y, 0, m_InputVector.x) * m_DracSpeed;
        m_CharacterMovement.SimpleMove(m_Movement);

        m_Angle = Vector2.SignedAngle(m_InputVector, Vector2.up) - 90;
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

    public void MoveToPoints(Transform point)
    {
        StartCoroutine(MoveToPoint(point));
    }

    private IEnumerator MoveToPoint(Transform point)
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Drac: " + transform.position + " pointPos: " + point.position);
    }

    public void EnableControl(bool value)
    {
        m_CanMove = value;
    }


    #region Inputs

    public void OnMove(InputAction.CallbackContext context)
    {
        m_InputVector = context.ReadValue<Vector2>();
    }

    public void OnEat(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position + (gameObject.transform.GetChild(0).transform.forward * 3.5f), m_SphereEatRadius, m_AnimalLayer);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.GetComponent<IEdable>().OnEat();
            break;  //Nomes pot menjar una ovella alhora
        }
    }

    #endregion
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + (gameObject.transform.GetChild(0).transform.forward * 3.5f), m_SphereEatRadius);
    }

}
