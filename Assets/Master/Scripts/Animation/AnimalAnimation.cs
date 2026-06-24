using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private string m_VerticalID = "Vert";
    [SerializeField] private float m_AnimBlendSpeed = 4.5f;
    [SerializeField] private Animator m_Animator;

    private float m_CurrentAnimValue = 0f;
    private float m_TargetAnimValue = 0f;

    public void Tick()
    {
        if (m_Animator == null) return;
        m_CurrentAnimValue = Mathf.MoveTowards(m_CurrentAnimValue, m_TargetAnimValue, m_AnimBlendSpeed * Time.deltaTime);
        m_Animator.SetFloat(m_VerticalID, m_CurrentAnimValue);
    }

    public void SetAnimTarget(float targetValue)
    {
        m_TargetAnimValue = targetValue;
    }
}
