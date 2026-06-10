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


    private void OnEnable()
    {
        m_SignalBus.Subscribe<LoginRequestSignal>(OnLoginRequest);
    }

    private void OnDisable()
    {
        m_SignalBus.TryUnsubscribe<LoginRequestSignal>(OnLoginRequest);
    }

    private async void OnLoginRequest(LoginRequestSignal signal)
    {
        AuthService.AuthResult result = await m_AuthService.LoginAsync(signal.UserName, signal.Password);

        if(result.IsSuccess)
        {
            m_SignalBus.Fire(new LoginSuccessSignal());
            m_SceneLoader.LoadTargetScene(Config.Main_Scene);
        }
        else
        {
            m_SignalBus.Fire(new LoginFailedSignal());
        }

    }
}
