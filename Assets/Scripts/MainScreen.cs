using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class MainScreen : MonoBehaviour
{
    private CanvasGroup m_CanvasGroup;
    [SerializeField] private Mask m_mask;
    private bool hasSounded = false;

    void Start()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_mask.enabled = false;
    }

    public void OnStart()
    {
        if (!hasSounded)
        {
            if (AudioManager.instance != null)
            {
                //AudioManager.instance.InitializeSound();
                hasSounded = true;
            }
        }
        m_mask.enabled = true;
        Cursor.visible = false;
        StartCoroutine(ShowTuto());
    }

    private IEnumerator ShowTuto()
    {
        yield return new WaitForSeconds(2f);
        m_CanvasGroup.DOFade(0, 1f);
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ShowTuto(Tutorial.click_rosa);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

}
