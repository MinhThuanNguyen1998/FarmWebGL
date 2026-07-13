using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenManager
{
    private const string ACCESS_TOKEN_KEY = "Access_Token";
    private const string REFRESH_TOKEN_KEY = "Refresh_Token";
    private const string EXPIRY_TIME_KEY = "Access_Token_Expiry"; // UTC ticks, stored as string

    public static void SaveTokens(string accessToken, string refreshToken, int expiresIn = 0)
    {
        PlayerPrefs.SetString(ACCESS_TOKEN_KEY, accessToken ?? string.Empty);
        PlayerPrefs.SetString(REFRESH_TOKEN_KEY, refreshToken ?? string.Empty);

        if (expiresIn > 0)
        {
            DateTime expiryTimeUtc = DateTime.UtcNow.AddSeconds(expiresIn);
            PlayerPrefs.SetString(EXPIRY_TIME_KEY, expiryTimeUtc.Ticks.ToString());
        }
        else
        {
            PlayerPrefs.DeleteKey(EXPIRY_TIME_KEY);
        }

        PlayerPrefs.Save();
    }

    public static string GetAccessToken()
    {
        return PlayerPrefs.GetString(ACCESS_TOKEN_KEY, string.Empty);
    }

    public static bool HasToken()
    {
        return !string.IsNullOrEmpty(GetAccessToken());
    }

    public static void ClearTokens()
    {
        PlayerPrefs.DeleteKey(ACCESS_TOKEN_KEY);
        PlayerPrefs.DeleteKey(REFRESH_TOKEN_KEY);
        PlayerPrefs.DeleteKey(EXPIRY_TIME_KEY);
        PlayerPrefs.Save();
    }
}
