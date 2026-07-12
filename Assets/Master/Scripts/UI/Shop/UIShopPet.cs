using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
public class UIShopPet : UIShopBase<UIPetItem, UIPetItem.Pool>
{
    [SerializeField] private Transform m_ContentContainer;

    protected override void UpdateUI(List<InventoryItem> items)
    {
        DespawnAll();

        if (items == null || items.Count == 0) return;

        var filteredAndSorted = items
        .Where(x => string.Equals(x.name, "chicken", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(x.name, "cat", StringComparison.OrdinalIgnoreCase))
        .OrderBy(x => x.name, StringComparer.Ordinal)
        .ToList();

        for (int i = 0; i < filteredAndSorted.Count; i++)
        {
            UIPetItem item = m_Pool.Spawn();
            item.transform.SetParent(m_ContentContainer, false);
            item.transform.SetSiblingIndex(i);
            item.InitAndSetup(filteredAndSorted[i]);
            m_ActiveItems.Add(item);
        }
    }

    protected override void DespawnItem(UIPetItem item) => m_Pool.Despawn(item);
}
