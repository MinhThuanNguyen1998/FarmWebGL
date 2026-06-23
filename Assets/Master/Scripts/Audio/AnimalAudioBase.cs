using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalAudioBase : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] protected SoundType m_AnimalSoundType;
    [Header("Interval Settings (Random)")]
  
    [SerializeField] protected float m_MinInterval = 5f;
    [SerializeField] protected float m_MaxInterval = 30f;

    private Coroutine m_LoopCoroutine;
    protected virtual void OnEnable()
    {
        m_LoopCoroutine = StartCoroutine(SoundLoopRoutine());
    }

    protected virtual void OnDisable()
    {
        if (m_LoopCoroutine != null)
        {
            StopCoroutine(m_LoopCoroutine);
        }
    }

    private IEnumerator SoundLoopRoutine()
    {
        while (true)
        {
            float randomWaitTime = Random.Range(m_MinInterval, m_MaxInterval);
            yield return new WaitForSeconds(randomWaitTime);
            PlaySound();
        }
    }

   
    protected virtual void PlaySound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(m_AnimalSoundType);
        }
        else
        {
            Debug.LogError($"[AnimalAudio] AudioManager.Instance is null on {gameObject.name}!");
        }
    }
}
