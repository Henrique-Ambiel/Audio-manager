using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Atributo [System.Serializable] permite que essa struct seja exibida no editor do Unity
// O AudioCue é um tipo de dado que contém informações sobre os sons a serem reproduzidos
[System.Serializable]
public struct AudioCue
{
    // Um array de AudioClip que contém diferentes amostras de áudio a serem usadas
    [SerializeField]
    private AudioClip[] audioSamples;

    // O intervalo de pitch (afinação) que pode ser ajustado no editor. O valor mínimo é 0.0 e o máximo é 3.0
    [SerializeField]
    [Range(0.0f, 3.0f)]
    private float minPitch;

    // O intervalo de pitch (afinação) para o valor máximo. O valor mínimo é 0.0 e o máximo é 3.0
    [SerializeField]
    [Range(0.0f, 3.0f)]
    private float maxPitch;

    // Espaço visual no editor para separar as propriedades
    [Space(10)]

    // O intervalo de volume mínimo para o som, entre 0.0 (mudo) e 1.0 (volume máximo)
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minVolume;

    // O intervalo de volume máximo para o som, entre 0.0 (mudo) e 1.0 (volume máximo)
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float maxVolume;


    // Método que retorna um áudio aleatório da lista de samples (sons)
    public AudioClip GetSample()
    {
        // Retorna um áudio aleatório dentro do array audioSamples
        return audioSamples[Random.Range(0, audioSamples.Length)];
    }

    // Método que retorna um valor aleatório de pitch dentro do intervalo definido (minPitch, maxPitch)
    public float GetPitch()
    {
        // Retorna um valor aleatório de pitch entre minPitch e maxPitch
        return Random.Range(minPitch, maxPitch);
    }

    // Método que retorna um valor aleatório de volume dentro do intervalo definido (minVolume, maxVolume)
    public float GetVolume()
    {
        // Retorna um valor aleatório de volume entre minVolume e maxVolume
        return Random.Range(minVolume, maxVolume);
    }
}

[System.Serializable]
public class AudioManager
{
    [SerializeField]
    private AudioMixer audioMixer = null;

    [SerializeField]
    private AudioSource musicSource = null;

    [SerializeField]
    private AudioSource sfxSource = null;

    [SerializeField]
    private List<AudioSource> sfxSourcePool = new List<AudioSource>();



    public void PlayMusic(AudioClip music, bool isLoop = true)
    {
        if (music != null) 
        {
            musicSource.Stop();

        }
    }
}

