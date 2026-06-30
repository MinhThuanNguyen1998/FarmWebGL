using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_UsernameInputField;
    [SerializeField] private TMP_InputField m_PasswordInputField;
    [SerializeField] private Button m_LoginButton;
    [SerializeField] private TMP_Text m_StatusText;

    [Inject] private SignalBus m_SignalBus;

    private void OnEnable()
    {
        m_StatusText.text = string.Empty;

        m_LoginButton.onClick.AddListener(OnLoginButtonClicked);
        m_SignalBus.Subscribe<LoginSuccessSignal>(OnLoginSuccess); // Listen from LoginManager
        m_SignalBus.Subscribe<LoginFailedSignal>(OnLoginFailed); // Listen from LoginManager
        m_SignalBus.Subscribe<LoginDataErrorSignal>(OnLoginDataError); // Listen from LoginManager
    }
    private void OnDisable()
    {
        m_LoginButton.onClick.RemoveListener(OnLoginButtonClicked);
        m_SignalBus.TryUnsubscribe<LoginSuccessSignal>(OnLoginSuccess); // Lister from LoginManager
        m_SignalBus.TryUnsubscribe<LoginFailedSignal>(OnLoginFailed); // Lister from LoginManager
        m_SignalBus.TryUnsubscribe<LoginDataErrorSignal>(OnLoginDataError); // Lister from LoginManager
    }
    private void OnLoginButtonClicked()
    {
        if (string.IsNullOrWhiteSpace(m_UsernameInputField.text) || string.IsNullOrWhiteSpace(m_PasswordInputField.text))
        {
            m_StatusText.text = Config.LoginEmptyFields;
            return;
        }
        SetUIState(false);
        m_StatusText.text = Config.LoginProcessing;

        m_SignalBus.Fire(new LoginRequestSignal(m_UsernameInputField.text, m_PasswordInputField.text));
    }

    private void OnLoginSuccess()
    {
        //Debug.Log("Login successful!");
        SetUIState(true);
        m_StatusText.text = Config.LoginSuccess;
        
    }
    private void OnLoginFailed(LoginFailedSignal signal)
    {
        //Debug.Log("Login failed!");
        SetUIState(true);
        m_PasswordInputField.text = string.Empty;

        // Dịch thông báo lỗi dựa trên nội dung text hoặc fallback mặc định
        string englishError = signal.ErrorMessage;
        m_StatusText.text = ErrorTranslator.GetVietnameseErrorMessage(signal.ErrorMessage);
    }
    private void OnLoginDataError()
    {
        //Debug.Log("Login data error!");
        SetUIState(true);
        m_StatusText.text = Config.DataLoadError;
    }

    private void SetUIState(bool isInteractable)
    {
        m_LoginButton.interactable = isInteractable;
        m_UsernameInputField.interactable = isInteractable;
        m_PasswordInputField.interactable = isInteractable;
    }
    
}
