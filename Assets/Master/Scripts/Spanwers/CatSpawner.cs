using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CatSpawner : AnimalSpawerBase
{
    private readonly List<GameObject> m_SpawnedCats = new List<GameObject>();

    protected override void UpdateAnimals(UserData data)
    {
        if (data?.farm == null) return;

        DespawnAll();

        foreach (var farmAnimal in data.farm)
        {
            if (farmAnimal.animal_name != m_GroupName) continue;

            GameObject cat = SpawnAnimal(farmAnimal);
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
