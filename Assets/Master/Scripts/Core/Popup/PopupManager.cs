using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PopupManager
{
    private readonly Dictionary<string, List<PopupBase>> m_Pool = new();
    private readonly DiContainer m_Container;
    private readonly Transform m_CanvasRoot;

    private PopupBase m_CurrentPopup;
    private bool m_IsTransitioning;

    public PopupManager(DiContainer container, Transform canvasRootPopup)
    {
        m_Container = container;
        m_CanvasRoot = canvasRootPopup;
    }

    public void ShowPopup(PopupBase prefab, object data = null)
    {
        if (prefab == null || m_IsTransitioning) return;

        if (m_CurrentPopup != null)
        {
            CloseCurrentPopup();
        }

        ShowPopupAsync(prefab, data).Forget();
    }

    private async UniTaskVoid ShowPopupAsync(PopupBase prefab, object data)
    {
        m_IsTransitioning = true;

        m_CurrentPopup = GetPopupFromPool(prefab);
        m_CurrentPopup.transform.SetAsLastSibling();

        m_CurrentPopup.Setup(data);

        
        await m_CurrentPopup.ShowAsync();

        m_IsTransitioning = false;
    }

    public async void CloseCurrentPopup()
    {
        if (m_CurrentPopup == null || m_IsTransitioning) return;

        m_IsTransitioning = true;
        var popupToHide = m_CurrentPopup;

        await popupToHide.HideAsync();

        m_CurrentPopup = null;
        m_IsTransitioning = false;

    }

    private PopupBase GetPopupFromPool(PopupBase prefab)
    {
        string key = prefab.name;
        if (!m_Pool.TryGetValue(key, out var list))
        {
            list = m_Pool[key] = new List<PopupBase>();
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && !list[i].gameObject.activeSelf)
            {
                return list[i];
            }
        }

        var popupObj = m_Container.InstantiatePrefab(prefab.gameObject, m_CanvasRoot);
        var newPopup = popupObj.GetComponent<PopupBase>();
        list.Add(newPopup);
        return newPopup;
    }

    public void ClearPool(bool forceDestroyActive = false)
    {

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
            m_IsTransitioning = false;
        }
    }
}
