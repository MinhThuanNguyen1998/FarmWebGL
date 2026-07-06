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

    [Inject] private readonly AuthService m_AuthService;
    [Inject] private readonly SceneLoader m_SceneLoader;
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
        ProcessLogoutAsync().Forget();
    }
    private void OnNoButtonClicked()
    {
        if (m_IsProcessing) return;
        Close();
    }
    private async UniTaskVoid ProcessLogoutAsync()
    {
        m_IsProcessing = true;
        SetButtonsInteractable(false);

        await m_AuthService.LogoutAsync();

        await HideAsync();

        await m_SceneLoader.LoadSceneWithLoadingBar(Config.Login_Scene);

        m_IsProcessing = false;
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
