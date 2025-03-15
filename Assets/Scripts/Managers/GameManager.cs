// A classe GameManager herda de IPresistentSingleton<GameManager>, 
// garantindo que exista apenas uma instância do GameManager no jogo, utilizando o padrão Singleton.
public class GameManager : IPresistentSingleton<GameManager>
{
    // Instância do AudioManager, responsável por gerenciar a reprodução de áudio no jogo.
    // O AudioManager é criado diretamente aqui, mas também poderia ser configurado para ser gerido 
    // de forma mais flexível, dependendo das necessidades do jogo.
    public AudioManager audioManager = new AudioManager();
}
