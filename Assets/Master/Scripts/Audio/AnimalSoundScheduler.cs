using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimalSoundScheduler : Singleton<AnimalSoundScheduler>
{
    [System.Serializable]
    public class IntervalConfig
    {
        public SoundType soundType;
        public float minInterval = 5f;
        public float maxInterval = 30f;
    }

    [SerializeField] private List<IntervalConfig> m_IntervalConfigs = new List<IntervalConfig>();

    private const float k_DefaultMinInterval = 5f;
    private const float k_DefaultMaxInterval = 30f;

    private Dictionary<SoundType, (float min, float max)> m_IntervalDict;
    private readonly Dictionary<SoundType, List<AnimalAudioBase>> m_Registered = new Dictionary<SoundType, List<AnimalAudioBase>>();
    private readonly Dictionary<SoundType, Coroutine> m_Routines = new Dictionary<SoundType, Coroutine>();

    protected override void Awake()
    {
        IsPersistent = true;
        base.Awake();
    }

    protected override void OnSingletonAwake()
    {
        m_IntervalDict = new Dictionary<SoundType, (float, float)>();
        foreach (var cfg in m_IntervalConfigs)
        {
            m_IntervalDict[cfg.soundType] = (cfg.minInterval, cfg.maxInterval);
        }
    }
    public void Register(SoundType type, AnimalAudioBase animal)
    {
        if (!m_Registered.TryGetValue(type, out var list))
        {
            list = new List<AnimalAudioBase>();
            m_Registered[type] = list;
        }

        if (!list.Contains(animal))
            list.Add(animal);

        if (!m_Routines.ContainsKey(type))
        {
            m_Routines[type] = StartCoroutine(SoundLoop(type));
        }
    }

    public void Unregister(SoundType type, AnimalAudioBase animal)
    {
        if (!m_Registered.TryGetValue(type, out var list))
            return;

        list.Remove(animal);

        if (list.Count == 0)
        {
            m_Registered.Remove(type);

            if (m_Routines.TryGetValue(type, out var routine))
            {
                if (routine != null)
                    StopCoroutine(routine);
                m_Routines.Remove(type);
            }
        }
    }

    private IEnumerator SoundLoop(SoundType type)
    {
        var (min, max) = m_IntervalDict != null && m_IntervalDict.TryGetValue(type, out var range)
            ? range
            : (k_DefaultMinInterval, k_DefaultMaxInterval);

        while (true)
        {
            float waitTime = Random.Range(min, max);
            yield return new WaitForSeconds(waitTime);

            if (m_Registered.TryGetValue(type, out var list) && list.Count > 0)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(type);
                }
                else
                {
                    Debug.LogError($"[AnimalSoundScheduler] AudioManager.Instance is null when trying to play {type}!");
                }
            }
        }
    }
}
