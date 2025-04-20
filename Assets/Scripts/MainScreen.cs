using UnityEngine;
using DG.Tweening;

public class MainScreen : MonoBehaviour
{
    private CanvasGroup m_CanvasGroup;
    
    void Start()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();   
    }

    public void OnStart()
    {
        m_CanvasGroup.DOFade(0, 0.5f);
    }
}
