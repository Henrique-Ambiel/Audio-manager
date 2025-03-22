// A classe GameManager herda de IPresistentSingleton<GameManager>, 
// garantindo que exista apenas uma instância do GameManager no jogo, utilizando o padrão Singleton.
public class GameManager : IPresistentSingleton<GameManager>
{
    // Instância do AudioManager, responsável por gerenciar a reprodução de áudio no jogo.
    public AudioManager audioManager = new AudioManager();

    // Método chamado ao iniciar o GameManager.
    protected override void Awake()
    {
        base.Awake();  // Chama o método Awake da classe base (IPresistentSingleton).

        // Inicializa o pool de fontes de áudio para os efeitos sonoros.
        audioManager.InitializePool();

        // Chama o método PlaySfxInLoop do AudioManager para tocar um efeito sonoro em loop. 
        // Neste caso, o parâmetro `null` significa que não foi passado um efeito específico,
        // então deve ser substituído por um efeito sonoro válido ou um AudioCue.
        AudioManager.AudioStop stopFrog = audioManager.PlaySfxInLoop(null);

        // Chama a função de stop retornada por PlaySfxInLoop para parar o áudio.
        stopFrog();
    }
}
