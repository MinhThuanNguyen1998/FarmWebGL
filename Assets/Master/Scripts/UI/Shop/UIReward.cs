using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIReward : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button m_ClaimRewardButton;

    [Inject] private readonly UserDataService m_UserDataService;

    private void Awake()
    {
        if (m_ClaimRewardButton != null)
        {
            m_ClaimRewardButton.onClick.AddListener(HandleClaimRewardClick);
        }
        else
        {
            Debug.LogError("[UIReward] Claim Reward Button is not assigned in the Inspector!");
        }
    }

    private void OnDestroy()
    {
        if (m_ClaimRewardButton != null)
        {
            m_ClaimRewardButton.onClick.RemoveListener(HandleClaimRewardClick);
        }
    }

    private async void HandleClaimRewardClick()
    {
        Debug.Log("[UIReward] Player clicked Claim Reward button.");
        bool isSuccess = await m_UserDataService.ClaimRewardAsync();

        if (isSuccess)
        {
            
        }
        else
        {
           
        }
    }

    private void SetButtonInteractable(bool isInteractable)
    {
        if (m_ClaimRewardButton != null)
        {
            m_ClaimRewardButton.interactable = isInteractable;
        }
    }
}
