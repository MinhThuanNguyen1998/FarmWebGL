using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public abstract class AnimalSpawerBase : MonoBehaviour
{
    [SerializeField] protected Transform m_SpawnPoint;
    [SerializeField] protected string m_GroupName;
    [SerializeField] private AnimalType m_AnimalType;

    [SerializeField] private int m_InitialPoolSize = 10;
    [SerializeField] private int m_MaxPoolSize = 50;

    protected SignalBus m_SignalBus;
    protected UserDataService m_UserDataService;

    private IAnimalPool m_AnimalPool;

    [Inject]
    public void Construct(
        SignalBus signalBus,
        UserDataService userDataService,
        Dictionary<AnimalType, GameObject> animalPrefabs,
        DiContainer container)
    {
        m_SignalBus = signalBus;
        m_UserDataService = userDataService;

        if (!animalPrefabs.TryGetValue(m_AnimalType, out var prefab) || prefab == null)
        {
            Debug.LogError($"[{name}] Can not find an AnimalType.{m_AnimalType} in GameInstaller.");
            return;
        }

        var poolParent = new GameObject($"[Pool_{gameObject.name}]").transform;
        poolParent.SetParent(this.transform);
        m_AnimalPool = new AnimalPool(prefab, poolParent, m_InitialPoolSize, m_MaxPoolSize, container);
    }

    protected virtual void OnEnable()
    {
        m_SignalBus.Subscribe<UserDataLoadedSignal>(OnUserDataLoaded);
        if (m_UserDataService.IsLoaded)
            UpdateAnimals(m_UserDataService.Data);
    }

    protected virtual void OnDisable() 
    {
        m_SignalBus.Unsubscribe<UserDataLoadedSignal>(OnUserDataLoaded);
    }

    private void OnUserDataLoaded(UserDataLoadedSignal signal) => UpdateAnimals(signal.Data);

    protected abstract void UpdateAnimals(UserData data);

    public virtual GameObject SpawnAnimal(FarmAnimal farmAnimal)
    {
        var animal = m_AnimalPool.Spawn(m_SpawnPoint.position, m_SpawnPoint.rotation);
        if (animal == null) return null;

        animal.GetComponent<UIAnimalDaysLeft>()?.UpdateUIDaysLeft(farmAnimal);

        return animal;

    }

    public virtual void DespawnAnimal(GameObject animal)
    {
        if (animal == null) return;
        m_AnimalPool.Despawn(animal);
    }

}
