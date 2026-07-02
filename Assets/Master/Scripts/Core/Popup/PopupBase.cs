using DG.Tweening;
using UnityEngine;
using Zenject;

public class PopupBase : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Transform m_AnimTarget;
    [SerializeField] private Ease m_ShowEase = Ease.OutBack;
    [SerializeField] private Ease m_HideEase = Ease.InBack;

    private float m_ShowDuration = 0.55f;
    private float m_HideDuration = 0.45f;
    private bool m_IsHiding = false;
    protected PopupManager m_PopupManager;

    private Transform AnimTarget => m_AnimTarget != null ? m_AnimTarget : transform;

    [Inject]
    public void Construct(PopupManager popupManager)
    {
        this.m_PopupManager = popupManager;
    }

    public virtual void Setup(object data) { }

    public virtual void Show()
    {
        m_IsHiding = false;

        AnimTarget.DOKill();
        AnimTarget.localScale = Vector3.zero;
        gameObject.SetActive(true);

        AnimTarget
            .DOScale(Vector3.one, m_ShowDuration)
            .SetEase(m_ShowEase)
            .SetUpdate(true);
    }

    public virtual void Hide(System.Action onHideComplete = null)
    {
        if (m_IsHiding) return;
        m_IsHiding = true;

        AnimTarget.DOKill();

        AnimTarget
            .DOScale(Vector3.zero, m_HideDuration)
            .SetEase(m_HideEase)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                m_IsHiding = false;
                onHideComplete?.Invoke();
            });
    }

    public virtual void Close()
    {
        m_PopupManager?.CloseCurrentPopup();
    }

    protected virtual void OnDestroy()
    {
        if (m_AnimTarget != null)
        {
            m_AnimTarget.DOKill();
        }
        else
        {
            transform.DOKill();
        }
    }
}
