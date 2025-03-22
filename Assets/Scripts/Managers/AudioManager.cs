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
    private int poolSize = 10;

    [SerializeField]
    private List<AudioSource> sfxSourcePool = new List<AudioSource>();
    private GameObject audioSourceInstance;

    /*
     * MUSIC
     */

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

    /*
     * SFX
     */

    public void InitializePool()
    {
        audioSourceInstance = new GameObject("AudioSourceInstance");
        audioSourceInstance.transform.SetParent(GameManager.Instance.transform);

        for (int i = 0; i < poolSize; i++)
        {
            CreateAudioInstance();
        }
    }

    public void PlaySfx(AudioClip sfx, float volume = 1.0f, float pitch = 1.0f)
    {
        InternalPlaySFX(sfx, volume, pitch, false);
    }

    public void PlaySfx(AudioCue sfx)
    {
        InternalPlaySFX(sfx.GetSample(), sfx.GetVolume(), sfx.GetPitch(), false);
    }

    public void PlaySfxInLoop(AudioClip sfx, float volume = 1.0f, float pitch = 1.0f)
    {
        InternalPlaySFX(sfx, volume, pitch, true);
    }

    public void PauseSfx(bool isPause)
    {
       if (isPause)
        {
            sfxSource.Pause();
            for (int i = 0; i < sfxSourcePool.Count; i++)
            {
                sfxSourcePool[i].Pause();
            }
        }
        else
        {
            sfxSource.UnPause();
            for (int i = 0; i < sfxSourcePool.Count; i++)
            {
                sfxSourcePool[i].UnPause();
            }
        }
    }

    public void StopSfx()
    {
       sfxSource.Stop();
       for (int i = 0; i < sfxSourcePool.Count; i++)
       {
            sfxSourcePool[i].Stop();
       }
    }

    private void InternalPlaySFX(AudioClip sxf, float volume, float pitch, bool isLoop = false)
    {
        int index = -1;


        if(sfxSource.isPlaying)
        {
            for (int i = 0; i < sfxSourcePool.Count; i++)
            {
                if (!sfxSourcePool[i].isPlaying)
                {
                    index = i;
                    break;
                }
            }
        }

        if(index == -1)
        {
           index = CreateAudioInstance();
        }

        AudioSource result = index == -1 ? sfxSource : sfxSourcePool[index];

        result.volume = volume;
        result.pitch = pitch;
        result.loop = isLoop;

        if (isLoop)
        {
            result.clip = sxf;
            result.Play();
        }
        else
        {
            result.PlayOneShot(sxf);
        }
       
    }

    private int CreateAudioInstance()
    {
        AudioSource source = audioSourceInstance.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SoundEfects")[0];
        source.playOnAwake = false;
        sfxSourcePool.Add(source);

        return sfxSourcePool.Count - 1;
    }
}
