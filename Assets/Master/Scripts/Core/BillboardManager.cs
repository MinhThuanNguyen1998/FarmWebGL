using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BillboardManager : ILateTickable
{
    private readonly List<Transform> m_BillboardTransforms = new List<Transform>();
    private Transform m_CustomTarget;

    public void Register(Transform transform)
    {
        if (!m_BillboardTransforms.Contains(transform))
        {
            m_BillboardTransforms.Add(transform);
        }
    }
    public void Unregister(Transform transform)
    {
        if (m_BillboardTransforms.Contains(transform))
        {
            m_BillboardTransforms.Remove(transform);
        }
    }

    public void SetTarget(Transform target)
    {
        m_CustomTarget = target;
    }

    /// <summary>
    /// Centralized update loop managed by Zenject's tick system.
    /// Executes after all standard Update loops have finished.
    /// </summary>
    public void LateTick()
    {
        // If no target is assigned, skip rotation logic to prevent NullReferenceException
        if (m_CustomTarget == null) return;

        int count = m_BillboardTransforms.Count;
        for (int i = 0; i < count; i++)
        {
            if (m_BillboardTransforms[i] != null)
            {
                // Force the text to face the target transform directly
                m_BillboardTransforms[i].LookAt(m_CustomTarget);
            }
        }
    }
}
