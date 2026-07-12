using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;
using Zenject;

/// <summary>
/// Listens for SessionExpiredSignal (fired by NetworkService when the access token is
/// rejected with 401 and cannot be refreshed) and sends the player back to the login scene.
/// The token itself is already cleared by NetworkService before this signal is fired.
/// </summary>
public class SessionManager : IInitializable, IDisposable
{
    [Inject] private readonly SignalBus m_SignalBus;
    [Inject] private readonly SceneLoader m_SceneLoader;

    private bool m_IsHandling;

    public void Initialize() => m_SignalBus.Subscribe<SessionExpiredSignal>(HandleSessionExpired);
    public void Dispose() => m_SignalBus.TryUnsubscribe<SessionExpiredSignal>(HandleSessionExpired);

    private void HandleSessionExpired()
    {
        if (m_IsHandling)
            return;

        // Already on the login screen (e.g. token expired while loading initial user data) - nothing to do.
        if (SceneManager.GetActiveScene().name == Config.Login_Scene)
            return;

        m_IsHandling = true;
        GoToLoginAsync().Forget();
    }

    private async UniTaskVoid GoToLoginAsync()
    {
        try
        {
            await m_SceneLoader.LoadSceneWithLoadingBar(Config.Login_Scene);
        }
        finally
        {
            m_IsHandling = false;
        }
    }
}
