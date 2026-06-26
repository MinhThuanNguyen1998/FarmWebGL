using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Login signals
public struct LoginRequestSignal
{
    public string UserName;
    public string Password;

    public LoginRequestSignal(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }
}
public struct LoginSuccessSignal { }
public struct LoginFailedSignal { }

public struct LoginDataErrorSignal { }


// User data signals
public struct UserDataLoadedSignal
{
    public UserData Data;

    public UserDataLoadedSignal(UserData data)
    {
        Data = data;
    }
}

// Animals
public struct AddAnimalSignal
{
    public string GroupName;

    public AddAnimalSignal(string groupName)
    {
        GroupName = groupName;
    }
}
public struct AddAnimalResultSignal
{
    public string GroupName;
    public bool IsSuccess;

    public AddAnimalResultSignal(string groupName, bool isSuccess)
    {
        GroupName = groupName;
        IsSuccess = isSuccess;
    }
}
