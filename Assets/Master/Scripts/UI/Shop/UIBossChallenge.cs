using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIBossChallenge : UIButtonSignalTrigger
{
    protected override void OnButtonClicked()
    {
        Debug.Log("[UIBossChallenge] Player clicked Boss Challenge button.");
        m_SignalBus.Fire<BossChallengeClickedSignal>();
    }
}