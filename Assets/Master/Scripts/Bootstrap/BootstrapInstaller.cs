using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private LoadingBar m_LoadingBar;
    public override void InstallBindings()
    {
        Container.Bind<LoadingBar>().FromInstance(m_LoadingBar).AsSingle().NonLazy();
        Container.Bind<AuthService>().AsSingle().NonLazy();
        Container.Bind<SceneLoader>().AsSingle().NonLazy();
        Container.Bind<UserDataService>().AsSingle().NonLazy();
    }
}