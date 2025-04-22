using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections;
using PathCreation.Examples;

public class DracController : MonoBehaviour
{
    private Vector3 m_InputVector;
    private Vector3 m_Movement;
    private Vector3 m_AutoMovement;
    private CharacterController m_CharacterMovement;

    private bool m_CanMove;
    private bool m_CanEat;
    private bool m_HaveMove; //For the tuto
    private bool m_HaveEat; //For the tuto
    private bool m_IsMoving;
    private float m_Angle;
    private float m_InitialVerticalPosition;

    private Tween m_JumpTween;


    [SerializeField] private float m_DracSpeed = 15;
    [SerializeField] private float m_SphereEatRadius = 5;
    [SerializeField] private Transform m_DracModel;

    [SerializeField] private LayerMask m_AnimalLayer;
    [SerializeField] private PathFollower m_PathFollower;


    private void Start()
    {
        m_InitialVerticalPosition = m_DracModel.transform.position.y;
        m_CanMove = m_CanEat = false;
        m_HaveMove = m_HaveEat = false;
        m_CharacterMovement = gameObject.GetComponent<CharacterController>();
        GameManager.Instance.SetDracReference(this);
    }

    void Update()
    {
        //Animation
        Boing();
        
        if (m_AutoMovement != Vector3.zero)
            AutoMovement();
        //If the player can has control
        else if (m_InputVector != Vector3.zero && m_CanMove)
            Movement();
        else
            m_IsMoving = false;
    }

    public void Movement()
    {

        if (!m_HaveMove)
        {
            m_HaveMove = true;
            Invoke("HideMoveTuto", 2f);
        }

        m_IsMoving = true;
        m_Movement = new Vector3(-m_InputVector.y, 0, m_InputVector.x) * m_DracSpeed;
        m_CharacterMovement.SimpleMove(m_Movement);

        m_Angle = Vector2.SignedAngle(m_InputVector, Vector2.up) - 90;

        m_DracModel.DOLocalRotate(new Vector3(0, m_Angle, 0), 0.3f);
    }

    private void HideMoveTuto()
    {
        GameManager.Instance.HideTuto(Tutorial.wasd);
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

    public void EnableControl(bool value)
    {
        m_CanMove = value;
    }

    public void EnableEat(bool value)
    {
        m_CanEat = value;
    }


    #region Inputs

    public void OnMove(InputAction.CallbackContext context)
    {
        m_InputVector = context.ReadValue<Vector2>();
    }

    public void OnEat(InputAction.CallbackContext context)
    {
        if (!context.performed || !m_CanEat)
            return;

        if (!m_HaveEat){
            m_HaveEat = true;
            Invoke("HideEatTuto", 2f);
        }
            

        Collider[] hitColliders = Physics.OverlapSphere(transform.position + (gameObject.transform.GetChild(0).transform.forward * 3.5f), m_SphereEatRadius, m_AnimalLayer);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.GetComponent<IEdable>().OnEat();
            break;  //Nomes pot menjar una ovella alhora
        }
        EatAnimation();
    }

    void EatAnimation()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.eat, this.transform.position);
        m_CanEat = m_CanMove = false;
        m_DracModel.DOLocalMoveY(m_DracModel.localPosition.y + 1, 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);

        m_DracModel.DOLocalRotate(m_DracModel.localEulerAngles + new Vector3(30f, 0f, 0f), 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    m_DracModel.position = new Vector3(m_DracModel.position.x, m_InitialVerticalPosition, m_DracModel.position.z);
                    m_CanEat = m_CanMove = true;
                });
    }

    private void HideEatTuto()
    {
        GameManager.Instance.HideTuto(Tutorial.espai);
    }


    #endregion

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + (gameObject.transform.GetChild(0).transform.forward * 3.5f), m_SphereEatRadius);
    }

    public void CameraOut()
    {
        Transform camera = GetComponentInChildren<Camera>().transform;
        camera.SetParent(transform.parent);
    }

    public void FlyAway()
    {
        Debug.Log("FlyAway!");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.outro, this.transform.position);
        m_PathFollower.enabled = true;
    }

}
