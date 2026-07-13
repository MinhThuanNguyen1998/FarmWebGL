using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
public class AuthService 
{
    [Inject] private readonly NetworkService m_NetworkService;

    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public TokenResponse Data { get; set; }
    }

    [Serializable]
    public class UserData
    {
        public string username;
    }

    [Serializable]
    public class LoginDataContent
    {
        public string access_token;
        public string refresh_token;
        public string token_type;
        public int expires_in;
        public UserData user;
    }

    [Serializable]
    public class TokenResponse
    {
        public bool status;
        public string message;
        public LoginDataContent data;
    }

    [Serializable]
    public class LogoutResponse
    {
        public bool status;
        public string message;
    }

    [Serializable]
    public class RefreshTokenRequest
    {
        public string refresh_token;
    }
    public async UniTask<AuthResult> LoginAsync(string username, string password)
    {
        try
        {
            var requestBody = new LoginRequest { username = username, password = password };
            var (networkSuccess, response) = await m_NetworkService.SendPostAsync<LoginRequest, TokenResponse>(
                ApiConfig.API_AUTH_URL, requestBody);

            // Server Error
            if (!networkSuccess)
                return new AuthResult { IsSuccess = false, ErrorMessage = Config.ServerError };
            if (response != null && response.status)
            {
                TokenManager.SaveTokens(
                    response.data?.access_token,
                    response.data?.refresh_token ?? string.Empty,
                    response.data?.expires_in ?? 0);
                return new AuthResult { IsSuccess = true, Data = response };
            }

            // Username or password incorrect
            string errorMsg = response != null ? response.message : Config.LoginFailed;
            return new AuthResult { IsSuccess = false, ErrorMessage = errorMsg };
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthService] Login failed: {ex.Message}");
            return new AuthResult { IsSuccess = false, ErrorMessage = Config.ServerError };
        }
    }

    public async UniTask<bool> LogoutAsync()
    {
        bool isSuccess = false;
        try
        {
            isSuccess = await m_NetworkService.SendAuthPostStatusAsync(ApiConfig.API_POST_LOGOUT_URL, new object());
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthService] Logout failed: {ex.Message}");
        }
        finally
        {
            TokenManager.ClearTokens();
        }

        return isSuccess;
    }

}

