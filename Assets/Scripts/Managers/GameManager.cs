// A classe GameManager herda de IPresistentSingleton<GameManager>, 
// garantindo que exista apenas uma inst�ncia do GameManager no jogo, utilizando o padr�o Singleton.
public class GameManager : IPresistentSingleton<GameManager>
{
    // Inst�ncia do AudioManager, respons�vel por gerenciar a reprodu��o de �udio no jogo.
    public AudioManager audioManager = new AudioManager();

    // M�todo chamado ao iniciar o GameManager.
    protected override void Awake()
    {
        base.Awake();  // Chama o m�todo Awake da classe base (IPresistentSingleton).

        // Inicializa o pool de fontes de �udio para os efeitos sonoros.
        audioManager.InitializePool();

        // Chama o m�todo PlaySfxInLoop do AudioManager para tocar um efeito sonoro em loop. 
        // Neste caso, o par�metro `null` significa que n�o foi passado um efeito espec�fico,
        // ent�o deve ser substitu�do por um efeito sonoro v�lido ou um AudioCue.
        AudioManager.AudioStop stopFrog = audioManager.PlaySfxInLoop(null);

        // Chama a fun��o de stop retornada por PlaySfxInLoop para parar o �udio.
        stopFrog();
    }
}
