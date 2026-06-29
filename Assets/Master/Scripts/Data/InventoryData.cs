using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ApiInventoryResponse
{
    public bool status;
    public ApiInventoryContent data;
}

[Serializable]
public class ApiInventoryContent
{
    public List<InventoryItem> items;
}

[Serializable]
public class InventoryItem
{
    public string name;
    public string avatar;
    public int quantity;
}
public class InventoryData
{
    public List<InventoryItem> Items { get; set; }
}