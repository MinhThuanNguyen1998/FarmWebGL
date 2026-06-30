using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PopupManager 
{
    Queue<PopupRequest> m_PopupQueue = new Queue<PopupRequest>();
    PopupBase m_CurrentPopup;
    PopupFactory m_PopupFactory;
    Transform m_CanvasRootPopup;
    bool m_IsShowing = false;
    SignalBus m_SignalBus;

    public PopupManager(PopupFactory popupFactory, Transform canvasRootPopup, SignalBus signalBus)
    {
        this.m_PopupFactory = popupFactory;
        this.m_CanvasRootPopup = canvasRootPopup;
        m_SignalBus = signalBus;
       
    }
    public void ShowPopup(PopupBase prefab, object data = null)
    {
        m_PopupQueue.Enqueue(new PopupRequest { prefab = prefab, data = data });
        if (!m_IsShowing) ShowNext();
    }
   
    void  ShowNext()
    {
        if (m_PopupQueue.Count == 0) { m_IsShowing = false; return; }

        m_IsShowing = true;
        PopupRequest request = m_PopupQueue.Dequeue();

        m_CurrentPopup = m_PopupFactory.Create(request.prefab.gameObject, m_CanvasRootPopup);
        m_CurrentPopup.Setup(request.data);
        m_CurrentPopup.Show();
    }
    public void Close()
    {
        if (m_CurrentPopup != null) 
        {
            Object.Destroy(m_CurrentPopup.gameObject); 
            m_CurrentPopup = null;
        }
        
        ShowNext(); // After closing current popup, show the next one in queue
    }
    private void CloseImmediate()
    {
        if (m_CurrentPopup != null)
        {
            Object.Destroy(m_CurrentPopup.gameObject);
            m_CurrentPopup = null;
        }

        m_PopupQueue.Clear();
        m_IsShowing = false;
    }

}
