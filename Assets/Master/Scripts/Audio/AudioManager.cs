using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource m_MusicSource;
    [SerializeField] private AudioSource m_SFXSource;
    [SerializeField] private List<SoundData> m_SoundsList;

   
    private Dictionary<SoundType, AudioClip> m_SoundDict;

    protected override void Awake()
    {
        IsPersistent = true;
        base.Awake();

    }
    [Inject]
    public void Initialize()
    {
        m_SoundDict = new Dictionary<SoundType, AudioClip>();
        foreach (var sound in m_SoundsList)
        {
            if (!m_SoundDict.ContainsKey(sound.type))
                m_SoundDict[sound.type] = sound.clip;
        }
    }

    public void PlayBackgroundMusic(SoundType type, bool loop = true)
    {
        if (!m_SoundDict.TryGetValue(type, out var clip))
        {
            Debug.LogWarning($"[AudioManager] Music not found for: {type}");
            return;
        }

        if (m_MusicSource.clip == clip && m_MusicSource.isPlaying)
            return;

        m_MusicSource.clip = clip;
        m_MusicSource.loop = loop;
        m_MusicSource.Play();
    }

    public void PlaySFX(SoundType type)
    { 
        if (m_SoundDict.TryGetValue(type, out var clip)) m_SFXSource.PlayOneShot(clip);
      
        else Debug.LogWarning($"[AudioManager] SFX not found for: {type}");
       
    }

    public void StopMusic() => m_MusicSource.Stop();

    public void SetMusicVolume(float value)
    {
        m_MusicSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        m_SFXSource.volume = value;
    }

}


