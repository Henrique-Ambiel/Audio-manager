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
// A classe AudioManager gerencia a reprodu��o de m�sica e efeitos sonoros no jogo
public class AudioManager
{
    // Refer�ncia para o AudioMixer, utilizado para controlar o mix de �udio no jogo
    [SerializeField]
    private AudioMixer audioMixer = null;

    // Fonte de �udio para a m�sica de fundo
    [SerializeField]
    private AudioSource musicSource = null;

    // Fonte de �udio para os efeitos sonoros (SFX)
    [SerializeField]
    private AudioSource sfxSource = null;

    // Lista de fontes de �udio que podem ser usadas para reproduzir efeitos sonoros
    [SerializeField]
    private List<AudioSource> sfxSourcePool = new List<AudioSource>();


    // M�todo para iniciar a reprodu��o de m�sica
    // Recebe o clip de m�sica e um valor booleano para determinar se deve ser repetido (loop)
    public void PlayMusic(AudioClip music, bool isLoop = true)
    {
        // Verifica se o �udio de m�sica n�o � nulo
        if (music != null)
        {
            // Para a m�sica se j� estiver tocando
            musicSource.Stop();

            // Configura a m�sica para repetir ou n�o
            musicSource.loop = isLoop;

            // Configura o volume para o m�ximo (1.0f)
            musicSource.volume = 1.0f;

            // Define o clip de m�sica e come�a a reprodu��o
            musicSource.clip = music;
            musicSource.Play();
        }
    }

    // M�todo para parar a m�sica
    public void StopMusic()
    {
        // Verifica se a m�sica est� tocando antes de parar
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // M�todo para pausar ou retomar a m�sica
    // O par�metro isPause define se a m�sica deve ser pausada (true) ou retomada (false)
    public void PauseMusic(bool isPause)
    {
        // Pausa a m�sica se isPause for true
        if (isPause == true)
        {
            musicSource.Pause();
        }
        else
        {
            // Retoma a m�sica se isPause for false
            musicSource.UnPause();
        }
    }

    // M�todo que verifica se a m�sica est� sendo reproduzida
    public bool IsPlayingMusic()
    {
        return musicSource.isPlaying;
    }

    // M�todo para realizar o fade-in (aumento gradual do volume) da m�sica
    // Recebe o clip de m�sica e o tempo para o fade-in
    public void FadeInMusic(AudioClip music, float time = 0.5f)
    {
        // Se a m�sica j� estiver tocando, a para antes de iniciar o fade-in
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        // Configura o clip de m�sica e o volume inicial para 0 (sil�ncio)
        musicSource.clip = music;
        musicSource.volume = 0.0f;

        // Inicia a reprodu��o da m�sica
        musicSource.Play();

        // Inicia a corrotina para o fade-in da m�sica
        GameManager.Instance.StartCoroutine(FadeMusic(true, time));
    }

    // M�todo para realizar o fade-out (diminui��o gradual do volume) da m�sica
    // Recebe o tempo de dura��o do fade-out
    public void FadeOutMusic(float time = 0.5f)
    {
        // Se a m�sica estiver tocando, inicia o fade-out
        if (musicSource.isPlaying)
        {
            GameManager.Instance.StartCoroutine(FadeMusic(false, time));
        }
    }

    // Corrotina que faz o fade-in ou fade-out da m�sica, dependendo do par�metro isFadeIn
    private IEnumerator FadeMusic(bool isFadeIn, float time)
    {
        // Vari�vel para controlar o tempo de execu��o do fade
        float deltaTime = 0.0f;

        // O alvo para o volume ser� 1.0f para fade-in (volume m�ximo) ou 0.0f para fade-out (volume m�nimo)
        float target = isFadeIn ? 1.0f : 0.0f;

        // O volume inicial da m�sica
        float current = musicSource.volume;

        // Enquanto o tempo de fade n�o atingir o valor final
        while (deltaTime < time)
        {
            // Incrementa o tempo decorrido
            deltaTime += Time.deltaTime;

            // Interpola o volume entre o valor atual e o alvo usando Lerp
            musicSource.volume = Mathf.Lerp(current, target, deltaTime / time);

            // Aguarda o pr�ximo frame antes de continuar o fade
            yield return null;
        }

        // Garante que o volume final ser� exatamente o alvo (0 ou 1)
        musicSource.volume = target;
    }
}

