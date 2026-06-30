using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Zenject;
public class LoginManager : MonoBehaviour
{
    [Inject] private readonly SignalBus m_SignalBus;
    [Inject] private readonly AuthService m_AuthService;
    [Inject] private readonly SceneLoader m_SceneLoader;
    [Inject] private readonly UserDataService m_UserDataService;

    private void OnEnable()
    {
        m_SignalBus.Subscribe<LoginRequestSignal>(HandleLoginRequest);
    }

    private void OnDisable()
    {
        m_SignalBus.TryUnsubscribe<LoginRequestSignal>(HandleLoginRequest);
    }
    private void HandleLoginRequest(LoginRequestSignal signal) => ProgressLoginRequest(signal).Forget(); // foret the async method since we don't need to await it here.
    private async UniTaskVoid ProgressLoginRequest(LoginRequestSignal signal)
    {
        try
        {
            var authResult = await m_AuthService.LoginAsync(signal.UserName, signal.Password);

            if (!authResult.IsSuccess)
            {
                m_SignalBus.Fire(new LoginFailedSignal(authResult.ErrorMessage));
                return; 
            }

            LoadDataResult dataResult = await m_UserDataService.LoadAllDataAsync();

            if (dataResult == LoadDataResult.Success)
            {
                m_SignalBus.Fire(new LoginSuccessSignal());
                await m_SceneLoader.LoadSceneWithLoadingBar(Config.Main_Scene);
            }
            else
            {
                if (dataResult == LoadDataResult.Unauthorized)
                {
                    TokenManager.ClearTokens();
                }

                m_SignalBus.Fire(new LoginDataErrorSignal());
                
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LoginManager] Critical error during login process: {ex.Message}");
            m_SignalBus.Fire(new LoginFailedSignal(Config.ServerError));
            
        }
    }


}
