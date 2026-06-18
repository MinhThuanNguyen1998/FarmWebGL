using UnityEngine;

public enum SoundType
{
    // Music
    None = 0,
    BackgroundMusic = 1,
   

    // SFX
    CoinEffect = 100,
    ClickedButton = 101,
   
}

[System.Serializable]
public class SoundData
{
    public SoundType type;
    public AudioClip clip;
   
}