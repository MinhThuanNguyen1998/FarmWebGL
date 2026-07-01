using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PopupManager
{
    private readonly Queue<(PopupBase prefab, object data)> m_PopupQueue = new();
    private readonly Dictionary<PopupBase, List<PopupBase>> m_Pool = new();
    private readonly DiContainer m_Container;
    private readonly Transform m_CanvasRoot;

    private PopupBase m_CurrentPopup;
    private bool m_IsShowing;

    public PopupManager(DiContainer container, Transform canvasRootPopup)
    {
        m_Container = container;
        m_CanvasRoot = canvasRootPopup;
    }

    public void ShowPopup(PopupBase prefab, object data = null)
    {
        if (prefab == null) return;
        m_PopupQueue.Enqueue((prefab, data));
        if (!m_IsShowing) ShowNext();
    }

    private void ShowNext()
    {
        while (m_PopupQueue.Count > 0)
        {
            var (prefab, data) = m_PopupQueue.Dequeue();
            if (prefab == null) continue;

            m_IsShowing = true;
            m_CurrentPopup = GetPopupFromPool(prefab);
            m_CurrentPopup.transform.SetAsLastSibling();

            m_CurrentPopup.Setup(data);
            m_CurrentPopup.Show();
            return;
        }
        m_IsShowing = false;
    }

    public void CloseCurrentPopup()
    {
        if (m_CurrentPopup == null || !m_IsShowing) return;

        var popupToHide = m_CurrentPopup;
        m_CurrentPopup = null;
        m_IsShowing = false;

        popupToHide.Hide(() =>
        {
            ShowNext();
        });
    }

    private PopupBase GetPopupFromPool(PopupBase prefab)
    {
        if (!m_Pool.TryGetValue(prefab, out var list))
        {
            list = m_Pool[prefab] = new List<PopupBase>();
        }

        list.RemoveAll(item => item == null);
        var pooledPopup = list.FirstOrDefault(p => !p.gameObject.activeSelf);

        if (pooledPopup != null) return pooledPopup;

        var popupObj = m_Container.InstantiatePrefab(prefab.gameObject, m_CanvasRoot);
        var newPopup = popupObj.GetComponent<PopupBase>();
        list.Add(newPopup);
        return newPopup;
    }

    public void ClearPool(bool forceDestroyActive = false)
    {
        m_PopupQueue.Clear();

        foreach (var (prefab, list) in m_Pool.ToList())
        {
            if (list == null) continue;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                var popup = list[i];
                if (popup == null) continue;

                if (popup.gameObject.activeSelf && !forceDestroyActive) continue;

                if (popup == m_CurrentPopup) m_CurrentPopup = null;
                Object.Destroy(popup.gameObject);
                list.RemoveAt(i);
            }

            if (list.Count == 0) m_Pool.Remove(prefab);
        }

        if (forceDestroyActive || m_CurrentPopup == null)
        {
            m_CurrentPopup = null;
            m_IsShowing = false;
        }
    }
}
