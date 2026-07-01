
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIPopupOpener : MonoBehaviour
{
    [Header("PrefabPopup")]
    [SerializeField] private PopupBase m_PopupReward;

    [Inject] private readonly PopupManager m_PopupManager;
    [Inject] private readonly SignalBus m_SignalBus;


    private void OnEnable()
    {
        m_SignalBus.Subscribe<RewardClaimedSignal>(OnReward);
    }

    private void OnDisable()
    {
        m_SignalBus.TryUnsubscribe<RewardClaimedSignal>(OnReward);
    }

    
    public void OnReward(RewardClaimedSignal signal)
    {
        RewardPopupData data = new RewardPopupData();

        if (signal.IsSuccess)
        {
            data.content = Config.RewardSuccess;
        }
        else
        {
            data.content = Config.RewardAlreadyClaimed;
        }

        m_PopupManager.ShowPopup(m_PopupReward, data);
    }
}
