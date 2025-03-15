using System.Collections.Generic;
using System.Collections;
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
// A classe AudioManager gerencia a reprodução de música e efeitos sonoros no jogo
public class AudioManager
{
    // Referência para o AudioMixer, utilizado para controlar o mix de áudio no jogo
    [SerializeField]
    private AudioMixer audioMixer = null;

    // Fonte de áudio para a música de fundo
    [SerializeField]
    private AudioSource musicSource = null;

    // Fonte de áudio para os efeitos sonoros (SFX)
    [SerializeField]
    private AudioSource sfxSource = null;

    // Lista de fontes de áudio que podem ser usadas para reproduzir efeitos sonoros
    [SerializeField]
    private List<AudioSource> sfxSourcePool = new List<AudioSource>();


    // Método para iniciar a reprodução de música
    // Recebe o clip de música e um valor booleano para determinar se deve ser repetido (loop)
    public void PlayMusic(AudioClip music, bool isLoop = true)
    {
        // Verifica se o áudio de música não é nulo
        if (music != null)
        {
            // Para a música se já estiver tocando
            musicSource.Stop();

            // Configura a música para repetir ou não
            musicSource.loop = isLoop;

            // Configura o volume para o máximo (1.0f)
            musicSource.volume = 1.0f;

            // Define o clip de música e começa a reprodução
            musicSource.clip = music;
            musicSource.Play();
        }
    }

    // Método para parar a música
    public void StopMusic()
    {
        // Verifica se a música está tocando antes de parar
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // Método para pausar ou retomar a música
    // O parâmetro isPause define se a música deve ser pausada (true) ou retomada (false)
    public void PauseMusic(bool isPause)
    {
        // Pausa a música se isPause for true
        if (isPause == true)
        {
            musicSource.Pause();
        }
        else
        {
            // Retoma a música se isPause for false
            musicSource.UnPause();
        }
    }

    // Método que verifica se a música está sendo reproduzida
    public bool IsPlayingMusic()
    {
        return musicSource.isPlaying;
    }

    // Método para realizar o fade-in (aumento gradual do volume) da música
    // Recebe o clip de música e o tempo para o fade-in
    public void FadeInMusic(AudioClip music, float time = 0.5f)
    {
        // Se a música já estiver tocando, a para antes de iniciar o fade-in
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        // Configura o clip de música e o volume inicial para 0 (silêncio)
        musicSource.clip = music;
        musicSource.volume = 0.0f;

        // Inicia a reprodução da música
        musicSource.Play();

        // Inicia a corrotina para o fade-in da música
        GameManager.Instance.StartCoroutine(FadeMusic(true, time));
    }

    // Método para realizar o fade-out (diminuição gradual do volume) da música
    // Recebe o tempo de duração do fade-out
    public void FadeOutMusic(float time = 0.5f)
    {
        // Se a música estiver tocando, inicia o fade-out
        if (musicSource.isPlaying)
        {
            GameManager.Instance.StartCoroutine(FadeMusic(false, time));
        }
    }

    // Corrotina que faz o fade-in ou fade-out da música, dependendo do parâmetro isFadeIn
    private IEnumerator FadeMusic(bool isFadeIn, float time)
    {
        // Variável para controlar o tempo de execução do fade
        float deltaTime = 0.0f;

        // O alvo para o volume será 1.0f para fade-in (volume máximo) ou 0.0f para fade-out (volume mínimo)
        float target = isFadeIn ? 1.0f : 0.0f;

        // O volume inicial da música
        float current = musicSource.volume;

        // Enquanto o tempo de fade não atingir o valor final
        while (deltaTime < time)
        {
            // Incrementa o tempo decorrido
            deltaTime += Time.deltaTime;

            // Interpola o volume entre o valor atual e o alvo usando Lerp
            musicSource.volume = Mathf.Lerp(current, target, deltaTime / time);

            // Aguarda o próximo frame antes de continuar o fade
            yield return null;
        }

        // Garante que o volume final será exatamente o alvo (0 ou 1)
        musicSource.volume = target;
    }
}

