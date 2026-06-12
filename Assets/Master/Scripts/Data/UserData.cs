using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData 
{
    public long money;
    public FarmData farm;
    public PetData pet;
    public InventoryData inventory;

}
[Serializable]
public class FarmData
{
    public int chickenCount;
    public int eggCount;
}

[Serializable]
public class PetData
{
    public int catCount;
    public int kittenCount;
}

[Serializable]
public class InventoryData
{
    public List<ItemData> foods;
}

[Serializable]
public class ItemData
{
    public string itemId;
    public int count;
}
