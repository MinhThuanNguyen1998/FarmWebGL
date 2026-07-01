using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class GameInstaller : MonoInstaller
{
    [Header("Shop Item Prefabs")]
    [SerializeField] private UIPetItem m_PetItemPrefab;

    [Header("Animal Prefabs")]
    [SerializeField] private GameObject m_CatPrefab;
    [SerializeField] private GameObject m_ChickenPrefab;

    [Header("Environment")]
    [SerializeField] private MovementArea m_MovementArea;

    [Header("Billboard Settings")]
    [SerializeField] private Transform m_TargetTransform;

    [Header("Popup")]
    [SerializeField] private Transform m_CanvasRootPopup;
    public override void InstallBindings()
    {
        // Signals
        Container.DeclareSignal<AddAnimalSignal>();
        Container.DeclareSignal<AddAnimalResultSignal>();

        // Pet item pool — initial size 5, expand as needed
        Container.BindMemoryPool<UIPetItem, UIPetItem.Pool>()
            .WithInitialSize(5)
            .FromComponentInNewPrefab(m_PetItemPrefab)
            .UnderTransformGroup("PetItemPool");

        // Animal prefab registry
        var animalPrefabs = new Dictionary<AnimalType, GameObject>
        {
            { AnimalType.cat,     m_CatPrefab     },
            { AnimalType.chicken, m_ChickenPrefab },
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
        // Register AnimalShopHandler to handle AddAnimalSignal from UI
        Container.BindInterfacesTo<AnimalController>().AsSingle();

        // Popup
        Container.Bind<PopupFactory>().AsSingle();
        Container.Bind<PopupManager>().AsSingle().WithArguments(m_CanvasRootPopup);
    }

}
