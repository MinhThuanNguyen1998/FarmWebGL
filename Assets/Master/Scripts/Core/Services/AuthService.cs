using System;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
public class AuthService 
{
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

    [System.Serializable]
    public class UserData
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class LoginDataContent
    {
        public string access_token;
        public string token_type;
        public int expires_in;
        public UserData user;
    }

    [System.Serializable]
    public class TokenResponse
    {
        public bool status; // JSON true/false
        public string message;
        public LoginDataContent data;
    }

    public async UniTask<AuthResult> LoginAsync(string username, string password)
    {
        try
        {
            // 1. Prepare json data
            LoginRequest loginRequest = new LoginRequest { username = username, password = password };
            string jsonBody = JsonUtility.ToJson(loginRequest);

            // 2. Create UnityWebRequest
            using (UnityWebRequest request = new UnityWebRequest(ApiConfig.API_AUTH_URL, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                // 3. Send request and await response
                await request.SendWebRequest().ToUniTask();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = request.downloadHandler.text;

                    // Parse json from server to TokenResponse 
                    TokenResponse tokens = JsonUtility.FromJson<TokenResponse>(jsonResponse);

                    
                    string accessToken = tokens.data?.access_token;

                   
                    string refreshToken = "";

                    // Save tokens to PlayerPrefs
                    TokenManager.SaveTokens(accessToken, refreshToken);

                    return new AuthResult { IsSuccess = true, Data = tokens };
                }
                else
                {
                    string errorFromFields = Config.LoginFailed;
                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        errorFromFields = request.downloadHandler.text;
                    }
                    return new AuthResult { IsSuccess = false, ErrorMessage = errorFromFields };
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Login failed: {ex.Message}");
            return new AuthResult { IsSuccess = false, ErrorMessage = Config.ServerError };
        }
    }
}
