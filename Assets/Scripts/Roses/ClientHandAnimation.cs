using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ClientHandAnimation : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform manoTransform;
    [SerializeField] private Client clienteController;

    [Header("Animación Idle (Arriba y Abajo)")]
    [SerializeField] private float amplitudMovimientoIdle = 0.05f;
    [SerializeField] private float velocidadIdle = 1f;
    [SerializeField] private Ease tipoEasingIdle = Ease.InOutSine;

    [Header("Animación Nervioso (Izquierda y Derecha)")]
    [SerializeField] private float amplitudMovimientoNervioso = 0.08f;
    [SerializeField] private float velocidadNervioso = 2.5f;
    [SerializeField] private Ease tipoEasingNervioso = Ease.InOutQuad;
    [SerializeField] private float tiempoParaPonerseNervioso = 5f;

    // Estado
    private bool estaActivo = false;
    private bool estaNervioso = false;
    private Sequence secuenciaActual;
    private Coroutine contadorNerviosismo;
    private Vector3 posicionOriginal;

    private void Awake()
    {
        if (clienteController == null)
        {
            clienteController = GetComponentInParent<Client>();
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
        while (clienteController.CurrentState != Client.ClientState.Offering && clienteController.CurrentState != Client.ClientState.Waiting)
        {
            yield return null;
        }

        posicionOriginal = manoTransform.localPosition;
        IniciarAnimacionIdle();
        contadorNerviosismo = StartCoroutine(ContadorNerviosismo());
    }

    private IEnumerator ContadorNerviosismo()
    {
        yield return new WaitForSeconds(tiempoParaPonerseNervioso);

        if (clienteController.CurrentState == Client.ClientState.Offering ||
            clienteController.CurrentState == Client.ClientState.Waiting)
        {
            CambiarANervioso();
        }
    }

    private void IniciarAnimacionIdle()
    {
        if (!estaActivo)
        {
            estaActivo = true;
            estaNervioso = false;

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

    private void CambiarANervioso()
    {
        if (!estaNervioso && estaActivo)
        {
            estaNervioso = true;
            LimpiarAnimaciones();
            secuenciaActual = DOTween.Sequence();

            Vector3 posicionIzquierda = posicionOriginal - new Vector3(0, 0, amplitudMovimientoNervioso);
            Vector3 posicionDerecha = posicionOriginal + new Vector3(0, 0, amplitudMovimientoNervioso);

            secuenciaActual.Append(manoTransform.DOLocalMove(posicionIzquierda, velocidadNervioso / 2).SetEase(tipoEasingNervioso));
            secuenciaActual.Append(manoTransform.DOLocalMove(posicionDerecha, velocidadNervioso / 2).SetEase(tipoEasingNervioso));

            secuenciaActual.SetLoops(-1, LoopType.Yoyo);
            secuenciaActual.Play();
        }
    }

    private void Update()
    {
        if (estaActivo &&
            clienteController.CurrentState != Client.ClientState.Offering &&
            clienteController.CurrentState != Client.ClientState.Waiting)
        {
            DetenerAnimaciones();
        }
    }

    private void DetenerAnimaciones()
    {
        estaActivo = false;
        estaNervioso = false;
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

        if (contadorNerviosismo != null)
        {
            StopCoroutine(contadorNerviosismo);
            contadorNerviosismo = null;
        }
    }
}