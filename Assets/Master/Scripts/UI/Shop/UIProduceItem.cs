//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using Zenject;
//public class UIProduceItem : MonoBehaviour
//{
//    [SerializeField] private Image m_AvatarImage;
//    [SerializeField] private TextMeshProUGUI m_NameText;
//    [SerializeField] private TextMeshProUGUI m_CountText;

//    public string ItemName { get; private set; }

//    // NOTE: API hiện chưa trả về produce data trong response.
//    // ProduceData vẫn giữ nguyên struct, chờ API bổ sung field này.
//    public void InitAndSetup( produceData)
//    {
//        if (produceData == null)
//        {
//            m_NameText?.SetText(string.Empty);
//            m_CountText?.SetText("0");
//            return;
//        }

//        ItemName = produceData.name;
//        m_NameText?.SetText(ItemName);
//        m_CountText?.SetText(produceData.count.ToString());

//        if (m_AvatarImage != null && !string.IsNullOrEmpty(ItemName))
//        {
//            Sprite loadedSprite = Resources.Load<Sprite>($"Avatar/{ItemName}");
//            if (loadedSprite != null) m_AvatarImage.sprite = loadedSprite;
//        }
//    }

//    public void OnSpawned()
//    {
//        gameObject.SetActive(true);
//    }

//    public void OnDespawned()
//    {
//        gameObject.SetActive(false);
//    }

//    public class Pool : MonoMemoryPool<UIProduceItem>
//    {
//        protected override void OnSpawned(UIProduceItem item) => item.OnSpawned();
//        protected override void OnDespawned(UIProduceItem item) => item.OnDespawned();
//    }
//}
