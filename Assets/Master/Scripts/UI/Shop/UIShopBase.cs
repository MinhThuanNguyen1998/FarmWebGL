using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public abstract class UIShopBase<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("UI Elements Setup")]
    protected List<T> m_UIItem = new List<T>();

    protected SignalBus m_SignalBus;
    protected UserDataService m_UserDataService;
    protected readonly Dictionary<string, int> m_CachedItemCounts = new Dictionary<string, int>();

    [Inject]
    public void Construct(SignalBus signalBus, UserDataService userDataService)
    {
        m_SignalBus = signalBus;
        m_UserDataService = userDataService;
    }
    protected virtual void OnEnable()
    {
        m_SignalBus.Subscribe<UserDataLoadedSignal>(OnUserDataLoaded);
        if (m_UserDataService.IsLoaded)
        {
            UpdateUI(m_UserDataService.Data);
        }
    }

    protected virtual void OnDisable()
    {
        m_SignalBus.Unsubscribe<UserDataLoadedSignal>(OnUserDataLoaded);
    }

    private void OnUserDataLoaded(UserDataLoadedSignal signal)
    {
        UpdateUI(signal.Data);
    }

    protected abstract void UpdateUI(UserData data);

   
}
