using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class UIShopProduce : UIShopBase<UIProduceItem>
{
    [Header("Dynamic UI Setup")]
    [SerializeField] private UIProduceItem m_PrefabElement;
    [SerializeField] private Transform m_ContentContainer;
    protected override void UpdateUI(UserData data)
    {
        if (data == null || data.petStorage == null || data.petStorage.generalProduces == null) return;
  
        foreach (Transform child in m_ContentContainer)
        {
            Destroy(child.gameObject);
        }
        m_UIItem.Clear();

        foreach (var produceData in data.petStorage.generalProduces)
        {
            if (produceData == null) continue;

            UIProduceItem newElement = Instantiate(m_PrefabElement, m_ContentContainer);
            newElement.InitAndSetup(produceData);
            m_UIItem.Add(newElement);
        }
    }
}
