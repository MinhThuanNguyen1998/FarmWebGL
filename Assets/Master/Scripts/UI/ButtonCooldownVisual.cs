using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCooldownVisual : MonoBehaviour
{
    
    private float m_CooldownTime = 1f;

    private Button m_Button;
    private WaitForSeconds m_WaitTime;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
        m_WaitTime = new WaitForSeconds(m_CooldownTime);

        
        m_Button.onClick.AddListener(TriggerCooldown);
    }

    private void OnDestroy()
    {
        if (m_Button != null)
        {
            m_Button.onClick.RemoveListener(TriggerCooldown);
        }
    }

    private void TriggerCooldown()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        m_Button.interactable = false;

        yield return m_WaitTime;

        m_Button.interactable = true;
    }



}
