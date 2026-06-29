using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
public class UserDataService
{
    public UserData Data { get; private set; }
    public bool IsLoaded { get; private set; }

    [Inject] private readonly SignalBus m_SignalBus;
    [Inject] private readonly NetworkService m_NetworkService;
    [Inject] private readonly InventoryService m_InventoryService;

    public async UniTask<LoadDataResult> LoadAllDataAsync()
    {
        var (result, response) = await m_NetworkService.SendGetRequestAsync<ApiUserDataResponse>(ApiConfig.API_GET_USER_DATA_URL);

        if (result == LoadDataResult.Success && response != null)
        {
            if (response.success && response.data != null)
            {
                Data = new UserData
                {
                    userInfo = response.data.user,
                    farm = response.data.farm
                };
                IsLoaded = true;
                m_SignalBus.Fire(new UserDataLoadedSignal(Data));
            }
            else
            {
                Debug.LogError("[UserDataService] API success=false or data null.");
                return LoadDataResult.FetchError;
            }
        }
        else
        {
            return result;
        }

        // Load inventory
        var inventoryResult = await m_InventoryService.LoadInventoryAsync();
        if (inventoryResult == LoadDataResult.Success)
        {
            m_SignalBus.Fire(new InventoryLoadedSignal(m_InventoryService.Items));
        }
        else
        {
            Debug.LogWarning($"[UserDataService] Inventory load failed: {inventoryResult}");
        }

        return result;
    }

    public async UniTask<bool> AddAnimalAsync(string groupName)
    {
        var requestBody = new AddAnimalRequest { groupName = groupName };
        if (await m_NetworkService.SendPostRequestAsync(ApiConfig.API_ADD_ANIMAL_URL, requestBody))
        {
            Debug.Log($"Successfully added animal: '{groupName}'");
            return await LoadAllDataAsync() == LoadDataResult.Success;
        }
        return false;
    }

    public void ResetData()
    {
        IsLoaded = false;
        Data = null;
        m_InventoryService.ResetData();
    }

    [Serializable] private class AddAnimalRequest { public string groupName; }
}