using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenManager
{
    private const string ACCESS_TOKEN_KEY = "Access_Token";
    private const string REFRESH_TOKEN_KEY = "Refresh_Token";
    public static void SaveTokens(string accessToken, string refreshToken)
    {
        PlayerPrefs.SetString(ACCESS_TOKEN_KEY, accessToken);
        PlayerPrefs.SetString(REFRESH_TOKEN_KEY, refreshToken);
        PlayerPrefs.Save();
    }
    public static string GetAccessToken()
    {
        return PlayerPrefs.GetString(ACCESS_TOKEN_KEY, string.Empty);
    }
    public static string GetRefreshToken()
    {
        return PlayerPrefs.GetString(REFRESH_TOKEN_KEY, string.Empty);
    }
    public static bool HasToken()
    {
        return !string.IsNullOrEmpty(GetAccessToken());
    }
    public static void ClearTokens()
    {
        PlayerPrefs.DeleteKey(ACCESS_TOKEN_KEY);
        PlayerPrefs.DeleteKey(REFRESH_TOKEN_KEY);
        PlayerPrefs.Save();
    }
}
