using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class BootstrapManager : MonoBehaviour
{
    [Inject] private readonly AuthService m_AuthService;
    [Inject] private readonly SceneLoader m_SceneLoader;

    private async void Start()
    {
        if (TokenManager.HasToken())
        {
            Debug.Log("Token found in PlayerPrefs. Redirecting to Main scene.");
            m_SceneLoader.LoadTargetScene(Config.Main_Scene);
        }
        else
        {
           
            Debug.Log("No token found. Redirecting to Login scene.");
            m_SceneLoader.LoadTargetScene(Config.Login_Scene);
        }
    }
}
