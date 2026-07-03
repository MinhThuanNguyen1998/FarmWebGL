using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class UIAnimalDaysLeft : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshPro m_DaysLeftText;

    private BillboardManager m_BillboardManager;

    [Inject]
    public void Construct(BillboardManager billboardManager)
    {
        m_BillboardManager = billboardManager;
    }
    private void OnEnable()
    {
        // Register the text transform to the billboard system when spawned/enabled
        if (m_BillboardManager != null && m_DaysLeftText != null)
        {
            m_BillboardManager.Register(m_DaysLeftText.transform);
        }
    }

    private void OnDisable()
    {
        // Unregister to prevent NullReferenceException when returned to the pool/disabled
        if (m_BillboardManager != null && m_DaysLeftText != null)
        {
            m_BillboardManager.Unregister(m_DaysLeftText.transform);
        }
    }
    public void UpdateUIDaysLeft(FarmAnimal farmAnimal)
    {
        if (m_DaysLeftText != null)
        {
            m_DaysLeftText.text = farmAnimal.remaining_days.ToString();
        }
        else
        {
            Debug.LogWarning($"[{name}] m_DaysLeftText is not assigned in the Inspector!");
        }

    }
}
