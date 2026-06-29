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
        public bool status; // JSON true/false status from the server
        public string message;
        public LoginDataContent data;
    }
    public async UniTask<AuthResult> LoginAsync(string username, string password)
    {
        try
        {
            // 1. Prepare JSON data
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

                    // Parse JSON response from the server into TokenResponse object
                    TokenResponse tokens = JsonUtility.FromJson<TokenResponse>(jsonResponse);

                    // Verify if the login credentials are correct based on the server's response status
                    if (tokens != null && tokens.status)
                    {
                        string accessToken = tokens.data?.access_token;

                        // Save only the access token to PlayerPrefs via TokenManager
                        TokenManager.SaveTokens(accessToken, "");

                        return new AuthResult { IsSuccess = true, Data = tokens };
                    }
                    else
                    {
                        // Handle authentication failure (e.g., wrong username or password)
                        string errorMsg = tokens != null ? tokens.message : Config.LoginFailed;
                        return new AuthResult { IsSuccess = false, ErrorMessage = errorMsg };
                    }
                }
                else
                {
                    // Handle network errors or HTTP errors (e.g., 404, 500)
                    string errorFromFields = Config.LoginFailed;
                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        errorFromFields = request.downloadHandler.text;
                    }
                    return new AuthResult { IsSuccess = false, ErrorMessage = errorFromFields };
                }
            }
        }
        catch (Exception ex) // Handle unexpected internal system or parsing exceptions
        {
            Debug.LogError($"Login failed: {ex.Message}");
            return new AuthResult { IsSuccess = false, ErrorMessage = Config.ServerError };
        }
    }
}
