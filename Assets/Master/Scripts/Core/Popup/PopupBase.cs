using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PopupBase : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Transform m_AnimTarget;
    [SerializeField] private Ease m_ShowEase = Ease.OutBack;
    [SerializeField] private Ease m_HideEase = Ease.InBack;

    private float m_ShowDuration = 0.45f;
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

    public virtual async UniTask ShowAsync()
    {
        m_IsHiding = false;

        AnimTarget.DOKill();
        AnimTarget.localScale = Vector3.zero;
        gameObject.SetActive(true);

        await AnimTarget
            .DOScale(Vector3.one, m_ShowDuration)
            .SetEase(m_ShowEase)
            .SetUpdate(true)
            .ToUniTask(TweenCancelBehaviour.Complete, this.GetCancellationTokenOnDestroy());
    }

    public virtual async UniTask HideAsync()
    {
        if (m_IsHiding) return;
        m_IsHiding = true;

        AnimTarget.DOKill();

        await AnimTarget
            .DOScale(Vector3.zero, m_HideDuration)
            .SetEase(m_HideEase)
            .SetUpdate(true)
            .ToUniTask(TweenCancelBehaviour.Complete, this.GetCancellationTokenOnDestroy());

        gameObject.SetActive(false);
        m_IsHiding = false;
    }

    public virtual void Close()
    {
        if (m_IsHiding) return;
        m_PopupManager?.CloseCurrentPopup();
    }

    protected virtual void OnDestroy()
    {
        if (m_AnimTarget != null) m_AnimTarget.DOKill();
        else transform.DOKill();
    }
}
