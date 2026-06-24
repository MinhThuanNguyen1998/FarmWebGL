using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawer : AnimalSpawerBase
{
    private readonly List<GameObject> m_SpawnedChickens = new List<GameObject>();


    protected override void UpdateAnimals(UserData data)
    {
        if (data?.petStorage?.animalGroups == null) return;

        DespawnAll();

        AnimalGroup chickenGroup = data.petStorage.animalGroups.Find(group => group.groupName == m_GroupName);

        if (chickenGroup?.animals == null) return;

        foreach (var animalData in chickenGroup.animals)
        {
            GameObject chicken = SpawnAnimal(animalData);
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
