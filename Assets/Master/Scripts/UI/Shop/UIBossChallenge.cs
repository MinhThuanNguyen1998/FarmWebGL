using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIBossChallenge : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button m_ChallengeButton;

    [Inject] private readonly SignalBus m_SignalBus;

    private void Awake()
    {
        if (m_ChallengeButton != null)
        {
            m_ChallengeButton.onClick.AddListener(HandleChallengeClick);
        }
        else
        {
            Debug.LogError("[UIBossChallenge] Challenge Button is not assigned in the Inspector!");
        }
    }

    private void OnDestroy()
    {
        if (m_ChallengeButton != null)
        {
            m_ChallengeButton.onClick.RemoveListener(HandleChallengeClick);
        }
    }

    private void HandleChallengeClick()
    {
        Debug.Log("[UIBossChallenge] Player clicked Boss Challenge button.");
        m_SignalBus.Fire<BossChallengeClickedSignal>();
    }
}
