// A classe GameManager herda de IPresistentSingleton<GameManager>, 
// garantindo que exista apenas uma instância do GameManager no jogo, utilizando o padrão Singleton.
public class GameManager : IPresistentSingleton<GameManager>
{
    // Instância do AudioManager, responsável por gerenciar a reprodução de áudio no jogo.
    public AudioManager audioManager = new AudioManager();

    protected override void Awake()
    {
        base.Awake();
        
        audioManager.InitializePool();
    }
}
