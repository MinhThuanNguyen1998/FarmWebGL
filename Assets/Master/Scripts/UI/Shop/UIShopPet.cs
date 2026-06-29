using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class UIShopPet : UIShopBase<UIPetItem, UIPetItem.Pool>
{
   
    [SerializeField] private Transform m_ContentContainer; 

    protected override void UpdateUI(UserData data)
    {

        DespawnAll();

        if (data?.farm == null) return;


        var groups = new Dictionary<string, List<FarmAnimal>>();
        foreach (var farmAnimal in data.farm)
        {
            if (!groups.ContainsKey(farmAnimal.animal_name))
                groups[farmAnimal.animal_name] = new List<FarmAnimal>();
            groups[farmAnimal.animal_name].Add(farmAnimal);
        }

        foreach (var kvp in groups)
        {
            UIPetItem item = m_Pool.Spawn();
            item.transform.SetParent(m_ContentContainer, false);
            item.InitAndSetup(kvp.Key, kvp.Value);
            m_ActiveItems.Add(item);
        }
    }
    protected override void DespawnItem(UIPetItem item) => m_Pool.Despawn(item);
}
