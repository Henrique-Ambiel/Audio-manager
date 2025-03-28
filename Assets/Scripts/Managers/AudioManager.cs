using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Estrutura para armazenar diferentes configura��es de �udio (ex: faixas de som, pitch, volume).
[System.Serializable]
public struct AudioCue
{
    [SerializeField]
    private AudioClip[] audioSamples;  // Array de faixas de �udio para escolher aleatoriamente.

    [SerializeField]
    [Range(0.0f, 3.0f)]
    private float minPitch;  // Pitch m�nimo para o som.

    [SerializeField]
    [Range(0.0f, 3.0f)]
    private float maxPitch;  // Pitch m�ximo para o som.

    [Space(10)]

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minVolume;  // Volume m�nimo para o som.

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float maxVolume;  // Volume m�ximo para o som.

    // Retorna uma faixa de �udio aleat�ria do array de �udio.
    public AudioClip GetSample()
    {
        return audioSamples[Random.Range(0, audioSamples.Length)];
    }

    // Retorna um pitch aleat�rio dentro do intervalo especificado.
    public float GetPitch()
    {
        return Random.Range(minPitch, maxPitch);
    }

    // Retorna um volume aleat�rio dentro do intervalo especificado.
    public float GetVolume()
    {
        return Random.Range(minVolume, maxVolume);
    }
}

// Classe que gerencia o �udio, incluindo m�sica e efeitos sonoros.
[System.Serializable]
public class AudioManager
{
    [SerializeField]
    private AudioMixer audioMixer = null;  // Refer�ncia ao mixer de �udio.

    [SerializeField]
    private AudioSource musicSource = null;  // Fonte de �udio para m�sica.

    [SerializeField]
    private AudioSource sfxSource = null;  // Fonte de �udio para efeitos sonoros.

    [SerializeField]
    private int poolSize = 10;  // Tamanho do pool de fontes de �udio para SFX.

    [SerializeField]
    private List<AudioSource> sfxSourcePool = new List<AudioSource>();  // Lista de fontes de �udio para SFX.
    private GameObject audioSourceInstance;  // Inst�ncia do GameObject para fontes de �udio.

    // Delegado para a a��o de parar o �udio.
    public delegate void AudioStop();

    /*
     * M�SICA
     */

    // Toca a m�sica especificada, podendo ser em loop ou n�o.
    public void PlayMusic(AudioClip music, bool isLoop = true)
    {
        if (music != null)
        {
            musicSource.Stop();  // Para a m�sica se j� estiver tocando.
            musicSource.loop = isLoop;  // Define se a m�sica ser� em loop.
            musicSource.volume = 1.0f;  // Define o volume da m�sica.
            musicSource.clip = music;  // Atribui a faixa de �udio � fonte.
            musicSource.Play();  // Toca a m�sica.
        }
    }

    // Para a m�sica se estiver tocando.
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // Pausa ou despausa a m�sica dependendo do par�metro.
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

    // Retorna se a m�sica est� sendo tocada no momento.
    public bool IsPlayingMusic()
    {
        return musicSource.isPlaying;
    }

