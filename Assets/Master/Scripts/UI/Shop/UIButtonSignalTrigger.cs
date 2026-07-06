using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public abstract class UIButtonSignalTrigger : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button m_TriggerButton;

    [Inject] protected readonly SignalBus m_SignalBus;

    protected virtual void Awake()
    {
        if (m_TriggerButton != null)
        {
            m_TriggerButton.onClick.AddListener(HandleButtonClick);
        }
        else
        {
            Debug.LogError($"[{System.Guid.NewGuid()}] Trigger Button is not assigned on {gameObject.name}!");
        }
    }

    protected virtual void OnDestroy()
    {
        if (m_TriggerButton != null)
        {
            m_TriggerButton.onClick.RemoveListener(HandleButtonClick);
        }
    }

    private void HandleButtonClick()
    {
        OnButtonClicked();
    }

    protected abstract void OnButtonClicked();
}
