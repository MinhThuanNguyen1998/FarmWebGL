using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIReward : UIButtonSignalTrigger
{
    protected override async void OnButtonClicked()
    {
        Debug.Log("[UIReward] Player clicked Claim Reward button.");
        m_SignalBus.Fire<RewardRequestSignal>();
    }
}
