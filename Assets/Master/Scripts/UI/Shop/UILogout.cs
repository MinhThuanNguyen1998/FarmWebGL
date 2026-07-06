using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILogout : UIButtonSignalTrigger
{
    protected override void OnButtonClicked()
    {
        Debug.Log("[UILogout] Player clicked Logout button.");
        m_SignalBus.Fire<LogoutRequestSignal>();
    }
}
