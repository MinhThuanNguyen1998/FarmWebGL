using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabManager : MonoBehaviour
{
    [System.Serializable]
    public struct TabPage
    {
        public Button tabButton;
        public GameObject contentPanel;
    }

    [Header("Tab Settings")]
    [SerializeField] private List<TabPage> m_Tabs;
    [SerializeField] private int m_DefaultTabIndex = 0;
    private Color m_ActiveColor;
    private Color m_InactiveColor;
    private void Awake()
    {
        ColorUtility.TryParseHtmlString("#AD7B35", out m_ActiveColor);
        ColorUtility.TryParseHtmlString("#EED8B9", out m_InactiveColor);
    }
    private void Start()
    {

        for (int i = 0; i < m_Tabs.Count; i++)
        {
            int index = i;
            if (m_Tabs[i].tabButton != null)
            {
                m_Tabs[i].tabButton.onClick.AddListener(() => SelectTab(index));
            }
        }
        SelectTab(m_DefaultTabIndex);
    }

    public void SelectTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= m_Tabs.Count) return;

        for (int i = 0; i < m_Tabs.Count; i++)
        {
            if (m_Tabs[i].contentPanel != null)
            {
                m_Tabs[i].contentPanel.SetActive(i == tabIndex);
            }
            if (m_Tabs[i].tabButton != null)
            {

                Image buttonImage = m_Tabs[i].tabButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = (i == tabIndex) ? m_ActiveColor : m_InactiveColor;
                }
            }
        }
    }
    private void OnDestroy()
    {

        for (int i = 0; i < m_Tabs.Count; i++)
        {
            if (m_Tabs[i].tabButton != null)
            {
                m_Tabs[i].tabButton.onClick.RemoveAllListeners();
            }
        }
    }
}

