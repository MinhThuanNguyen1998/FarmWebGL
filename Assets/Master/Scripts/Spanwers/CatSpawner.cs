using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CatSpawner : AnimalSpawerBase
{
    private readonly List<GameObject> m_SpawnedCats = new List<GameObject>();
   
   
    protected override void UpdateAnimals(UserData data)
    {
        if (data?.petStorage?.animalGroups == null) return;

        DespawnAll();

        AnimalGroup catGroup = data.petStorage.animalGroups.Find(group => group.groupName == m_GroupName);

        if (catGroup?.animals == null) return;

        for (int i = 0; i < catGroup.animals.Count; i++)
        {
            GameObject cat = SpawnAnimal();
            if (cat != null)
                m_SpawnedCats.Add(cat);
        }
    }

    private void DespawnAll()
    {
        foreach (var cat in m_SpawnedCats)
            DespawnAnimal(cat);

        m_SpawnedCats.Clear();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        DespawnAll();
    }
}
