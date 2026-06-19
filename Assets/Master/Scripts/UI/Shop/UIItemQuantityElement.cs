using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIItemQuantityElement : MonoBehaviour
{
    [SerializeField] private string m_ItemId; 
    [SerializeField] private TextMeshProUGUI m_CountText;

    public string ItemId => m_ItemId;

    public void UpdateCount(int count)
    {
        if (m_CountText != null)
        {
            m_CountText.text = count.ToString();
        }
    }
}
