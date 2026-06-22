using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public abstract class UIShopBase<TItem, TPool> : MonoBehaviour
    where TItem : MonoBehaviour
    where TPool : IMemoryPool
{
    protected TPool m_Pool;
    protected SignalBus m_SignalBus;
    protected UserDataService m_UserDataService;

    protected readonly List<TItem> m_ActiveItems = new List<TItem>();

    [Inject]
    public void Construct(SignalBus signalBus, UserDataService userDataService, TPool pool)
    {
        m_SignalBus = signalBus;
        m_UserDataService = userDataService;
        m_Pool = pool;
    }

    protected virtual void OnEnable()
    {
        m_SignalBus.Subscribe<UserDataLoadedSignal>(OnUserDataLoaded);
        if (m_UserDataService.IsLoaded)
            UpdateUI(m_UserDataService.Data);
    }

    protected virtual void OnDisable()
    {
        m_SignalBus.Unsubscribe<UserDataLoadedSignal>(OnUserDataLoaded);
    }

    private void OnUserDataLoaded(UserDataLoadedSignal signal) => UpdateUI(signal.Data);

    protected abstract void UpdateUI(UserData data);

    protected void DespawnAll()
    {
        foreach (var item in m_ActiveItems)
            DespawnItem(item);
        m_ActiveItems.Clear();
    }

    protected abstract void DespawnItem(TItem item);

    protected virtual void OnDestroy() => DespawnAll();
}
