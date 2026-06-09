using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
public class AuthService 
{
    private const string API_LOGIN_URL = "https://dummyjson.com/auth/login";
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
    public class TokenResponse
    {
        public string accessToken;
        public string refreshToken;
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        try
        {
            // 1. Prepare json data
            LoginRequest loginRequest = new LoginRequest { username = username, password = password };
            string jsonBody = JsonUtility.ToJson(loginRequest);

            // 2. Create UnityWebRequest
            using (UnityWebRequest request = new UnityWebRequest(API_LOGIN_URL, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody); // convert json string to byte array
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);// set download handler to receive response
                request.downloadHandler = new DownloadHandlerBuffer(); // set header for json content type
                request.SetRequestHeader("Content-Type", "application/json");

                // 3. Send request and await response
                var operation = request.SendWebRequest();
                var tcs = new TaskCompletionSource<bool>();
                operation.completed += _ => tcs.SetResult(true);
                // Await the completion of the request
                await tcs.Task;
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = request.downloadHandler.text;

                    // Parse json from server to TokenResponse object
                    TokenResponse tokens = JsonUtility.FromJson<TokenResponse>(jsonResponse);

                    // Save tokens to PlayerPrefs
                    TokenManager.SaveTokens(tokens.accessToken, tokens.refreshToken);

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
