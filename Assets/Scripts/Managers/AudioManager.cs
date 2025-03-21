using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct AudioCue
{
    [SerializeField]
    private AudioClip[] audioSamples;

    [SerializeField]
    [Range(0.0f, 3.0f)]
    private float minPitch;

    [SerializeField]
    [Range(0.0f, 3.0f)]
    private float maxPitch;

    [Space(10)]

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minVolume;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float maxVolume;


    public AudioClip GetSample()
    {
        return audioSamples[Random.Range(0, audioSamples.Length)];
    }

    public float GetPitch()
    {
        return Random.Range(minPitch, maxPitch);
    }

    public float GetVolume()
    {
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

    public void FadeInMusic(AudioClip music, float time = 0.5f)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        musicSource.clip = music;
        musicSource.volume = 0.0f;

        musicSource.Play();

        GameManager.Instance.StartCoroutine(FadeMusic(true, time));
    }

    public void FadeOutMusic(float time = 0.5f)
    {
        if (musicSource.isPlaying)
        {
            GameManager.Instance.StartCoroutine(FadeMusic(false, time));
        }
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

    public void IntializePool()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject newInstance = new GameObject("AudioSourceInstance");
            newInstance.transform.SetParent(GameManager.Instance.transform);
            AudioSource source = newInstance.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SoundEfects")[0];
        }
    }
}