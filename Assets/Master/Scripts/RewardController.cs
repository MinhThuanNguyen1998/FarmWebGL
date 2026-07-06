using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RewardController : IInitializable, IDisposable
{
    [Inject] private readonly SignalBus m_SignalBus;
    [Inject] private readonly UserDataService m_UserDataService;

    public void Initialize() => m_SignalBus.Subscribe<RewardRequestSignal>(HandleRewardRequest);
    public void Dispose() => m_SignalBus.Unsubscribe<RewardRequestSignal>(HandleRewardRequest);

    private void HandleRewardRequest()
    {
        ProcessClaimRewardAsync().Forget();
    }

    private async UniTaskVoid ProcessClaimRewardAsync()
    {
        try
        {
            await m_UserDataService.ClaimRewardAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[RewardController] Critical error during claim reward process: {ex.Message}");
        }
    }
}
