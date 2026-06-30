using DG.Tweening;
using UnityEngine;
using Zenject;

public class PopupBase : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Ease m_ShowEase = Ease.OutBack;
    [SerializeField] private Ease m_HideEase = Ease.InBack;

    private float m_ShowDuration = 0.55f;
    private float m_HideDuration = 0.45f;
    private bool m_IsHiding = false;
    protected PopupManager m_PopupManager;

    [Inject]
    public void Construct(PopupManager popupManager)
    {
        this.m_PopupManager = popupManager;
    }
    public virtual void Setup(object data) { }

    public virtual void Show()
    {
        m_IsHiding = false;
        transform.DOKill();
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform
            .DOScale(Vector3.one, m_ShowDuration)
            .SetEase(m_ShowEase)
            .SetUpdate(true);
    }

    public virtual void Hide()
    {
        if (m_IsHiding) return; // Prevent calling Hide function multiple times
        m_IsHiding = true;
        transform.DOKill();
        transform
            .DOScale(Vector3.zero, m_HideDuration)
            .SetEase(m_HideEase)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                m_PopupManager?.Close();
            });
    }
}
