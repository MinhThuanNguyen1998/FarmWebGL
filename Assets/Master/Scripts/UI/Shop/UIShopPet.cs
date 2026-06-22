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

        if (data?.petStorage?.animalGroups == null) return;

        foreach (var groupData in data.petStorage.animalGroups)
        {
            if (groupData == null) continue;

            UIPetItem item = m_Pool.Spawn();
            item.transform.SetParent(m_ContentContainer, false);
            item.InitAndSetup(groupData);
            m_ActiveItems.Add(item);
        }
    }
    protected override void DespawnItem(UIPetItem item) => m_Pool.Despawn(item);
}
