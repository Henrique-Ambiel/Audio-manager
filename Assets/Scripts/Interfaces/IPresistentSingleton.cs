using UnityEngine;

// A classe IPresistentSingleton<T> implementa o padrão de design Singleton para garantir que exista
// apenas uma instância de um objeto T (herdado de MonoBehaviour) durante a execução do jogo.
public class IPresistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // A variável estática que guarda a única instância do tipo T
    private static T _uniqueInstance = null;

    // Flag usada para impedir a criação de novas instâncias quando o aplicativo estiver saindo no Editor Unity
#if UNITY_EDITOR
    private static bool _isApplicationQuiting = false;
#endif

    // Propriedade que acessa a instância única da classe
    public static T Instance
    {
        get
        {
            // Se a instância única ainda não foi criada e o aplicativo não está saindo, tenta obter ou criar uma nova instância
            if (_uniqueInstance == null
#if UNITY_EDITOR && !UNITY_WEBGL
                && !_isApplicationQuiting
#endif
                )
            {
                // Busca a instância na cena (caso já tenha sido criada)
                _uniqueInstance = FindFirstObjectByType<T>();
                if (_uniqueInstance == null)
                {
                    // Se não encontrou, tenta carregar o prefab correspondente ao tipo T
                    GameObject SingletonPrefab = Resources.Load<GameObject>(typeof(T).Name);
                    if (SingletonPrefab)
                    {
                        // Se o prefab foi encontrado, instancia-o e obtém a instância de T
                        GameObject SingletonObject = Instantiate<GameObject>(SingletonPrefab);
                        if (SingletonObject != null)
                            _uniqueInstance = SingletonObject.GetComponent<T>();
                    }
                    // Se ainda não encontrou a instância, cria um novo GameObject com o tipo T
                    if (_uniqueInstance == null)
                        _uniqueInstance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return _uniqueInstance;
        }
        private set
        {
            // Se a instância ainda não foi definida, define-a e impede que seja destruída ao carregar uma nova cena
            if (null == _uniqueInstance)
            {
                _uniqueInstance = value;
                DontDestroyOnLoad(_uniqueInstance.gameObject);
            }
            // Se a instância já existe, e não é a mesma, emite um erro e destrói a nova instância
            else if (_uniqueInstance != value)
            {
#if UNITY_EDITOR && !UNITY_WEBGL
                // Exibe um erro no Editor Unity, alertando sobre a tentativa de criar uma segunda instância
                Debug.LogError("[" + typeof(T).Name + "] Tentou instanciar uma segunda instância da classe IPresistentSingleton.");
#endif
                // Destroi o novo GameObject que foi instanciado para garantir que apenas a primeira instância exista
                DestroyImmediate(value.gameObject);
            }
        }
    }

    // Método que verifica se a instância já foi criada (se está inicializada)
    public static bool IsInitialized()
    {
        return _uniqueInstance != null;
    }

    // Método chamado quando o script é carregado
    // Define a instância única da classe para o próprio objeto (this)
    protected virtual void Awake() => Instance = this as T;

    // Método chamado quando o MonoBehaviour é destruído
    // Limpa a instância única caso a instância atual seja destruída
    protected virtual void OnDestroy()
    {
        if (_uniqueInstance == this)
            _uniqueInstance = null;
    }

    // Método chamado quando a aplicação é encerrada
    // Marca que a aplicação está saindo para evitar a criação de novas instâncias
    protected virtual void OnApplicationQuit()
    {
#if UNITY_EDITOR
        // No Editor Unity, marca que a aplicação está saindo para impedir instâncias extras
        _isApplicationQuiting = true;
#endif
    }
}