    // Faz um fade-in (gradualmente aumenta o volume) para a m�sica.
    public void FadeInMusic(AudioClip music, float time = 0.5f)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();  // Para a m�sica se j� estiver tocando.
        }

        musicSource.clip = music;  // Atribui a faixa de �udio � fonte.
        musicSource.volume = 0.0f;  // Come�a o volume em 0.
        musicSource.Play();  // Toca a m�sica.

        GameManager.Instance.StartCoroutine(FadeMusic(true, time));  // Inicia o processo de fade-in.
    }

    // Faz um fade-out (gradualmente diminui o volume) para a m�sica.
    public void FadeOutMusic(float time = 0.5f)
    {
        if (musicSource.isPlaying)
        {
            GameManager.Instance.StartCoroutine(FadeMusic(false, time));  // Inicia o processo de fade-out.
        }
    }

    // Corrotina que realiza o fade de volume para a m�sica.
    private IEnumerator FadeMusic(bool isFadeIn, float time)
    {
        float deltaTime = 0.0f;
        float target = isFadeIn ? 1.0f : 0.0f;  // Define o valor final (1 para fade-in, 0 para fade-out).
        float current = musicSource.volume;  // Obt�m o volume atual.

        while (deltaTime < time)
        {
            deltaTime += Time.deltaTime;  // Aumenta o tempo.
            musicSource.volume = Mathf.Lerp(current, target, deltaTime / time);  // Lerp do volume.
            yield return null;  // Espera o pr�ximo frame.
        }

        musicSource.volume = target;  // Define o volume final.
    }

    /*
     * EFEITOS SONOROS (SFX)
     */

    // Inicializa o pool de fontes de �udio para os efeitos sonoros.
    public void InitializePool()
    {
        audioSourceInstance = new GameObject("AudioSourceInstance");  // Cria um GameObject para gerenciar as fontes de �udio.
        audioSourceInstance.transform.SetParent(GameManager.Instance.transform);  // Define o GameObject como filho do GameManager.

        // Cria as inst�ncias do �udio.
        for (int i = 0; i < poolSize; i++)
        {
            CreateAudioInstance();
        }
    }

    // Toca um efeito sonoro uma vez, sem loop.
    public void PlaySfx(AudioClip sfx, float volume = 1.0f, float pitch = 1.0f)
    {
        InternalPlaySFX(sfx, volume, pitch, false);
    }

    // Toca um efeito sonoro aleat�rio de um AudioCue.
    public void PlaySfx(AudioCue sfx)
    {
        InternalPlaySFX(sfx.GetSample(), sfx.GetVolume(), sfx.GetPitch(), false);
    }

    // Toca um efeito sonoro em loop.
    public AudioStop PlaySfxInLoop(AudioClip sfx, float volume = 1.0f, float pitch = 1.0f)
    {
        return InternalPlaySFX(sfx, volume, pitch, true);
    }

    // Toca um efeito sonoro por um tempo limitado.
    public void PlaySfxForTime(AudioClip sfx, float timeToPlay, float volume = 1.0f, float pitch = 1.0f)
    {
        GameManager.Instance.StartCoroutine(InternalPlaySfxForTime(sfx, timeToPlay, volume, pitch));
    }

    public void PlaySfxForTime(AudioCue sfx, float timeToPlay)
    {
        GameManager.Instance.StartCoroutine(InternalPlaySfxForTime(sfx.GetSample(), timeToPlay, sfx.GetVolume(), sfx.GetPitch()));
    }

    // Toca um efeito sonoro em loop a partir de um AudioCue.
    public AudioStop PlaySfxInLoop(AudioCue sfx)
    {
        return InternalPlaySFX(sfx.GetSample(), sfx.GetVolume(), sfx.GetPitch(), true);
    }

    // Pausa ou despausa os efeitos sonoros.
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

    // Para todos os efeitos sonoros.
    public void StopAllSfx()
    {
        sfxSource.Stop();
        for (int i = 0; i < sfxSourcePool.Count; i++)
        {
            sfxSourcePool[i].Stop();
        }
    }

    // Ajusta o volume master (geral).
    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.001f, 1.0f);
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    // Ajusta o volume da m�sica.
    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.001f, 1.0f);
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    // Ajusta o volume dos efeitos sonoros.
    public void SetSfxVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.001f, 1.0f);
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }

    // Fun��o interna para tocar efeitos sonoros, podendo ser em loop ou n�o.
    private AudioStop InternalPlaySFX(AudioClip sfx, float volume, float pitch, bool isLoop = false)
    {
        int index = -1;

        // Procura por fontes de �udio livres no pool.
        if (sfxSource.isPlaying)
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

        // Se n�o encontrou uma fonte livre, cria uma nova.
        if (index == -1)
        {
            index = CreateAudioInstance();
        }

        AudioSource audioSource = index == -1 ? sfxSource : sfxSourcePool[index];  // Escolhe a fonte de �udio a ser usada.

        audioSource.volume = volume;  // Define o volume do �udio.
        audioSource.pitch = pitch;  // Define o pitch do �udio.
        audioSource.loop = isLoop;  // Define se o �udio ser� repetido.

        if (isLoop)
        {
            audioSource.clip = sfx;  // Atribui a faixa de �udio.
            audioSource.Play();  // Toca o �udio.
        }
        else
        {
            audioSource.PlayOneShot(sfx);  // Toca o �udio uma vez.
        }

        // Retorna uma fun��o para parar o �udio quando necess�rio.
        AudioStop result;
        result = () => { audioSource.Stop(); };
        return result;
    }

    // Corrotina que toca o efeito sonoro por um tempo espec�fico e depois para.
    private IEnumerator InternalPlaySfxForTime(AudioClip sfx, float timeToPlay, float volume = 1.0f, float pitch = 1.0f)
    {
        AudioStop audioStop = InternalPlaySFX(sfx, volume, pitch, true);
        yield return new WaitForSeconds(timeToPlay);

        audioStop();
    }

    // Cria uma nova inst�ncia de AudioSource e a adiciona ao pool.
    private int CreateAudioInstance()
    {
        AudioSource source = audioSourceInstance.AddComponent<AudioSource>();  // Cria a fonte de �udio.
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];  // Atribui o grupo de mixer de efeitos sonoros.
        source.playOnAwake = false;  // N�o toca o �udio automaticamente.
        sfxSourcePool.Add(source);  // Adiciona a fonte ao pool.

        return sfxSourcePool.Count - 1;  // Retorna o �ndice da nova fonte criada.
    }
}
