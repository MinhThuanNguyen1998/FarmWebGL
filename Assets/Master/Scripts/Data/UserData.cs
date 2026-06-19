using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData 
{
    public long money;
    public ItemContainer pet;
    
}



[Serializable]
public class ItemContainer
{
    public List<ItemData> items;
}

[Serializable]


public class ItemData
{
    public string itemId;
    public int count;
}
