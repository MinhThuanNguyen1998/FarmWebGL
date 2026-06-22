using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class GameInstaller : MonoInstaller
{
    [Header("Shop Item Prefabs")]
    [SerializeField] private UIPetItem m_PetItemPrefab;
    [SerializeField] private UIProduceItem m_ProduceItemPrefab;
    public override void InstallBindings()
    {
        // Pet item pool — initial size 5, expand as needed
        Container.BindMemoryPool<UIPetItem, UIPetItem.Pool>()
            .WithInitialSize(5)
            .FromComponentInNewPrefab(m_PetItemPrefab)
            .UnderTransformGroup("PetItemPool");

        // Produce item pool
        Container.BindMemoryPool<UIProduceItem, UIProduceItem.Pool>()
            .WithInitialSize(5)
            .FromComponentInNewPrefab(m_ProduceItemPrefab)
            .UnderTransformGroup("ProduceItemPool");
    }

}
