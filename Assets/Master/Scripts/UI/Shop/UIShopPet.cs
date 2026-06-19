using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class UIShopPet : UIShopBase
{
    protected override void UpdateUI(UserData data)
    {
        if (data == null) return;

        m_CachedItemCounts.Clear();
        AddItemDataToDict(data.pet?.items, m_CachedItemCounts);
        RefreshUIElements();
    }
}
