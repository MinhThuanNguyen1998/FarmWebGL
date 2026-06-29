using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkService
{
    /// <summary>
    /// Creates a UnityWebRequest with an Authorization Bearer token header.
    /// </summary>
    private UnityWebRequest CreateAuthenticatedRequest(string url, string method)
    {
        var request = new UnityWebRequest(url, method);
        request.SetRequestHeader("Authorization", $"Bearer {TokenManager.GetAccessToken()}");
        return request;
    }

    /// <summary>
    /// Creates a standard UnityWebRequest without authentication headers.
    /// </summary>
    private UnityWebRequest CreateUnauthenticatedRequest(string url, string method)
    {
        return new UnityWebRequest(url, method);
    }

    /// <summary>
    /// Sends an authenticated GET request to endpoints requiring a token.
    /// </summary>
    public async UniTask<(LoadDataResult status, T responseData)> SendGetRequestAsync<T>(string url) where T : class
    {
        try
        {
            using var request = CreateAuthenticatedRequest(url, UnityWebRequest.kHttpVerbGET);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<T>(request.downloadHandler.text);
                return (LoadDataResult.Success, response);
            }

            if (request.responseCode == 401)
                Debug.LogWarning("Token expired (401 Unauthorized).");
            else
                Debug.LogError($"API GET Error: {request.error} (Code: {request.responseCode})");

            return (request.responseCode == 401 ? LoadDataResult.Unauthorized : LoadDataResult.FetchError, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during GET: {ex.Message}");
            return (LoadDataResult.FetchError, null);
        }
    }

    /// <summary>
    /// Sends an authenticated POST request for actions requiring a token (e.g., purchasing items).
    /// </summary>
    public async UniTask<bool> SendPostRequestAsync<TRequest>(string url, TRequest body)
    {
        try
        {
            using var request = CreateAuthenticatedRequest(url, UnityWebRequest.kHttpVerbPOST);
            string jsonBody = JsonUtility.ToJson(body);

            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest().ToUniTask();
            return request.result == UnityWebRequest.Result.Success;
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during POST: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Sends an unauthenticated POST request that returns a response body (e.g., Login).
    /// Dynamically appends username/password as URL query parameters if they exist in the request body.
    /// </summary>
    public async UniTask<(bool networkSuccess, TResponse responseData)> SendPublicPostRequestAsync<TRequest, TResponse>(
        string url, TRequest body) where TResponse : class
    {
       try
        {
            // Instead of checking the URL/domain, we check the type of the Request Model.
            // If the model is explicitly a LoginRequest, we extract fields and append them to the URL.
            if (body is AuthService.LoginRequest loginData)
            {
                string separator = url.Contains("?") ? "&" : "?";
                url = $"{url}{separator}username={UnityWebRequest.EscapeURL(loginData.username)}&password={UnityWebRequest.EscapeURL(loginData.password)}";
            }

            using var request = CreateUnauthenticatedRequest(url, UnityWebRequest.kHttpVerbPOST);
            request.downloadHandler = new DownloadHandlerBuffer();

            // If it's NOT a login request, we attach the standard JSON body payload
            if (!(body is AuthService.LoginRequest))
            {
                string jsonBody = JsonUtility.ToJson(body);
                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
                request.SetRequestHeader("Content-Type", "application/json");
            }

            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<TResponse>(request.downloadHandler.text);
                return (true, response);
            }

            Debug.LogError($"API POST Error: {request.error} (Code: {request.responseCode})");
            return (false, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during public POST: {ex.Message}");
            return (false, null);
        }
    }
}
