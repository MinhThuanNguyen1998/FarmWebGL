using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    [SerializeField] private Image m_AvatarImage;
    [SerializeField] private TextMeshProUGUI m_NameText;
    [SerializeField] private TextMeshProUGUI m_CountText;

    public string ItemName { get; private set; }

    public void InitAndSetup(ItemData itemData)
    {
        if (itemData == null)
        {
            m_NameText?.SetText(string.Empty);
            m_CountText?.SetText("0");
            return;
        }

        ItemName = itemData.name;
        m_NameText?.SetText(ItemName);
        m_CountText?.SetText(itemData.count.ToString());

        if (m_AvatarImage != null && !string.IsNullOrEmpty(itemData.avatar))
        {
            Sprite loadedSprite = Resources.Load<Sprite>($"Avatar/{itemData.avatar}");
            if (loadedSprite != null) m_AvatarImage.sprite = loadedSprite;
        }
    }
}
