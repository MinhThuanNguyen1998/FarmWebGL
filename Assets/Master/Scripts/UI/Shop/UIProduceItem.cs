using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProduceItem : MonoBehaviour
{
    [SerializeField] private Image m_AvatarImage;
    [SerializeField] private TextMeshProUGUI m_NameText;
    [SerializeField] private TextMeshProUGUI m_CountText;
    public string ItemName { get; private set; }

    public void InitAndSetup(ProduceData produceData)
    {
        if (produceData == null)
        {
            m_NameText?.SetText(string.Empty);
            m_CountText?.SetText("0");
            return;
        }

        ItemName = produceData.name;
        m_NameText?.SetText(ItemName);
        m_CountText?.SetText(produceData.count.ToString());

        if (m_AvatarImage != null && !string.IsNullOrEmpty(ItemName))
        {
            Sprite loadedSprite = Resources.Load<Sprite>($"Avatar/{ItemName}");
            if (loadedSprite != null) m_AvatarImage.sprite = loadedSprite;
        }
    }
}
