
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIPopupOpener : MonoBehaviour
{
    [Header("PrefabPopup")]
    [SerializeField] private PopupBase m_Popup;

    [Inject] private readonly PopupManager m_PopupManager;
    [Inject] private readonly SignalBus m_SignalBus;


    private void OnEnable()
    {
        m_SignalBus.Subscribe<RewardClaimedSignal>(OnReward);
        m_SignalBus.Subscribe<AddAnimalResultSignal>(OnAddAnimalResult);
        m_SignalBus.Subscribe<BossChallengeClickedSignal>(OnBossChallengeClicked);
    }

    private void OnDisable()
    {
        m_SignalBus.TryUnsubscribe<RewardClaimedSignal>(OnReward);
        m_SignalBus.TryUnsubscribe<AddAnimalResultSignal>(OnAddAnimalResult);
        m_SignalBus.TryUnsubscribe<BossChallengeClickedSignal>(OnBossChallengeClicked);
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
            if (signal.Message == "Network Error")
            {
                data.content = Config.RewardFailed;
            }
            else
            {
                data.content = Config.RewardAlreadyClaimed;
            }
        }
       
        m_PopupManager.ShowPopup(m_Popup, data);
    }

    public void OnAddAnimalResult(AddAnimalResultSignal signal)
    {
        RewardPopupData data = new RewardPopupData();

        data.content = signal.IsSuccess ? Config.AddAnimalSuccess : Config.AddAnimalFailed;

        m_PopupManager.ShowPopup(m_Popup, data);
    }
    public void OnBossChallengeClicked(BossChallengeClickedSignal signal)
    {
        RewardPopupData data = new RewardPopupData();
        data.content = Config.BossChallengeComingSoon;

        m_PopupManager.ShowPopup(m_Popup, data);
    }
}
