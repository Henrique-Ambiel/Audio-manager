// A classe GameManager herda de IPresistentSingleton<GameManager>, 
// garantindo que exista apenas uma inst�ncia do GameManager no jogo, utilizando o padr�o Singleton.
public class GameManager : IPresistentSingleton<GameManager>
{
    // Inst�ncia do AudioManager, respons�vel por gerenciar a reprodu��o de �udio no jogo.
    // O AudioManager � criado diretamente aqui, mas tamb�m poderia ser configurado para ser gerido 
    // de forma mais flex�vel, dependendo das necessidades do jogo.
    public AudioManager audioManager = new AudioManager();
}
