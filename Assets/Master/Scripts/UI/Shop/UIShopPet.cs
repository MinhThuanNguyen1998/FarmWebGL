using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class UIShopPet : UIShopBase<UIPetItem, UIPetItem.Pool>
{
    [SerializeField] private Transform m_ContentContainer;

    protected override void UpdateUI(List<InventoryItem> items)
    {
        DespawnAll();

        if (items == null || items.Count == 0) return;

        foreach (var inventoryItem in items)
        {
            UIPetItem item = m_Pool.Spawn();
            item.transform.SetParent(m_ContentContainer, false);
            item.InitAndSetup(inventoryItem);
            m_ActiveItems.Add(item);
        }
    }

    protected override void DespawnItem(UIPetItem item) => m_Pool.Despawn(item);
}
