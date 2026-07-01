using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPopupReward : PopupBase
{
    [SerializeField] private TextMeshProUGUI m_TextTitle;

    private RewardPopupData m_Data;
    public override void Setup(object data)
    {
        m_Data = data as RewardPopupData;
        if (m_Data == null)
        {
            Debug.LogWarning("[RewardPopup] data is null or wrong type.");
            return;
        }
        if (m_TextTitle != null) m_TextTitle.text = m_Data.content;
    }
    
}
