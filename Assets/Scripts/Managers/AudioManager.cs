using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Estrutura para armazenar diferentes configurações de áudio (ex: faixas de som, pitch, volume).
[System.Serializable]
public struct AudioCue
{
    [SerializeField]
    private AudioClip[] audioSamples;  // Array de faixas de áudio para escolher aleatoriamente.

    [SerializeField]
    [Range(0.0f, 3.0f)]
    private float minPitch;  // Pitch mínimo para o som.

    [SerializeField]
    [Range(0.0f, 3.0f)]
    private float maxPitch;  // Pitch máximo para o som.

    [Space(10)]

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minVolume;  // Volume mínimo para o som.

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float maxVolume;  // Volume máximo para o som.

    // Retorna uma faixa de áudio aleatória do array de áudio.
    public AudioClip GetSample()
    {
        return audioSamples[Random.Range(0, audioSamples.Length)];
    }

    // Retorna um pitch aleatório dentro do intervalo especificado.
    public float GetPitch()
    {
        return Random.Range(minPitch, maxPitch);
    }

    // Retorna um volume aleatório dentro do intervalo especificado.
    public float GetVolume()
    {
        return Random.Range(minVolume, maxVolume);
    }
}

// Classe que gerencia o áudio, incluindo música e efeitos sonoros.
[System.Serializable]
public class AudioManager
{
    [SerializeField]
    private AudioMixer audioMixer = null;  // Referência ao mixer de áudio.

    [SerializeField]
    private AudioSource musicSource = null;  // Fonte de áudio para música.

    [SerializeField]
    private AudioSource sfxSource = null;  // Fonte de áudio para efeitos sonoros.

    [SerializeField]
    private int poolSize = 10;  // Tamanho do pool de fontes de áudio para SFX.

    [SerializeField]
    private List<AudioSource> sfxSourcePool = new List<AudioSource>();  // Lista de fontes de áudio para SFX.
    private GameObject audioSourceInstance;  // Instância do GameObject para fontes de áudio.

    // Delegado para a ação de parar o áudio.
    public delegate void AudioStop();

    /*
     * MÚSICA
     */

    // Toca a música especificada, podendo ser em loop ou não.
    public void PlayMusic(AudioClip music, bool isLoop = true)
    {
        if (music != null)
        {
            musicSource.Stop();  // Para a música se já estiver tocando.
            musicSource.loop = isLoop;  // Define se a música será em loop.
            musicSource.volume = 1.0f;  // Define o volume da música.
            musicSource.clip = music;  // Atribui a faixa de áudio à fonte.
            musicSource.Play();  // Toca a música.
        }
    }

    // Para a música se estiver tocando.
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // Pausa ou despausa a música dependendo do parâmetro.
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

    // Retorna se a música está sendo tocada no momento.
    public bool IsPlayingMusic()
    {
        return musicSource.isPlaying;
    }

    // Faz um fade-in (gradualmente aumenta o volume) para a música.
    public void FadeInMusic(AudioClip music, float time = 0.5f)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();  // Para a música se já estiver tocando.
        }

        musicSource.clip = music;  // Atribui a faixa de áudio à fonte.
        musicSource.volume = 0.0f;  // Começa o volume em 0.
        musicSource.Play();  // Toca a música.

        GameManager.Instance.StartCoroutine(FadeMusic(true, time));  // Inicia o processo de fade-in.
    }

    // Faz um fade-out (gradualmente diminui o volume) para a música.
    public void FadeOutMusic(float time = 0.5f)
    {
        if (musicSource.isPlaying)
        {
            GameManager.Instance.StartCoroutine(FadeMusic(false, time));  // Inicia o processo de fade-out.
        }
    }

    // Corrotina que realiza o fade de volume para a música.
    private IEnumerator FadeMusic(bool isFadeIn, float time)
    {
        float deltaTime = 0.0f;
        float target = isFadeIn ? 1.0f : 0.0f;  // Define o valor final (1 para fade-in, 0 para fade-out).
        float current = musicSource.volume;  // Obtém o volume atual.

        while (deltaTime < time)
        {
            deltaTime += Time.deltaTime;  // Aumenta o tempo.
            musicSource.volume = Mathf.Lerp(current, target, deltaTime / time);  // Lerp do volume.
            yield return null;  // Espera o próximo frame.
        }

        musicSource.volume = target;  // Define o volume final.
    }

    /*
     * EFEITOS SONOROS (SFX)
     */

    // Inicializa o pool de fontes de áudio para os efeitos sonoros.
    public void InitializePool()
    {
        audioSourceInstance = new GameObject("AudioSourceInstance");  // Cria um GameObject para gerenciar as fontes de áudio.
        audioSourceInstance.transform.SetParent(GameManager.Instance.transform);  // Define o GameObject como filho do GameManager.

        // Cria as instâncias do áudio.
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

    // Toca um efeito sonoro aleatório de um AudioCue.
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

    // Ajusta o volume da música.
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

    // Função interna para tocar efeitos sonoros, podendo ser em loop ou não.
    private AudioStop InternalPlaySFX(AudioClip sfx, float volume, float pitch, bool isLoop = false)
    {
        int index = -1;

        // Procura por fontes de áudio livres no pool.
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

        // Se não encontrou uma fonte livre, cria uma nova.
        if (index == -1)
        {
            index = CreateAudioInstance();
        }

        AudioSource audioSource = index == -1 ? sfxSource : sfxSourcePool[index];  // Escolhe a fonte de áudio a ser usada.

        audioSource.volume = volume;  // Define o volume do áudio.
        audioSource.pitch = pitch;  // Define o pitch do áudio.
        audioSource.loop = isLoop;  // Define se o áudio será repetido.

        if (isLoop)
        {
            audioSource.clip = sfx;  // Atribui a faixa de áudio.
            audioSource.Play();  // Toca o áudio.
        }
        else
        {
            audioSource.PlayOneShot(sfx);  // Toca o áudio uma vez.
        }

        // Retorna uma função para parar o áudio quando necessário.
        AudioStop result;
        result = () => { audioSource.Stop(); };
        return result;
    }

    // Corrotina que toca o efeito sonoro por um tempo específico e depois para.
    private IEnumerator InternalPlaySfxForTime(AudioClip sfx, float timeToPlay, float volume = 1.0f, float pitch = 1.0f)
    {
        AudioStop audioStop = InternalPlaySFX(sfx, volume, pitch, true);
        yield return new WaitForSeconds(timeToPlay);

        audioStop();
    }

    // Cria uma nova instância de AudioSource e a adiciona ao pool.
    private int CreateAudioInstance()
    {
        AudioSource source = audioSourceInstance.AddComponent<AudioSource>();  // Cria a fonte de áudio.
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];  // Atribui o grupo de mixer de efeitos sonoros.
        source.playOnAwake = false;  // Não toca o áudio automaticamente.
        sfxSourcePool.Add(source);  // Adiciona a fonte ao pool.

        return sfxSourcePool.Count - 1;  // Retorna o índice da nova fonte criada.
    }
}
