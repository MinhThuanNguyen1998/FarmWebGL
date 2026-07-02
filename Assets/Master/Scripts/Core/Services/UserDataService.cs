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
        if (await m_NetworkService.SendAuthPostStatusAsync(ApiConfig.API_ADD_ANIMAL_URL, requestBody))
        {
            Debug.Log($"Successfully added animal: '{groupName}'");
            return await LoadAnimalAsync();
        }
        return false;
    }
    public async UniTask<bool> LoadAnimalAsync()
    {
        var (result, response) = await m_NetworkService.SendGetRequestAsync<ApiLoadAnimalResponse>(ApiConfig.API_GET_LOAD_ANIMAL_URL);

        if (result == LoadDataResult.Success && response != null && response.success)
        {
            if (Data == null)
                Data = new UserData();

            Data.farm = response.data ?? new System.Collections.Generic.List<FarmAnimal>();
            m_SignalBus.Fire(new UserDataLoadedSignal(Data));
            return true;
        }
        Debug.LogError($"[UserDataService] Failed to load animal list. Result: {result}, Message: {response?.message}");
        return false;
    }
    public async UniTask<bool> ClaimRewardAsync()
    {
        var (networkSuccess, response) = await m_NetworkService
        .SendAuthPostAsync<object, ApiRewardResponse>(ApiConfig.API_POST_CLAIM_REWARD, new { });
        if (!networkSuccess || response == null || !response.status || response.data == null)
        {
            m_SignalBus.Fire(new RewardClaimedSignal(false, response?.message ?? "Network Error"));
            return false;
        }
        if (!response.data.can_claim)
        {
            m_SignalBus.Fire(new RewardClaimedSignal(false, response.message ?? "Already claimed today"));
            return false;
        }
        if (Data?.userInfo != null && !string.IsNullOrEmpty(response.data.total_amount_user))
        {
            Data.userInfo.amount = response.data.total_amount_user;
        }

        m_SignalBus.Fire(new RewardClaimedSignal(true, "Success", response.data));
        return true;
    }

    public void ResetData()
    {
        IsLoaded = false;
        Data = null;
        m_InventoryService.ResetData();
    }

    [Serializable] private class AddAnimalRequest { public string groupName; }
}