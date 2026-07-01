using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private LoadingBar m_LoadingBar;
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.Bind<LoadingBar>().FromInstance(m_LoadingBar).AsSingle().NonLazy();
        Container.Bind<AuthService>().AsSingle().NonLazy();
        Container.Bind<SceneLoader>().AsSingle().NonLazy();
        Container.Bind<NetworkService>().AsSingle().NonLazy();
        Container.Bind<InventoryService>().AsSingle().NonLazy();
        Container.Bind<UserDataService>().AsSingle().NonLazy();

        // Signals
        Container.DeclareSignal <UserDataLoadedSignal>();
        Container.DeclareSignal<InventoryLoadedSignal>();
        Container.DeclareSignal<RewardClaimedSignal>();
    }
}