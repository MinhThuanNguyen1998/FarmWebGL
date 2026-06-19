using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject m_ShopPanel; 

    [Header("UI Buttons")]
    [SerializeField] private Button m_OpenShopButton;  
    [SerializeField] private Button m_CloseShopButton;

    private void Start()
    {
        if (m_ShopPanel != null) m_ShopPanel.SetActive(false);
        if (m_OpenShopButton != null) m_OpenShopButton.onClick.AddListener(OpenShop);
        if (m_CloseShopButton != null) m_CloseShopButton.onClick.AddListener(CloseShop);

    }

    public void OpenShop()
    {
        if (m_ShopPanel != null) m_ShopPanel.SetActive(true);
    }
    public void CloseShop()
    {
        if (m_ShopPanel != null) m_ShopPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (m_OpenShopButton != null) m_OpenShopButton.onClick.RemoveListener(OpenShop);
        if (m_CloseShopButton != null) m_CloseShopButton.onClick.RemoveListener(CloseShop);
    }
}
