using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ApiUserDataResponse
{
    public bool success;
    public string message;
    public ApiUserDataContent data;
}

[Serializable]
public class ApiUserDataContent
{
    public UserInfo user;
    public List<FarmAnimal> farm;
}

[Serializable]
public class UserInfo
{
    public int id;
    public int parent_id;
    public string name;
    public string username;
    public string email;
    public string ref_code;
    public string wallet_address;
    public string amount; 
}

[Serializable]
public class FarmAnimal
{
    public int id;
    public int user_id;
    public string animal_name;  
    public string status;       
    public int total_days;
    public int remaining_days;
    public string start_time;
    public string finish_time;
}

[Serializable]
public class UserData
{
    public UserInfo userInfo;
    public List<FarmAnimal> farm;
    public long money => ParseMoney(userInfo?.amount);

    private static long ParseMoney(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return 0;
        if (double.TryParse(raw,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out double val))
        {
            return (long)val;
        }
        return 0;
    }

    public double GetBalance()
    {
        return double.TryParse(userInfo?.amount,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out double val) ? val : 0;
    }
    public Dictionary<string, List<FarmAnimal>> GetAnimalGroups()
    {
        var groups = new Dictionary<string, List<FarmAnimal>>();
        if (farm == null) return groups;
        foreach (var animal in farm)
        {
            if (!groups.ContainsKey(animal.animal_name))
                groups[animal.animal_name] = new List<FarmAnimal>();
            groups[animal.animal_name].Add(animal);
        }
        return groups;
    }
}