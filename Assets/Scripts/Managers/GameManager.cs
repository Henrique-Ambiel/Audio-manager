// A classe GameManager herda de IPresistentSingleton<GameManager>, 
// garantindo que exista apenas uma inst�ncia do GameManager no jogo, utilizando o padr�o Singleton.
public class GameManager : IPresistentSingleton<GameManager>
{
    // Inst�ncia do AudioManager, respons�vel por gerenciar a reprodu��o de �udio no jogo.
    public AudioManager audioManager = new AudioManager();

    protected override void Awake()
    {
        base.Awake();
        
        audioManager.InitializePool();
    }
}
