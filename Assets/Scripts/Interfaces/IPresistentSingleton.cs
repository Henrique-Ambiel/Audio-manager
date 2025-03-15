using UnityEngine;

// A classe IPresistentSingleton<T> implementa o padr�o de design Singleton para garantir que exista
// apenas uma inst�ncia de um objeto T (herdado de MonoBehaviour) durante a execu��o do jogo.
public class IPresistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // A vari�vel est�tica que guarda a �nica inst�ncia do tipo T
    private static T _uniqueInstance = null;

    // Flag usada para impedir a cria��o de novas inst�ncias quando o aplicativo estiver saindo no Editor Unity
#if UNITY_EDITOR
    private static bool _isApplicationQuiting = false;
#endif

    // Propriedade que acessa a inst�ncia �nica da classe
    public static T Instance
    {
        get
        {
            // Se a inst�ncia �nica ainda n�o foi criada e o aplicativo n�o est� saindo, tenta obter ou criar uma nova inst�ncia
            if (_uniqueInstance == null
#if UNITY_EDITOR && !UNITY_WEBGL
                && !_isApplicationQuiting
#endif
                )
            {
                // Busca a inst�ncia na cena (caso j� tenha sido criada)
                _uniqueInstance = FindFirstObjectByType<T>();
                if (_uniqueInstance == null)
                {
                    // Se n�o encontrou, tenta carregar o prefab correspondente ao tipo T
                    GameObject SingletonPrefab = Resources.Load<GameObject>(typeof(T).Name);
                    if (SingletonPrefab)
                    {
                        // Se o prefab foi encontrado, instancia-o e obt�m a inst�ncia de T
                        GameObject SingletonObject = Instantiate<GameObject>(SingletonPrefab);
                        if (SingletonObject != null)
                            _uniqueInstance = SingletonObject.GetComponent<T>();
                    }
                    // Se ainda n�o encontrou a inst�ncia, cria um novo GameObject com o tipo T
                    if (_uniqueInstance == null)
                        _uniqueInstance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return _uniqueInstance;
        }
        private set
        {
            // Se a inst�ncia ainda n�o foi definida, define-a e impede que seja destru�da ao carregar uma nova cena
            if (null == _uniqueInstance)
            {
                _uniqueInstance = value;
                DontDestroyOnLoad(_uniqueInstance.gameObject);
            }
            // Se a inst�ncia j� existe, e n�o � a mesma, emite um erro e destr�i a nova inst�ncia
            else if (_uniqueInstance != value)
            {
#if UNITY_EDITOR && !UNITY_WEBGL
                // Exibe um erro no Editor Unity, alertando sobre a tentativa de criar uma segunda inst�ncia
                Debug.LogError("[" + typeof(T).Name + "] Tentou instanciar uma segunda inst�ncia da classe IPresistentSingleton.");
#endif
                // Destroi o novo GameObject que foi instanciado para garantir que apenas a primeira inst�ncia exista
                DestroyImmediate(value.gameObject);
            }
        }
    }

    // M�todo que verifica se a inst�ncia j� foi criada (se est� inicializada)
    public static bool IsInitialized()
    {
        return _uniqueInstance != null;
    }

    // M�todo chamado quando o script � carregado
    // Define a inst�ncia �nica da classe para o pr�prio objeto (this)
    protected virtual void Awake() => Instance = this as T;

    // M�todo chamado quando o MonoBehaviour � destru�do
    // Limpa a inst�ncia �nica caso a inst�ncia atual seja destru�da
    protected virtual void OnDestroy()
    {
        if (_uniqueInstance == this)
            _uniqueInstance = null;
    }

    // M�todo chamado quando a aplica��o � encerrada
    // Marca que a aplica��o est� saindo para evitar a cria��o de novas inst�ncias
    protected virtual void OnApplicationQuit()
    {
#if UNITY_EDITOR
        // No Editor Unity, marca que a aplica��o est� saindo para impedir inst�ncias extras
        _isApplicationQuiting = true;
#endif
    }
}
