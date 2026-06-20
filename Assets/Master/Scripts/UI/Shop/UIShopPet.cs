using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class UIShopPet : UIShopBase
{
    [Header("Dynamic UI Setup")]
    [SerializeField] private UIItem m_PrefabElement;
    [SerializeField] private Transform m_ContentContainer;
    protected override void UpdateUI(UserData data)
    {
        if (data == null || data.pet == null || data.pet.items == null) return;

        foreach (Transform child in m_ContentContainer)
        {
            Destroy(child.gameObject);
        }
        m_UIItem.Clear(); 

        foreach (var itemData in data.pet.items)
        {
            if (itemData == null) continue;
            UIItem newElement = Instantiate(m_PrefabElement, m_ContentContainer);
            newElement.InitAndSetup(itemData);
            m_UIItem.Add(newElement);
        }

    }
}
