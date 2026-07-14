using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SessionManager : IInitializable, IDisposable
{
    [Inject] private readonly SignalBus m_SignalBus;
    [Inject] private readonly SceneLoader m_SceneLoader;
    [Inject] private readonly UserDataService m_UserDataService;

    private bool m_IsHandling;

    public void Initialize() => m_SignalBus.Subscribe<SessionExpiredSignal>(HandleSessionExpired);
    public void Dispose() => m_SignalBus.TryUnsubscribe<SessionExpiredSignal>(HandleSessionExpired);

    private void HandleSessionExpired()
    {
        if (m_IsHandling)
            return;

        m_IsHandling = true;
        m_UserDataService.ResetData();

        if (SceneManager.GetActiveScene().name == Config.Login_Scene)
        {
            m_IsHandling = false;
            return;
        }

        GoToLoginAsync().Forget();
    }

    private async UniTaskVoid GoToLoginAsync()
    {
        try
        {
            await UniTask.Yield(PlayerLoopTiming.Update);

            Debug.Log("Session expired. Redirecting to login scene...");
            await m_SceneLoader.LoadSceneWithLoadingBar(Config.Login_Scene);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error redirecting to login: {ex.Message}");
        }
        finally
        {
            m_IsHandling = false;
        }
    }
}
