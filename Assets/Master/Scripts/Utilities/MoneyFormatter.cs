using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoneyFormatter
{
    public static string ToShortString(long money)
    {
        // 1e12 = 1,000,000,000,000 
        if (money >= 1e12) return (money / 1e12f).ToString("F2") + "T";

        // 1e9  = 1,000,000,000 
        if (money >= 1e9) return (money / 1e9f).ToString("F2") + "B";

        // 1e6  = 1,000,000 
        if (money >= 1e6) return (money / 1e6f).ToString("F2") + "M";

        // 1e3  = 1,000 
        if (money >= 1e3) return (money / 1e3f).ToString("F1") + "K";

        return money.ToString("N0");
    }
    public static string ParseAndFormat(string rawMoney)
    {
        if (string.IsNullOrEmpty(rawMoney)) return "0";

        if (double.TryParse(rawMoney,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out double val))
        {
            return ToShortString((long)val);
        }

        Debug.LogError($"[MoneyFormatter] Can not parse string: {rawMoney}");
        return "0";
    }
}
