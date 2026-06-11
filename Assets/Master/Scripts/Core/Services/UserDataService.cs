using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class UserDataService
{
    public async UniTask<bool> LoadAllDataAsync()
    {
        string url = ApiConfig.API_DATA_URL;
        string token = TokenManager.GetAccessToken();
        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
                await request.SendWebRequest().ToUniTask();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = request.downloadHandler.text;
                    UserData data = JsonUtility.FromJson<UserData>(jsonResponse);
                    //Debug.Log("Money: " + data.money);
                    Debug.Log($"Raw JSON response: {jsonResponse}");
                    return true;
                }
                else
                {
                    Debug.LogError($"API Error: {request.error}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading user data: {ex.Message}");
            return false;
        }
    }
}
