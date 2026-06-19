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
        if (m_UserDataService.IsLoaded)
        {
            UpdateUserInfor(m_UserDataService.Data);
        }
    }
    private void OnDisable()
    {
        m_SignalBus.Unsubscribe<UserDataLoadedSignal>(OnUserDataLoaded);
    }
    private void OnUserDataLoaded(UserDataLoadedSignal signal)
    {
        UpdateUserInfor(signal.Data);
    }

    private void UpdateUserInfor(UserData data)
    {
        m_TextMoney.text = MoneyFormatter.ToShortString(data.money);

    }
}
