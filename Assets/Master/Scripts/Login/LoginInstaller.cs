using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class LoginInstaller : MonoInstaller
{
    
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        // Declare signals
        Container.DeclareSignal<LoginRequestSignal>();
        Container.DeclareSignal<LoginSuccessSignal>();
        Container.DeclareSignal<LoginFailedSignal>();

    }
}
