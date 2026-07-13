using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Zenject;
public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private bool m_IsClearTokensOnStart = true;
 
    [Inject] private readonly AuthService m_AuthService;
    [Inject] private readonly SceneLoader m_SceneLoader;
    [Inject] private readonly UserDataService m_UserDataService;
    private void Awake()
    {
        
        
    }
 
    private async void Start()
    {
        if (TokenManager.HasToken())
        {
            Debug.Log("Token found in PlayerPrefs. Trying to load data...");
            await HandleUserBootstrappingAsync();
        }
        else
        {
            Debug.Log("No token found. Redirecting to Login scene.");
            await m_SceneLoader.LoadSceneWithoutLoadingBar(Config.Login_Scene);
        }
    }
    private async UniTask HandleUserBootstrappingAsync()
    {
        LoadDataResult result = await m_UserDataService.LoadAllDataAsync();
 
        if (result == LoadDataResult.Success)
        {
            await m_SceneLoader.LoadSceneWithLoadingBar(Config.Main_Scene);
            return;
        }
 
        if (result == LoadDataResult.Unauthorized)
        {
            Debug.LogWarning("Session expired. SessionManager will redirect to Login.");
            return;
        }
 
        Debug.LogError($"Fetch data failed: {result}");
        await RedirectToLoginAsync();
    }
    private async UniTask RedirectToLoginAsync()
    {
        TokenManager.ClearTokens();
        await m_SceneLoader.LoadSceneWithoutLoadingBar(Config.Login_Scene);
    }
}
