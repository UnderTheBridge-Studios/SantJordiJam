using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ClientHandAnimation : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform manoTransform;
    [SerializeField] private Client clienteController;

    [Header("Animación Idle")]
    [SerializeField] private float amplitudRotacionIdle = 5f;
    [SerializeField] private float velocidadIdle = 1f;
    [SerializeField] private Ease tipoEasingIdle = Ease.InOutSine;

    [Header("Animación Nervioso")]
    [SerializeField] private float amplitudRotacionNervioso = 15f;
    [SerializeField] private float velocidadNervioso = 2.5f;
    [SerializeField] private Ease tipoEasingNervioso = Ease.InOutQuad;
    [SerializeField] private float tiempoParaPonerseNervioso = 5f;
    [SerializeField] private float amplitudTemblor = 2f;

    // Estado
    private bool estaActivo = false;
    private bool estaNervioso = false;
    private Sequence secuenciaActual;
    private Coroutine contadorNerviosismo;
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;

    private void Awake()
    {
        // Obtener referencias si no están asignadas
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
        // Guardar posición y rotación iniciales
        posicionOriginal = manoTransform.localPosition;
        rotacionOriginal = manoTransform.localRotation;

        // Iniciar animación cuando el cliente ofrezca el billete
        StartCoroutine(IniciarAnimacionConDelay());
    }

    private IEnumerator IniciarAnimacionConDelay()
    {
        // Esperar hasta que el cliente comience a ofrecer el billete
        while (clienteController.CurrentState != Client.ClientState.Offering)
        {
            yield return null;
        }

        // Iniciar la animación de idle
        IniciarAnimacionIdle();

        // Iniciar el contador para ponerse nervioso
        contadorNerviosismo = StartCoroutine(ContadorNerviosismo());
    }

    private IEnumerator ContadorNerviosismo()
    {
        yield return new WaitForSeconds(tiempoParaPonerseNervioso);

        // Comprobar si el cliente todavía está esperando
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

            // Detener animaciones anteriores
            LimpiarAnimaciones();

            // Crear secuencia de animación idle
            secuenciaActual = DOTween.Sequence();

            // Pequeña rotación oscilante de la mano
            Quaternion rotacionInicial = rotacionOriginal;
            Quaternion rotacionDerecha = rotacionOriginal * Quaternion.Euler(0, 0, amplitudRotacionIdle);
            Quaternion rotacionIzquierda = rotacionOriginal * Quaternion.Euler(0, 0, -amplitudRotacionIdle);

            secuenciaActual.Append(manoTransform.DOLocalRotateQuaternion(rotacionDerecha, velocidadIdle / 2).SetEase(tipoEasingIdle));
            secuenciaActual.Append(manoTransform.DOLocalRotateQuaternion(rotacionIzquierda, velocidadIdle).SetEase(tipoEasingIdle));
            secuenciaActual.Append(manoTransform.DOLocalRotateQuaternion(rotacionInicial, velocidadIdle / 2).SetEase(tipoEasingIdle));

            // Hacer que la secuencia se repita indefinidamente
            secuenciaActual.SetLoops(-1);
            secuenciaActual.Play();
        }
    }

    private void CambiarANervioso()
    {
        if (!estaNervioso && estaActivo)
        {
            estaNervioso = true;

            // Detener animaciones anteriores
            LimpiarAnimaciones();

            // Crear secuencia de animación nerviosa
            secuenciaActual = DOTween.Sequence();

            // Rotación más rápida y amplia
            Quaternion rotacionInicial = rotacionOriginal;
            Quaternion rotacionDerecha = rotacionOriginal * Quaternion.Euler(0, 0, amplitudRotacionNervioso);
            Quaternion rotacionIzquierda = rotacionOriginal * Quaternion.Euler(0, 0, -amplitudRotacionNervioso);

            // Añadir también un ligero temblor en la posición
            Vector3 posicionTemblor1 = posicionOriginal + new Vector3(amplitudTemblor, 0, 0);
            Vector3 posicionTemblor2 = posicionOriginal - new Vector3(amplitudTemblor, 0, 0);

            // Combinar rotación y temblor
            secuenciaActual.Append(DOTween.Sequence()
                .Join(manoTransform.DOLocalRotateQuaternion(rotacionDerecha, velocidadNervioso / 2).SetEase(tipoEasingNervioso))
                .Join(manoTransform.DOLocalMove(posicionTemblor1, velocidadNervioso / 2).SetEase(Ease.OutQuad))
            );

            secuenciaActual.Append(DOTween.Sequence()
                .Join(manoTransform.DOLocalRotateQuaternion(rotacionIzquierda, velocidadNervioso / 2).SetEase(tipoEasingNervioso))
                .Join(manoTransform.DOLocalMove(posicionTemblor2, velocidadNervioso / 2).SetEase(Ease.OutQuad))
            );

            // Hacer que la secuencia se repita indefinidamente
            secuenciaActual.SetLoops(-1, LoopType.Yoyo);
            secuenciaActual.Play();
        }
    }

    private void Update()
    {
        // Detener las animaciones cuando el cliente ya no está ofreciendo o esperando
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

        // Restaurar posición y rotación originales
        manoTransform.localPosition = posicionOriginal;
        manoTransform.localRotation = rotacionOriginal;
    }

    private void LimpiarAnimaciones()
    {
        // Detener secuencia de animación si existe
        if (secuenciaActual != null && secuenciaActual.IsActive())
        {
            secuenciaActual.Kill();
            secuenciaActual = null;
        }

        // Detener el contador de nerviosismo si está activo
        if (contadorNerviosismo != null)
        {
            StopCoroutine(contadorNerviosismo);
            contadorNerviosismo = null;
        }
    }

    // Método público para restablecer el contador de nerviosismo (útil si el jugador interactúa pero no toma el billete)
    public void ReiniciarContadorNerviosismo()
    {
        if (estaNervioso)
        {
            estaNervioso = false;
            IniciarAnimacionIdle();
        }

        if (contadorNerviosismo != null)
        {
            StopCoroutine(contadorNerviosismo);
        }

        contadorNerviosismo = StartCoroutine(ContadorNerviosismo());
    }

    private void OnDisable()
    {
        LimpiarAnimaciones();
    }

    private void OnDestroy()
    {
        LimpiarAnimaciones();
    }
}