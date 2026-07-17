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
            return;

        TokenManager.ClearTokens();
        m_SignalBus.Fire(new SessionExpiredSignal());
    }

    private bool CheckAndHandleUnauthenticated(UnityWebRequest request, string responseText)
    {
        if (request == null)
            return false;

        if (request.responseCode == 401)
        {
            HandleSessionExpired();
            return true;
        }

        if (string.IsNullOrEmpty(responseText))
            return false;

        ApiStatusResponse parsed;
        try
        {
            parsed = JsonUtility.FromJson<ApiStatusResponse>(responseText);
        }
        catch
        {
            return false;
        }

        if (parsed != null && !parsed.status && parsed.code == 401)
        {
            HandleSessionExpired();
            return true;
        }

        return false;
    }

    private async UniTask<(bool success, TResponse data)> SendPostCoreAsync<TRequest, TResponse>(string url, TRequest body, bool isAuthenticated) where TResponse : class
    {
        var request = CreateRequest(url, UnityWebRequest.kHttpVerbPOST, body, isAuthenticated);
        try
        {
            
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            if (isAuthenticated && request.responseCode == 401)
            {
                HandleSessionExpired();
                return (false, null);
            }

            string responseText = string.Empty;
            if (request.downloadHandler != null)
            {
                responseText = request.downloadHandler.text;
            }

            if (isAuthenticated && CheckAndHandleUnauthenticated(request, responseText))
                return (false, null);

            bool isSuccess = request.result == UnityWebRequest.Result.Success;
            bool isProtocolError = request.result == UnityWebRequest.Result.ProtocolError;

            TResponse parsedResponse = null;
            if ((isSuccess || isProtocolError) && !string.IsNullOrEmpty(responseText))
            {
                try
                {
                    parsedResponse = JsonUtility.FromJson<TResponse>(responseText);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[WebGL Network] Parse POST response JSON error: {e.Message}");
                }
            }

            if (parsedResponse != null)
                return (true, parsedResponse);

            LogNetworkError("POST", request);
            return (false, null);
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("POST Request was canceled.");
            return (false, null);
        }
        catch (Exception ex)
        {
            
            Debug.LogError($"System error during POST: {ex.Message}");
            return (false, null);
        }
        finally
        {
            request?.Dispose();
        }
    }

    public async UniTask<(LoadDataResult status, T responseData)> SendGetRequestAsync<T>(string url) where T : class
    {
        var request = CreateRequest(url, UnityWebRequest.kHttpVerbGET);
        try
        {
            
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            if (request.responseCode == 401)
            {
                HandleSessionExpired();
                return (LoadDataResult.Unauthorized, null);
            }

            string responseText = string.Empty;
            if (request.downloadHandler != null)
            {
                responseText = request.downloadHandler.text;
            }

            if (CheckAndHandleUnauthenticated(request, responseText))
                return (LoadDataResult.Unauthorized, null);

            if (request.result == UnityWebRequest.Result.Success)
            {
                if (string.IsNullOrEmpty(responseText))
                {
                    Debug.LogWarning("[WebGL Network] GET Response body is empty.");
                    return (LoadDataResult.FetchError, null);
                }

                try
                {
                    T parsed = JsonUtility.FromJson<T>(responseText);
                    return (LoadDataResult.Success, parsed);
                }
                catch (Exception jsonEx)
                {
                    Debug.LogError($"[WebGL Network] GET Parse JSON failed: {jsonEx.Message}");
                    return (LoadDataResult.FetchError, null);
                }
            }

            LogNetworkError("GET", request);
            return (LoadDataResult.FetchError, null);
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("GET Request was canceled.");
            return (LoadDataResult.FetchError, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during GET: {ex.Message}");
            return (LoadDataResult.FetchError, null);
        }
        finally
        {
            request?.Dispose();
        }
    }

    public async UniTask<bool> SendAuthPostStatusAsync<TRequest>(string url, TRequest body)
    {
        var request = CreateRequest(url, UnityWebRequest.kHttpVerbPOST, body);
        try
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            if (request.responseCode == 401)
            {
                HandleSessionExpired();
                return false;
            }

            string responseText = string.Empty;
            if (request.downloadHandler != null)
            {
                responseText = request.downloadHandler.text;
            }

            CheckAndHandleUnauthenticated(request, responseText);

            return request.result == UnityWebRequest.Result.Success;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during POST Status: {ex.Message}");
            return false;
        }
        finally
        {
            request?.Dispose();
        }
    }

    public UniTask<(bool networkSuccess, TResponse responseData)> SendAuthPostAsync<TRequest, TResponse>(string url, TRequest body) where TResponse : class
        => SendPostCoreAsync<TRequest, TResponse>(url, body, isAuthenticated: true);

    public UniTask<(bool networkSuccess, TResponse responseData)> SendPostAsync<TRequest, TResponse>(string url, TRequest body) where TResponse : class
        => SendPostCoreAsync<TRequest, TResponse>(url, body, isAuthenticated: false);

    private void LogNetworkError(string method, UnityWebRequest request)
    {
        if (request == null) return;
        if (request.responseCode == 401) Debug.LogWarning("Token expired (401 Unauthorized).");
        else Debug.LogError($"API {method} Error: {request.error} (Code: {request.responseCode})");
    }
}
