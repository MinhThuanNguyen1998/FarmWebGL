using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
public class AuthService 
{

    [Inject] private readonly NetworkService m_NetworkService;

    [Serializable]
    private class LoginRequest
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

    public async UniTask<AuthResult> LoginAsync(string username, string password)
    {
        try
        {
            var requestBody = new LoginRequest { username = username, password = password };
            var (networkSuccess, response) = await m_NetworkService.SendPublicPostRequestAsync<LoginRequest, TokenResponse>(
                ApiConfig.API_AUTH_URL, requestBody);

            if (!networkSuccess)
                return new AuthResult { IsSuccess = false, ErrorMessage = Config.LoginFailed };

            if (response != null && response.status)
            {
                TokenManager.SaveTokens(response.data?.access_token, "");
                return new AuthResult { IsSuccess = true, Data = response };
            }

            string errorMsg = response != null ? response.message : Config.LoginFailed;
            return new AuthResult { IsSuccess = false, ErrorMessage = errorMsg };
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthService] Login failed: {ex.Message}");
            return new AuthResult { IsSuccess = false, ErrorMessage = Config.ServerError };
        }
    }
}
