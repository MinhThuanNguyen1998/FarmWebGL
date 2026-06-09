using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class LoginManager : MonoBehaviour
{
    [Inject] private readonly SignalBus m_SignalBus;
    [Inject] private readonly AuthService m_AuthService;

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
        }
        else
        {
            m_SignalBus.Fire(new LoginFailedSignal());
        }

    }
}
