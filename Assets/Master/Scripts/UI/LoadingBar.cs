using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Image m_ImageComp;
    private Tween m_ProgressTween;

    public void Show()
    {
        m_ImageComp.fillAmount = 0f;
        m_CanvasGroup.alpha = 1f; 
    }

    public void SetProgress(float progress)
    {
        if (m_ProgressTween != null && m_ProgressTween.IsActive())
        {
            m_ProgressTween.Kill();
        }

        m_ProgressTween = m_ImageComp.DOFillAmount(progress, 0.2f).SetEase(Ease.OutQuad);
    }
    public void Hide()
    {
        m_CanvasGroup.DOFade(0f, 0.3f).OnComplete(() => {
            m_ImageComp.fillAmount = 0f;
        });
    }
    public void ForceHide()
    {
        m_CanvasGroup.DOKill();
        m_CanvasGroup.alpha = 0f;
        m_ImageComp.fillAmount = 0f;
    }

    private void OnDestroy()
    {
        m_ProgressTween?.Kill();
    }
}
