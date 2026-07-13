using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
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
            if (response.status && response.data != null)
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

    public async UniTask<(bool isSuccess, string message)> AddAnimalAsync(string groupName)
    {
        var requestBody = new AddAnimalRequest { animal_name = groupName };
        var (networkSuccess, response) = await m_NetworkService
            .SendAuthPostAsync<AddAnimalRequest, ApiAddAnimalResponse>(ApiConfig.API_POST_ADD_ANIMAL_URL, requestBody);

        if (networkSuccess && response != null && response.status)
        {
            Debug.Log($"Successfully added animal: '{groupName}'");
            bool loadOk = await LoadAnimalAsync();
            return (loadOk, response.message);
        }

        return (false, response?.message);
    }
    public async UniTask<bool> LoadAnimalAsync()
    {
        var (result, response) = await m_NetworkService.SendGetRequestAsync<ApiLoadAnimalResponse>(ApiConfig.API_GET_LOAD_ANIMAL_URL);

        if (result == LoadDataResult.Success && response != null && response.status)
        {
            if (Data == null)
                Data = new UserData();

            Data.farm = response.data?.farm ?? new List<FarmAnimal>();
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

        // 1. Server error or network error
        if (!networkSuccess || response == null)
        {
            m_SignalBus.Fire(new RewardClaimedSignal(false, "Network Error"));
            return false;
        }

        // 2. status == true 
        if (response.status)
        {
            if (response.data != null)
            {
                if (Data != null && Data.userInfo != null)
                {
                    Data.userInfo.amount = response.data.total_amount_user;
                }
                m_SignalBus.Fire(new RewardClaimedSignal(true, "Success", response.data));
                return true;
            }
        }
        // 3. status = false
        else
        {
            m_SignalBus.Fire(new RewardClaimedSignal(false, response.message ?? "Already claimed today"));
            return false;
        }

        return false;
    }

    public void ResetData()
    {
        IsLoaded = false;
        Data = null;
        m_InventoryService.ResetData();
    }

    [Serializable] public class AddAnimalRequest { public string animal_name; }
}