using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIPopupLogOut : PopupBase
{
    [Header("Logout UI Elements")]
    [SerializeField] private TextMeshProUGUI m_TextContent;
    [SerializeField] private Button m_BtnYes;
    [SerializeField] private Button m_BtnNo;

    private LogoutPopupData m_Data;
    private bool m_IsProcessing;
    [Inject] private readonly SignalBus m_SignalBus;
    private void Awake()
    {
        if (m_BtnYes != null) m_BtnYes.onClick.AddListener(OnYesButtonClicked);
        if (m_BtnNo != null) m_BtnNo.onClick.AddListener(OnNoButtonClicked);
    }
    public override void Setup(object data)
    {
        m_Data = data as LogoutPopupData;
        m_IsProcessing = false;
        SetButtonsInteractable(true);

        if (m_TextContent != null)
        {
            m_TextContent.text = (m_Data != null && !string.IsNullOrEmpty(m_Data.content))
                ? m_Data.content
                : Config.Logout;
        }
    }
    private void OnYesButtonClicked()
    {
        if (m_IsProcessing) return;
        m_IsProcessing = true;
        SetButtonsInteractable(false);

        m_SignalBus.Fire<LogoutConfirmedSignal>(); // fire signal to LogOutManager
        Close();
    }
    private void OnNoButtonClicked()
    {
        if (m_IsProcessing) return;
        Close();
    }
    

    private void SetButtonsInteractable(bool interactable)
    {
        if (m_BtnYes != null) m_BtnYes.interactable = interactable;
        if (m_BtnNo != null) m_BtnNo.interactable = interactable;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (m_BtnYes != null) m_BtnYes.onClick.RemoveListener(OnYesButtonClicked);
        if (m_BtnNo != null) m_BtnNo.onClick.RemoveListener(OnNoButtonClicked);
    }
}
