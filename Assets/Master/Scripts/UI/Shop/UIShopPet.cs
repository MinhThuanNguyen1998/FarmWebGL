using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShopPet : UIShopBase<UIPetItem>
{
    [Header("Dynamic UI Setup")]
    [SerializeField] private UIPetItem m_PrefabElement; 
    [SerializeField] private Transform m_ContentContainer; 

    protected override void UpdateUI(UserData data)
    {
       
        if (data == null || data.petStorage == null || data.petStorage.animalGroups == null) return;

        foreach (Transform child in m_ContentContainer)
        {
            Destroy(child.gameObject); 
        }
        m_UIItem.Clear();

        foreach (var groupData in data.petStorage.animalGroups)
        {
            if (groupData == null) continue;

            UIPetItem newElement = Instantiate(m_PrefabElement, m_ContentContainer);
            newElement.InitAndSetup(groupData); 
            m_UIItem.Add(newElement); 
        }
    }
}
