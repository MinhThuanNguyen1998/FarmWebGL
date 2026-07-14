using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class NetworkService
{
    [Inject] private readonly SignalBus m_SignalBus;

    // Minimal shape used only to detect an "Unauthenticated" response as shown by the API:
    // { "status": false, "code": 401, "message": "Unauthenticated" }
    [Serializable]
    private class ApiStatusResponse
    {
        public bool status;
        public int code;
        public string message;
    }

    private UnityWebRequest CreateRequest(string url, string method, object body = null, bool isAuthenticated = true)
    {
        var request = new UnityWebRequest(url, method) { downloadHandler = new DownloadHandlerBuffer() };

        if (isAuthenticated)
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.GetAccessToken()}");

        if (body == null) return request;

        if (body is AuthService.LoginRequest loginData)
        {
            var form = new WWWForm();
            form.AddField("username", loginData.username);
            form.AddField("password", loginData.password);
            request.uploadHandler = new UploadHandlerRaw(form.data);
            foreach (var header in form.headers) request.SetRequestHeader(header.Key, header.Value);
        }
        else if (body is UserDataService.AddAnimalRequest addAnimalData)
        {
            var form = new WWWForm();
            form.AddField("animal_name", addAnimalData.animal_name);
            request.uploadHandler = new UploadHandlerRaw(form.data);
            foreach (var header in form.headers) request.SetRequestHeader(header.Key, header.Value);
        }
        else
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(body));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.SetRequestHeader("Content-Type", "application/json");
        }
        return request;
    }

    private void HandleSessionExpired()
    {
        if (!TokenManager.HasToken())
            return; // already handled

        TokenManager.ClearTokens();
        m_SignalBus.Fire(new SessionExpiredSignal());
    }

    private bool CheckAndHandleUnauthenticated(UnityWebRequest request, string responseText)
    {
        if (request.responseCode == 401)
        {
            HandleSessionExpired();
            return true;
        }

        if (string.IsNullOrEmpty(responseText))
            return false;

        ApiStatusResponse parsed;
        try { parsed = JsonUtility.FromJson<ApiStatusResponse>(responseText); }
        catch { return false; }

        if (parsed != null && !parsed.status && parsed.code == 401)
        {
            HandleSessionExpired();
            return true;
        }

        return false;
    }

    private async UniTask<(bool success, TResponse data)> SendPostCoreAsync<TRequest, TResponse>(string url, TRequest body, bool isAuthenticated) where TResponse : class
    {
        try
        {
            using var request = CreateRequest(url, UnityWebRequest.kHttpVerbPOST, body, isAuthenticated);
            try { await request.SendWebRequest().ToUniTask(); } catch { /* Ignore network abort exception */ }

            bool isSuccess = request.result == UnityWebRequest.Result.Success;
            bool isProtocolError = request.result == UnityWebRequest.Result.ProtocolError;

            string responseText = request.downloadHandler?.text;

            // 401 (HTTP status or body flag) -> clear token and go to login.
            if (isAuthenticated && CheckAndHandleUnauthenticated(request, responseText))
                return (false, null);

            TResponse parsedResponse = null;
            if ((isSuccess || isProtocolError) && !string.IsNullOrEmpty(responseText))
            {
                try { parsedResponse = JsonUtility.FromJson<TResponse>(responseText); }
                catch (Exception e) { Debug.LogError($"Parse error: {e.Message}"); }
            }

            if (parsedResponse != null)
                return (true, parsedResponse);

            LogNetworkError("POST", request);
            return (false, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during POST: {ex.Message}");
            return (false, null);
        }
    }

    public async UniTask<(LoadDataResult status, T responseData)> SendGetRequestAsync<T>(string url) where T : class
    {
        try
        {
            using var request = CreateRequest(url, UnityWebRequest.kHttpVerbGET);
            await request.SendWebRequest().ToUniTask();

            string responseText = request.downloadHandler?.text;

            // 401 (HTTP status or body flag) -> clear token and go to login.
            if (CheckAndHandleUnauthenticated(request, responseText))
                return (LoadDataResult.Unauthorized, null);

            if (request.result == UnityWebRequest.Result.Success)
                return (LoadDataResult.Success, JsonUtility.FromJson<T>(responseText));

            LogNetworkError("GET", request);
            return (LoadDataResult.FetchError, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during GET: {ex.Message}");
            return (LoadDataResult.FetchError, null);
        }
    }

    //Sends an authenticated POST request and only returns the success status
    public async UniTask<bool> SendAuthPostStatusAsync<TRequest>(string url, TRequest body)
    {
        using var request = CreateRequest(url, UnityWebRequest.kHttpVerbPOST, body);
        try { await request.SendWebRequest().ToUniTask(); } catch { }

        // 401 (HTTP status or body flag) -> clear token and go to login.
        CheckAndHandleUnauthenticated(request, request.downloadHandler?.text);

        return request.result == UnityWebRequest.Result.Success;
    }

    // Sends a POST request with Token
    public UniTask<(bool networkSuccess, TResponse responseData)> SendAuthPostAsync<TRequest, TResponse>(string url, TRequest body) where TResponse : class
        => SendPostCoreAsync<TRequest, TResponse>(url, body, isAuthenticated: true);

    //Sends a POST request WITHOUT Token
    public UniTask<(bool networkSuccess, TResponse responseData)> SendPostAsync<TRequest, TResponse>(string url, TRequest body) where TResponse : class
        => SendPostCoreAsync<TRequest, TResponse>(url, body, isAuthenticated: false);

    private void LogNetworkError(string method, UnityWebRequest request)
    {
        if (request.responseCode == 401) Debug.LogWarning("Token expired (401 Unauthorized).");
        else Debug.LogError($"API {method} Error: {request.error} (Code: {request.responseCode})");
    }
}
