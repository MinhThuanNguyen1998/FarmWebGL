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
    protected InventoryService m_InventoryService;

    protected readonly List<TItem> m_ActiveItems = new List<TItem>();

    [Inject]
    public void Construct(SignalBus signalBus, InventoryService inventoryService, TPool pool)
    {
        m_SignalBus = signalBus;
        m_InventoryService = inventoryService;
        m_Pool = pool;
    }

    protected virtual void OnEnable()
    {
        m_SignalBus.Subscribe<InventoryLoadedSignal>(OnInventoryLoaded);
        if (m_InventoryService.IsLoaded)
            UpdateUI(m_InventoryService.Items);
    }

    protected virtual void OnDisable()
    {
        m_SignalBus.Unsubscribe<InventoryLoadedSignal>(OnInventoryLoaded);
    }

    private void OnInventoryLoaded(InventoryLoadedSignal signal) => UpdateUI(signal.Items);

    protected abstract void UpdateUI(List<InventoryItem> items);

    protected void DespawnAll()
    {
        foreach (var item in m_ActiveItems)
            DespawnItem(item);
        m_ActiveItems.Clear();
    }

    protected abstract void DespawnItem(TItem item);

    protected virtual void OnDestroy() => DespawnAll();
}
