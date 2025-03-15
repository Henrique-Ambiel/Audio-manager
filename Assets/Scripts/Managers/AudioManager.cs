using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Atributo [System.Serializable] permite que essa struct seja exibida no editor do Unity
// O AudioCue � um tipo de dado que cont�m informa��es sobre os sons a serem reproduzidos
[System.Serializable]
public struct AudioCue
{
    // Um array de AudioClip que cont�m diferentes amostras de �udio a serem usadas
    [SerializeField]
    private AudioClip[] audioSamples;

    // O intervalo de pitch (afina��o) que pode ser ajustado no editor. O valor m�nimo � 0.0 e o m�ximo � 3.0
    [SerializeField]
    [Range(0.0f, 3.0f)]
    private float minPitch;

    // O intervalo de pitch (afina��o) para o valor m�ximo. O valor m�nimo � 0.0 e o m�ximo � 3.0
    [SerializeField]
    [Range(0.0f, 3.0f)]
    private float maxPitch;

    // Espa�o visual no editor para separar as propriedades
    [Space(10)]

    // O intervalo de volume m�nimo para o som, entre 0.0 (mudo) e 1.0 (volume m�ximo)
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minVolume;

    // O intervalo de volume m�ximo para o som, entre 0.0 (mudo) e 1.0 (volume m�ximo)
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float maxVolume;


    // M�todo que retorna um �udio aleat�rio da lista de samples (sons)
    public AudioClip GetSample()
    {
        // Retorna um �udio aleat�rio dentro do array audioSamples
        return audioSamples[Random.Range(0, audioSamples.Length)];
    }

    // M�todo que retorna um valor aleat�rio de pitch dentro do intervalo definido (minPitch, maxPitch)
    public float GetPitch()
    {
        // Retorna um valor aleat�rio de pitch entre minPitch e maxPitch
        return Random.Range(minPitch, maxPitch);
    }

    // M�todo que retorna um valor aleat�rio de volume dentro do intervalo definido (minVolume, maxVolume)
    public float GetVolume()
    {
        // Retorna um valor aleat�rio de volume entre minVolume e maxVolume
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
            musicSource.loop = isLoop;
            musicSource.volume = 1.0f;
            musicSource.clip = music;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying) 
        { 
            musicSource.Stop(); 
        }
    }

    public void PauseMusic(bool isPause)
    {
        if (isPause == true)
        {
           musicSource.Pause();
        }
        else
        {
            musicSource.UnPause();
        }
    }

    public bool IsPlayingMusic()
    {
        return musicSource.isPlaying;
    }

    private IEnumerator FadeMusic(bool isFadeIn, float time)
    {
       float deltaTime = 0.0f;
       float target = isFadeIn ? 1.0f : 0.0f;
       float current = musicSource.volume;


        while (deltaTime < time) 
        {
            deltaTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(current, target, deltaTime / time);
            yield return null;
        }

        musicSource.volume = target;
    }
}

