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
    /// Authenticated GET request — used for any API endpoint requiring a token.
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
    /// Authenticated POST request — used for actions requiring a token (e.g., purchasing items).
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
    /// Unauthenticated POST request with response body — used for Login (no token yet).
    /// Automatically handles routing for both Production Server (URL Params) and Mockoon (JSON Body).
    /// </summary>
    public async UniTask<(bool networkSuccess, TResponse responseData)> SendPublicPostRequestAsync<TRequest, TResponse>(
        string url, TRequest body) where TResponse : class
    {
        try
        {
            UnityWebRequest request = null;

            // Check if the request is a LoginRequest to apply specific environment logic
            if (body is AuthService.LoginRequest loginData)
            {
                // If it is the production server, pass data via Query Parameters
                if (url.Contains("homagame.com"))
                {
                    url = $"{url}?username={UnityWebRequest.EscapeURL(loginData.username)}&password={UnityWebRequest.EscapeURL(loginData.password)}";
                    request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
                }
                // If it is Mockoon/Localhost, keep sending data via JSON Body as originally configured
                else
                {
                    request = CreateUnauthenticatedRequest(url, UnityWebRequest.kHttpVerbPOST);
                    string jsonBody = JsonUtility.ToJson(body);
                    request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
                    request.SetRequestHeader("Content-Type", "application/json");
                }
            }
            else
            {
                // Fallback for any other generic public POST requests
                request = CreateUnauthenticatedRequest(url, UnityWebRequest.kHttpVerbPOST);
                string jsonBody = JsonUtility.ToJson(body);
                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
                request.SetRequestHeader("Content-Type", "application/json");
            }

            // Wrap in a try-finally block via standard utilizing logic to guarantee disposal
            try
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                await request.SendWebRequest().ToUniTask();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<TResponse>(request.downloadHandler.text);
                    return (true, response);
                }

                Debug.LogError($"API POST Error: {request.error} (Code: {request.responseCode})");
                return (false, null);
            }
            finally
            {
                request?.Dispose();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during public POST: {ex.Message}");
            return (false, null);
        }
    }
}
