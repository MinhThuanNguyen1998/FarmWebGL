using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    public long money;
    public PetStorage petStorage;
}

[Serializable]
public class PetStorage
{
    public List<ProduceData> generalProduces = new List<ProduceData>();
    public List<AnimalGroup> animalGroups = new List<AnimalGroup>();
}

[Serializable]
public class ProduceData 
{
    public string name; 
    public int count;   
}

[Serializable]
public class AnimalGroup
{
    public string groupName;
    public List<AnimalData> animals = new List<AnimalData>(); 
}
[Serializable]
public class AnimalData
{
    public string id;
    public int daysLeft;
    
    public AnimalData(string id, int daysLeft)
    {
        this.id = id;
        this.daysLeft = daysLeft;
    }
}
