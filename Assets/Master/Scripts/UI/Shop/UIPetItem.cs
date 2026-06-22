using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPetItem : MonoBehaviour
{
    [SerializeField] private Image m_AvatarImage; 
    [SerializeField] private TextMeshProUGUI m_NameText; 
    [SerializeField] private TextMeshProUGUI m_CountText; 
    [SerializeField] private Button m_ActionButton; 

    public string ItemName { get; private set; } 

   
    public void InitAndSetup(AnimalGroup groupData)
    {
        if (groupData == null)
        {
            m_NameText?.SetText(string.Empty); 
            m_CountText?.SetText("0"); 
            return;
        }

        ItemName = groupData.groupName;
        m_NameText?.SetText(ItemName); 

        
        int totalCount = groupData.animals != null ? groupData.animals.Count : 0;
        m_CountText?.SetText(totalCount.ToString());

       
        if (m_AvatarImage != null && !string.IsNullOrEmpty(ItemName))
        {
            Sprite loadedSprite = Resources.Load<Sprite>($"Avatar/{ItemName}"); 
            if (loadedSprite != null) m_AvatarImage.sprite = loadedSprite;
        }
        m_ActionButton?.onClick.RemoveAllListeners();
        m_ActionButton?.onClick.AddListener(() => OnClickItem(groupData));


    }
    private void OnClickItem(AnimalGroup groupData)
    {
       
    }
}
