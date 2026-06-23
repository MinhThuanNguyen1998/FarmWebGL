using System.Collections.Generic;
using UnityEngine;

public class AnimalPool : IAnimalPool
{
    private readonly GameObject m_Prefab;
    private readonly Transform m_PoolParent;
    private readonly int m_MaxSize;

    private readonly Stack<GameObject> m_Inactive = new Stack<GameObject>();
    private readonly HashSet<GameObject> m_Active = new HashSet<GameObject>();

    public AnimalPool(GameObject prefab, Transform poolParent, int initialSize, int maxSize)
    {
        m_Prefab = prefab;
        m_PoolParent = poolParent;
        m_MaxSize = maxSize; 

        for (int i = 0; i < initialSize; i++)
            m_Inactive.Push(CreateNew());
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject obj;

        if (m_Inactive.Count > 0)
            obj = m_Inactive.Pop();
        else if (m_Active.Count < m_MaxSize)
            obj = CreateNew();
        else
        {
            Debug.LogWarning($"[AnimalPool] Reached max limit ({m_MaxSize}).");
            return null;
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        m_Active.Add(obj);
        return obj;
    }

    public void Despawn(GameObject animal)
    {
        if (!m_Active.Remove(animal))
        {
            Debug.LogWarning("[AnimalPool] Object is not part of this pool.");
            return;
        }

        animal.SetActive(false);
        animal.transform.SetParent(m_PoolParent);
        m_Inactive.Push(animal);
    }

    public void Clear()
    {
        foreach (var obj in m_Active) Object.Destroy(obj);
        foreach (var obj in m_Inactive) Object.Destroy(obj);
        m_Active.Clear();
        m_Inactive.Clear();
    }

    private GameObject CreateNew()
    {
        var obj = Object.Instantiate(m_Prefab, m_PoolParent);
        obj.SetActive(false);
        return obj;
    }
}
