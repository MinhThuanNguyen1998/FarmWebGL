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
    private async UniTask EnsureValidTokenAsync()
    {
        if (!TokenManager.HasToken())
            return;

        if (!TokenManager.IsTokenExpiringSoon(TokenManager.DEFAULT_REFRESH_THRESHOLD_MINUTES))
            return;

        bool refreshed = await RefreshAccessTokenAsync();
        if (!refreshed)
        {
            HandleSessionExpired();
        }
    }

    public async UniTask<bool> RefreshAccessTokenAsync()
    {
        string refreshToken = TokenManager.GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        try
        {
            var requestBody = new AuthService.RefreshTokenRequest { refresh_token = refreshToken };
            // isAuthenticated: false -> this call itself must not trigger EnsureValidTokenAsync again.
            var (networkSuccess, response) = await SendPostCoreAsync<AuthService.RefreshTokenRequest, AuthService.TokenResponse>(
                ApiConfig.API_POST_REFRESH_TOKEN_URL, requestBody, isAuthenticated: false);

            if (networkSuccess && response != null && response.status
                && response.data != null && !string.IsNullOrEmpty(response.data.access_token))
            {
                TokenManager.SaveTokens(
                    response.data.access_token,
                    string.IsNullOrEmpty(response.data.refresh_token) ? refreshToken : response.data.refresh_token,
                    response.data.expires_in);
                return true;
            }

            Debug.LogWarning($"[NetworkService] Refresh token failed: {response?.message}");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[NetworkService] RefreshToken error: {ex.Message}");
            return false;
        }
    }

    private void HandleSessionExpired()
    {
        if (!TokenManager.HasToken())
            return; // already handled

        TokenManager.ClearTokens();
        m_SignalBus.Fire(new SessionExpiredSignal());
    }

    private bool IsExplicitStatusFalse<TResponse>(TResponse data, bool defaultWhenUnknown)
    {
        if (data == null)
            return defaultWhenUnknown;

        foreach (var fieldName in new[] { "status", "success" })
        {
            FieldInfo field = typeof(TResponse).GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(bool))
                return !(bool)field.GetValue(data);
        }

        return defaultWhenUnknown;
    }

    private async UniTask<(bool success, TResponse data)> SendPostCoreAsync<TRequest, TResponse>(string url, TRequest body, bool isAuthenticated) where TResponse : class
    {
        try
        {
            if (isAuthenticated)
                await EnsureValidTokenAsync();

            using var request = CreateRequest(url, UnityWebRequest.kHttpVerbPOST, body, isAuthenticated);
            try { await request.SendWebRequest().ToUniTask(); } catch { /* Ignore network abort exception */ }

            bool isSuccess = request.result == UnityWebRequest.Result.Success;
            bool isProtocolError = request.result == UnityWebRequest.Result.ProtocolError;

            TResponse parsedResponse = null;
            if ((isSuccess || isProtocolError) && !string.IsNullOrEmpty(request.downloadHandler?.text))
            {
                try { parsedResponse = JsonUtility.FromJson<TResponse>(request.downloadHandler.text); }
                catch (Exception e) { Debug.LogError($"Parse error: {e.Message}"); }
            }

            // Session invalid: HTTP 401, confirmed by an explicit status/success = false when available.
            if (isAuthenticated && request.responseCode == 401 && IsExplicitStatusFalse(parsedResponse, defaultWhenUnknown: true))
            {
                HandleSessionExpired();
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
            await EnsureValidTokenAsync();

            using var request = CreateRequest(url, UnityWebRequest.kHttpVerbGET);
            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
                return (LoadDataResult.Success, JsonUtility.FromJson<T>(request.downloadHandler.text));

            LogNetworkError("GET", request);

            if (request.responseCode == 401)
            {
                HandleSessionExpired();
                return (LoadDataResult.Unauthorized, null);
            }

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
        await EnsureValidTokenAsync();

        using var request = CreateRequest(url, UnityWebRequest.kHttpVerbPOST, body);
        try { await request.SendWebRequest().ToUniTask(); } catch { }

        if (request.responseCode == 401)
            HandleSessionExpired();

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
