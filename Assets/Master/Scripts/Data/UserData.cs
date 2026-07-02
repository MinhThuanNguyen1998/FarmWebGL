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
public class ApiLoadAnimalResponse
{
    public bool success;
    public string message;
    public List<FarmAnimal> data;
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
    public double money => ParseMoney(userInfo?.amount);

    private static double ParseMoney(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return 0;
        if (double.TryParse(raw,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out double val))
        {
            return val;
        }
        return 0;
    }


}