using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenManager
{
    private const string ACCESS_TOKEN_KEY = "Access_Token";
    private const string REFRESH_TOKEN_KEY = "Refresh_Token";
    private const string EXPIRY_TIME_KEY = "Access_Token_Expiry"; // UTC ticks, stored as string

    // Threshold used to decide when a token is "about to expire" and should be refreshed proactively.
    public const int DEFAULT_REFRESH_THRESHOLD_MINUTES = 5;

    /// <summary>
    /// Save access + refresh token. expiresIn is the lifetime of the access token in seconds
    /// (usually taken from the login/refresh API response's "expires_in" field).
    /// </summary>
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

    // Kept for backward compatibility with older call sites that don't provide an expiry.
    public static void SaveTokens(string accessToken, string refreshToken)
    {
        SaveTokens(accessToken, refreshToken, 0);
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
    public static bool HasRefreshToken()
    {
        return !string.IsNullOrEmpty(GetRefreshToken());
    }

    public static DateTime GetAccessTokenExpiryUtc()
    {
        string raw = PlayerPrefs.GetString(EXPIRY_TIME_KEY, string.Empty);
        if (string.IsNullOrEmpty(raw) || !long.TryParse(raw, out long ticks))
            return DateTime.MinValue;

        return new DateTime(ticks, DateTimeKind.Utc);
    }

    public static bool IsTokenExpiringSoon(int thresholdMinutes = DEFAULT_REFRESH_THRESHOLD_MINUTES)
    {
        if (!HasToken())
            return false;

        DateTime expiryUtc = GetAccessTokenExpiryUtc();
        if (expiryUtc == DateTime.MinValue)
            return false;

        return DateTime.UtcNow >= expiryUtc.AddMinutes(-thresholdMinutes);
    }

    public static void ClearTokens()
    {
        PlayerPrefs.DeleteKey(ACCESS_TOKEN_KEY);
        PlayerPrefs.DeleteKey(REFRESH_TOKEN_KEY);
        PlayerPrefs.DeleteKey(EXPIRY_TIME_KEY);
        PlayerPrefs.Save();
    }
}
