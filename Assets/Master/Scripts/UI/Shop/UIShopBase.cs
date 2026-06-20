using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public abstract class UIShopBase : MonoBehaviour
{
    [Header("UI Elements Setup")]
    [SerializeField] protected List<UIItemQuantityElement> m_UiElements;

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
        if (m_UserDataService.IsLoaded && m_UserDataService.Data != null)
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

    protected void AddItemDataToDict(List<ItemData> items, Dictionary<string, int> dict)
    {
        if (items == null) return;
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.itemId))
                dict[item.itemId] = item.count; // Add itemId and count to the dictionary { itemId = "chicken", count = 1 }
        }
    }

    protected void RefreshUIElements()
    {
        if (m_UiElements == null) return;
        foreach (var uiElement in m_UiElements.Where(ui => ui != null))
        {
            m_CachedItemCounts.TryGetValue(uiElement.ItemId, out int count);
            uiElement.UpdateCount(count);
        }
    }
}
