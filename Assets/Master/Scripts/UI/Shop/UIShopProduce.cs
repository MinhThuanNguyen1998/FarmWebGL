//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using TMPro;
//using UnityEngine;
//using Zenject;

//public class UIShopProduce : UIShopBase<UIProduceItem, UIProduceItem.Pool>
//{
//    [SerializeField] private Transform m_ContentContainer;

//    protected override void UpdateUI(UserData data)
//    {
//        DespawnAll();

        
//        if (data?.produces == null) return;

//        foreach (var produceData in data.produces)
//        {
//            if (produceData == null) continue;

//            UIProduceItem item = m_Pool.Spawn();
//            item.transform.SetParent(m_ContentContainer, false);
//            item.InitAndSetup(produceData);
//            m_ActiveItems.Add(item);
//        }
//    }

//    protected override void DespawnItem(UIProduceItem item) => m_Pool.Despawn(item);
//}
