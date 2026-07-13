using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalAudioBase : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] protected SoundType m_AnimalSoundType;

    protected virtual void OnEnable()
    {
        if (AnimalSoundScheduler.Instance != null)
        {
            AnimalSoundScheduler.Instance.Register(m_AnimalSoundType, this);
        }
        else
        {
            Debug.LogError($"[AnimalAudio] AnimalSoundScheduler.Instance is null on {gameObject.name}!");
        }
    }

    protected virtual void OnDisable()
    {
        if (AnimalSoundScheduler.Instance != null)
        {
            AnimalSoundScheduler.Instance.Unregister(m_AnimalSoundType, this);
        }
    }
}
