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
public struct LoginFailedSignal 
{
    public string ErrorMessage;
    public LoginFailedSignal(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}

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
// Inventory signals
public struct InventoryLoadedSignal
{
    public List<InventoryItem> Items;

    public InventoryLoadedSignal(List<InventoryItem> items)
    {
        Items = items;
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
