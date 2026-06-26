using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AnimalController : IInitializable, IDisposable
{
    // This class receives signal from UI
    [Inject] private readonly SignalBus m_SignalBus;
    [Inject] private readonly UserDataService m_UserDataService;

   
    public void Initialize() => m_SignalBus.Subscribe<AddAnimalSignal>(HandleAnimal);
    public void Dispose() => m_SignalBus.Unsubscribe<AddAnimalSignal>(HandleAnimal);

    private async void HandleAnimal(AddAnimalSignal signal)
    {
        HandleAnimalAsync(signal).Forget();
    }
    private async UniTaskVoid HandleAnimalAsync(AddAnimalSignal signal)
    {
        try
        {
            bool isSuccess = await m_UserDataService.AddAnimalAsync(signal.GroupName);
            m_SignalBus.Fire(new AddAnimalResultSignal(signal.GroupName, isSuccess));
        }
        catch (Exception ex)
        {

            Debug.LogError($"[AnimalController] Critical error processing AddAnimal for {signal.GroupName}: {ex.Message}");
            m_SignalBus.Fire(new AddAnimalResultSignal(signal.GroupName, false));
        }
    }
}

