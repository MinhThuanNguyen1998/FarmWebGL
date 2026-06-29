using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawer : AnimalSpawerBase
{
    private readonly List<GameObject> m_SpawnedChickens = new List<GameObject>();


    protected override void UpdateAnimals(UserData data)
    {
        if (data?.farm == null) return;

        DespawnAll();

        foreach (var farmAnimal in data.farm)
        {
            if (farmAnimal.animal_name != m_GroupName) continue;

            GameObject chicken = SpawnAnimal(farmAnimal);
            if (chicken != null)
                m_SpawnedChickens.Add(chicken);
        }
    }

    private void DespawnAll()
    {
        foreach (var chicken in m_SpawnedChickens)
            DespawnAnimal(chicken);

        m_SpawnedChickens.Clear();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        DespawnAll();
    }

}
