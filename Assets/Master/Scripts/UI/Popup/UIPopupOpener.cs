
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIPopupOpener : MonoBehaviour
{
    [Header("PrefabPopup")]
    [SerializeField] private PopupBase m_NotificationPopup;
    [SerializeField] private PopupBase m_LogoutPopup;

    [Inject] private readonly PopupManager m_PopupManager;
    [Inject] private readonly SignalBus m_SignalBus;


    private void OnEnable()
    {
        m_SignalBus.Subscribe<RewardClaimedSignal>(OnReward);
        m_SignalBus.Subscribe<AddAnimalResultSignal>(OnAddAnimalResult);
        m_SignalBus.Subscribe<BossChallengeClickedSignal>(OnBossChallengeClicked);
        m_SignalBus.Subscribe<LogoutRequestSignal>(OnLogOutClicked);
    }

    private void OnDisable()
    {
        m_SignalBus.TryUnsubscribe<RewardClaimedSignal>(OnReward);
        m_SignalBus.TryUnsubscribe<AddAnimalResultSignal>(OnAddAnimalResult);
        m_SignalBus.TryUnsubscribe<BossChallengeClickedSignal>(OnBossChallengeClicked);
        m_SignalBus.TryUnsubscribe<LogoutRequestSignal>(OnLogOutClicked);
    }


    public void OnReward(RewardClaimedSignal signal)
    {
        RewardPopupData data = new RewardPopupData();
        data.content = signal.Message;

        m_PopupManager.ShowPopup(m_NotificationPopup, data);
    }

    public void OnAddAnimalResult(AddAnimalResultSignal signal)
    {
        RewardPopupData data = new RewardPopupData();
        data.content = signal.Message;

        m_PopupManager.ShowPopup(m_NotificationPopup, data);
    }
    public void OnBossChallengeClicked(BossChallengeClickedSignal signal)
    {
        RewardPopupData data = new RewardPopupData();
        data.content = Config.BossChallengeComingSoon;

        m_PopupManager.ShowPopup(m_NotificationPopup, data);
    }

    public void OnLogOutClicked()
    {
        LogoutPopupData data = new LogoutPopupData
        {
            content = Config.Logout
        };

        m_PopupManager.ShowPopup(m_LogoutPopup, data);
    }


}
