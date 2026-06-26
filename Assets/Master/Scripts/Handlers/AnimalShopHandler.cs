using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AnimalShopHandler : IInitializable, IDisposable
{
    // This class receives signal from UI
    [Inject] private readonly SignalBus m_SignalBus;
    [Inject] private readonly UserDataService m_UserDataService;

   
    public void Initialize() => m_SignalBus.Subscribe<AddAnimalSignal>(OnAddAnimal);
    public void Dispose() => m_SignalBus.Unsubscribe<AddAnimalSignal>(OnAddAnimal);

    private async void OnAddAnimal(AddAnimalSignal signal)
    {
        Debug.Log("OnAddAnimal at AnimalShopHandler");
        bool isSuccess = await m_UserDataService.AddAnimalAsync(signal.GroupName);
        m_SignalBus.Fire(new AddAnimalResultSignal(signal.GroupName, isSuccess));
    }
}

