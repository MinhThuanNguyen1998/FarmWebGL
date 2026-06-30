using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ErrorTranslator 
{
    /// <summary>
    /// Translates system or API error messages into Vietnamese.
    /// </summary>
    /// <param name="originalError">The original error message from the server or Config.</param>
    /// <returns>The corresponding Vietnamese error message.</returns>
  
    public static string GetVietnameseErrorMessage(string originalError)
    {
        if (string.IsNullOrEmpty(originalError))
            return Config.ServerError; // Default to server error if empty

        // Convert the error message to lowercase to handle case-insensitive checks
        string lowerError = originalError.ToLower();

        // Check if the error message contains login-related keywords (username, password, incorrect, unauthorized)
        if (originalError == Config.LoginFailed ||
            lowerError.Contains("username") ||
            lowerError.Contains("password") ||
            lowerError.Contains("incorrect") ||
            lowerError.Contains("unauthorized"))
        {
            return Config.LoginFailed; // Returns "Sai Thông tin đăng nhập "
        }

        // Everything else falls back to ServerError
        return Config.ServerError; // Returns "Lỗi kết nối đến server"
    }

    
}

