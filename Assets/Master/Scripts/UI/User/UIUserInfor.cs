using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class UIUserInfor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextMoney;
    [Inject] private SignalBus m_SignalBus;
    [Inject] private UserDataService m_UserDataService;
    private void OnEnable()
    {
        m_SignalBus.Subscribe<UserDataLoadedSignal>(OnUserDataLoaded);
        m_SignalBus.Subscribe<RewardClaimedSignal>(OnRewardClaimed);
        if (m_UserDataService.IsLoaded)
        {
            UpdateUserInfor(m_UserDataService.Data);
        }
    }
    private void OnDisable()
    {
        m_SignalBus.Unsubscribe<UserDataLoadedSignal>(OnUserDataLoaded);
        m_SignalBus.Unsubscribe<RewardClaimedSignal>(OnRewardClaimed);
    }
    private void OnUserDataLoaded(UserDataLoadedSignal signal)
    {
        UpdateUserInfor(signal.Data);
    }
    private void OnRewardClaimed(RewardClaimedSignal signal)
    {
        if (signal.IsSuccess && signal.Data != null)
        {
            //string rawMoney = signal.Data.total_amount_user; 
            //m_TextMoney.text = MoneyFormatter.ParseAndFormat(rawMoney);
            UpdateUserInfor(m_UserDataService.Data);
        }
    }
    private void UpdateUserInfor(UserData data)
    {
        m_TextMoney.text = MoneyFormatter.ToShortString(data.money);

    }
}
