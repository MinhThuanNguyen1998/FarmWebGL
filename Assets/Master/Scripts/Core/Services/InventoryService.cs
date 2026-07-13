using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InventoryService 
{
    public bool IsLoaded { get; private set; }

    public List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();

    [Inject] private readonly NetworkService m_NetworkService;
    [Inject] private readonly SignalBus m_SignalBus;

    public async UniTask<LoadDataResult> LoadInventoryAsync()
    {
        string url = ApiConfig.API_GET_INVENTORY_URL;

        var (result, response) = await m_NetworkService.SendGetRequestAsync<ApiInventoryResponse>(url);

        if (result == LoadDataResult.Success && response != null)
        {
            if (response.status && response.data != null)
            {
                Items = response.data.items ?? new List<InventoryItem>();
                IsLoaded = true;

                Debug.Log($"[InventoryService] Loaded {Items.Count} types of items.");

                m_SignalBus.Fire(new InventoryLoadedSignal(Items));
            }
            else
            {
                Debug.LogError("[InventoryService] API status = false orr data is null.");
                return LoadDataResult.FetchError;
            }
        }
        else
        {
            Debug.LogWarning($"[InventoryService] Failed to load inventory (API_GET_INVENTORY_URL). Result: {result}");
        }
        return result;
    }

    public void ResetData()
    {
        IsLoaded = false;
        Items.Clear();
    }
}
