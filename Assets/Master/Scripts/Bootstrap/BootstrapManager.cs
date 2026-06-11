using Cysharp.Threading.Tasks;
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
        if (m_IsClearTokensOnStart) TokenManager.ClearTokens();
        
    }

    private async void Start()
    {
        if (TokenManager.HasToken())
        {
            Debug.Log("Token found in PlayerPrefs. Redirecting to Main scene.");
            bool dataLoaded = await m_UserDataService.LoadAllDataAsync();
            if (dataLoaded)
            {
                await m_SceneLoader.LoadSceneWithLoadingBar(Config.Main_Scene);
            }
            else
            {
                Debug.LogError("Failed to load user data.");
            }
        }
        else
        {
           
            Debug.Log("No token found. Redirecting to Login scene.");
            await m_SceneLoader.LoadSceneWithoutLoadingBar(Config.Login_Scene);
        }
    }
}
