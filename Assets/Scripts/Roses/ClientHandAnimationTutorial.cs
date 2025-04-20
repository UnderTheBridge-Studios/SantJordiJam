using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ClientHandAnimationTutorial : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform manoTransform;
    [SerializeField] private ClientTutorial clienteController;

    [Header("Animación Idle (Arriba y Abajo)")]
    [SerializeField] private float amplitudMovimientoIdle = 0.05f;
    [SerializeField] private float velocidadIdle = 1f;
    [SerializeField] private Ease tipoEasingIdle = Ease.InOutSine;

    // Estado
    private bool estaActivo = false;
    private Sequence secuenciaActual;
    private Vector3 posicionOriginal;

    private void Awake()
    {
        if (clienteController == null)
        {
            clienteController = GetComponentInParent<ClientTutorial>();
        }

        if (manoTransform == null)
        {
            manoTransform = transform;
        }
    }

    private void Start()
    {
        StartCoroutine(IniciarAnimacionConDelay());
    }

    private IEnumerator IniciarAnimacionConDelay()
    {
        while (clienteController.CurrentState != ClientTutorial.ClientState.Waiting)
        {
            yield return null;
        }

        posicionOriginal = manoTransform.localPosition;
        IniciarAnimacionIdle();
    }

    private void IniciarAnimacionIdle()
    {
        if (!estaActivo)
        {
            estaActivo = true;

            LimpiarAnimaciones();
            secuenciaActual = DOTween.Sequence();

            Vector3 posicionArriba = posicionOriginal + new Vector3(0, amplitudMovimientoIdle, 0);
            Vector3 posicionAbajo = posicionOriginal - new Vector3(0, amplitudMovimientoIdle, 0);

            secuenciaActual.Append(manoTransform.DOLocalMove(posicionArriba, velocidadIdle / 2).SetEase(tipoEasingIdle));
            secuenciaActual.Append(manoTransform.DOLocalMove(posicionAbajo, velocidadIdle / 2).SetEase(tipoEasingIdle));

            secuenciaActual.SetLoops(-1, LoopType.Yoyo);
            secuenciaActual.Play();
        }
    }

    private void Update()
    {
        if (estaActivo &&
            clienteController.CurrentState != ClientTutorial.ClientState.Waiting)
        {
            DetenerAnimaciones();
        }
    }

    private void DetenerAnimaciones()
    {
        estaActivo = false;
        LimpiarAnimaciones();
        manoTransform.localPosition = posicionOriginal;
    }

    private void LimpiarAnimaciones()
    {
        if (secuenciaActual != null && secuenciaActual.IsActive())
        {
            secuenciaActual.Kill();
            secuenciaActual = null;
        }
    }
}