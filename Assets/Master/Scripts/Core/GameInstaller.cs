using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class GameInstaller : MonoInstaller
{
    [Header("Shop Item Prefabs")]
    [SerializeField] private UIPetItem m_PetItemPrefab;
    [SerializeField] private UIProduceItem m_ProduceItemPrefab;

    [Header("Animal Prefabs")]
    [SerializeField] private GameObject m_CatPrefab;
    [SerializeField] private GameObject m_ChickenPrefab;

    [Header("Environment")]
    [SerializeField] private MovementArea m_MovementArea;

    [Header("Billboard Settings")]
    [SerializeField] private Transform m_TargetTransform;
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

        // Animal prefab registry
        var animalPrefabs = new Dictionary<AnimalType, GameObject>
        {
            { AnimalType.Cat,     m_CatPrefab     },
            { AnimalType.Chicken, m_ChickenPrefab },
        };

        Container.BindInstance(animalPrefabs)
            .AsSingle()
            .WhenInjectedInto(typeof(AnimalSpawerBase));

        // Movement area registry
        Container.Bind<MovementArea>()
            .FromInstance(m_MovementArea)
            .AsSingle();

        // Bind BillboardManager and automatically set the target after creation
        Container.BindInterfacesAndSelfTo<BillboardManager>()
            .AsSingle()
            .OnInstantiated<BillboardManager>((ctx, manager) =>
            {
                manager.SetTarget(m_TargetTransform);
            });
    }

}
