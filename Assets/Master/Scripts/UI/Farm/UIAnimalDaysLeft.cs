using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;    

public class UIAnimalDaysLeft : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshPro m_DaysLeftText;

    public void UpdateUIDaysLeft(AnimalData animalData)
    {
        if (m_DaysLeftText != null)
        {
            m_DaysLeftText.text = Config.Days_Left_First+ animalData.daysLeft.ToString() + Config.Days_Left_Last;
        }
        else
        {
            Debug.LogWarning($"[{name}] m_DaysLeftText is not assigned in the Inspector!");
        }

    }
}
