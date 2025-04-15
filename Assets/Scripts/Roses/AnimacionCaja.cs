using UnityEngine;
using DG.Tweening;

public class AnimacionCaja : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cajonTransform;

    [Header("Detección")]
    [SerializeField] private float radioDeteccion = 29f;
    [SerializeField] private LayerMask manoLayer;
    [SerializeField] private float tiempoAutocerrarse = 1.5f;

    [Header("Animación")]
    [SerializeField] private float distanciaApertura = 20f;
    [SerializeField] private float duracionAnimacion = 0.3f;
    [SerializeField] private Ease easingApertura = Ease.OutBack;
    [SerializeField] private Ease easingCierre = Ease.InQuad;

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoApertura;
    [SerializeField] private AudioClip sonidoCierre;

    // Componentes
    private AudioSource audioSource;

    // Estados
    private bool estaAbierta = false;
    private Sequence secuenciaActual;
    private Tween timerAutocierre;
    private float posicionXinicial;

    private void Awake()
    {
        posicionXinicial = cajonTransform.position.x;
        // Audios
    }

    private void Update()
    {
        ComprobarProximidadMano();
    }

    private void ComprobarProximidadMano()
    {
        bool manoPresente = Physics.CheckSphere(transform.position, radioDeteccion, manoLayer);

        if (manoPresente && !estaAbierta)
        {
            AbrirCajon();
        }
        else if (!manoPresente && estaAbierta)
        {
            // Cerrar
            if (timerAutocierre == null || !timerAutocierre.IsActive())
            {
                timerAutocierre = DOVirtual.DelayedCall(tiempoAutocerrarse, CerrarCajon);
            }
        }
        else if (manoPresente && estaAbierta)
        {
            // Cancelar cierre
            if (timerAutocierre != null && timerAutocierre.IsActive())
            {
                timerAutocierre.Kill();
                timerAutocierre = null;
            }
        }
    }

    public void AbrirCajon()
    {
        if (estaAbierta || cajonTransform == null)
            return;

        LimpiarAnimaciones();

        if (sonidoApertura != null && audioSource != null)
        {
            //audioSource.PlayOneShot(sonidoApertura);
        }

        Vector3 posicionFinal = new Vector3(posicionXinicial+distanciaApertura, cajonTransform.localPosition.y, cajonTransform.localPosition.z);

        secuenciaActual = DOTween.Sequence();
        secuenciaActual.Append(
            cajonTransform.DOLocalMove(posicionFinal, duracionAnimacion)
            .SetEase(easingApertura)
        );

        secuenciaActual.OnComplete(() => {
            estaAbierta = true;
        });

        secuenciaActual.Play();
    }

    public void CerrarCajon()
    {
        if (!estaAbierta || cajonTransform == null)
            return;

        LimpiarAnimaciones();

        if (sonidoCierre != null && audioSource != null)
        {
            //audioSource.PlayOneShot(sonidoCierre);
        }

        Vector3 posicionCerrada = new Vector3(posicionXinicial, cajonTransform.localPosition.y, cajonTransform.localPosition.z);

        secuenciaActual = DOTween.Sequence();
        secuenciaActual.Append(
            cajonTransform.DOLocalMove(posicionCerrada, duracionAnimacion)
            .SetEase(easingCierre)
        );

        secuenciaActual.OnComplete(() => {
            estaAbierta = false;
        });

        secuenciaActual.Play();
    }

    private void LimpiarAnimaciones()
    {
        if (secuenciaActual != null && secuenciaActual.IsActive())
        {
            secuenciaActual.Kill();
            secuenciaActual = null;
        }

        if (timerAutocierre != null && timerAutocierre.IsActive())
        {
            timerAutocierre.Kill();
            timerAutocierre = null;
        }
    }
}