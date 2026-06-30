using UnityEngine;
using Zenject;
public class PopupFactory
{
    DiContainer container;
    public PopupFactory(DiContainer container)
    {
        this.container = container;
    }
    public PopupBase Create(GameObject prefab, Transform parent)
    {
        GameObject obj = container.InstantiatePrefab(prefab, parent);
        return obj.GetComponent<PopupBase>();
    }
}
