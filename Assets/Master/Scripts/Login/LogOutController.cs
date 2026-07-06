using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LogOutController : IInitializable, IDisposable
{
    [Inject] private readonly SignalBus m_SignalBus;
    [Inject] private readonly AuthService m_AuthService;
    [Inject] private readonly SceneLoader m_SceneLoader;
    public void Initialize() => m_SignalBus.Subscribe<LogoutConfirmedSignal>(HandleLogoutConfirmed);
    public void Dispose() => m_SignalBus.Unsubscribe<LogoutConfirmedSignal>(HandleLogoutConfirmed);

    private void HandleLogoutConfirmed()
    {
        ProcessLogoutRequestAsync().Forget();
    }

    private async UniTaskVoid ProcessLogoutRequestAsync()
    {
        try
        {
            await m_AuthService.LogoutAsync();
            await m_SceneLoader.LoadSceneWithLoadingBar(Config.Login_Scene);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LogoutManager] Critical error during logout process: {ex.Message}");
        }
    }
}
