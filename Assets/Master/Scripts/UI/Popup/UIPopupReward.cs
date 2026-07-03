using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupReward : PopupBase
{
    [Header("Reward UI Elements")]
    [SerializeField] private TextMeshProUGUI m_TextTitle;
    [SerializeField] private Button m_BtnClose;

    private RewardPopupData m_Data;

    private void Awake()
    {
        if (m_BtnClose != null)
        {
            m_BtnClose.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    public override void Setup(object data)
    {
        m_Data = data as RewardPopupData;
        if (m_Data == null)
        {
            Debug.LogWarning("[RewardPopup] data is null or wrong type.");
            return;
        }
        if (m_TextTitle != null) m_TextTitle.text = m_Data.content;

        if (m_BtnClose != null)
        {
            m_BtnClose.interactable = true;
        }
    }

    private void OnCloseButtonClicked()
    {
        if (m_BtnClose == null) return;
        m_BtnClose.interactable = false;

        Close();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (m_BtnClose != null)
        {
            m_BtnClose.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }

}
