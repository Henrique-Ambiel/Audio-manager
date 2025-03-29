using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;


public class Events
{
    [System.Serializable]
    public class LoadLevelComplete : UnityEvent<string> { }
}


public class GameManager : IPresistentSingleton<GameManager>
{
    public AudioManager audioManager = new AudioManager();

    private string currentLevel;
    private Events.LoadLevelComplete onLoadLevrlComplete = new Events.LoadLevelComplete();

    public string CurrentLevel { get => currentLevel; }

    private void Start()
    {
        currentLevel = SceneManager.GetActiveScene().name;
    }

    public void RegisterLevelLoadEvent(UnityAction<string> callback)
    {
        onLoadLevrlComplete.AddListener(callback);  
    }

    public void UnregisterLevelLoadEvent(UnityAction<string> callback)
    {
        onLoadLevrlComplete.RemoveListener(callback);
    }

    public void ClearLevelLoadEvents()
    {
        onLoadLevrlComplete.RemoveAllListeners();
    }

    public void LoadLevel(string levelName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        if(asyncLoad == null)
        {
#if UNITY_EDITOR
            Debug.LogError("GameManager: LoadLevel - Falha ao carregar a cena " + levelName);
#endif
            return;
        }
        asyncLoad.completed += (AsyncOperation) =>
        {
            onLoadLevrlComplete.Invoke(levelName);
        };


        currentLevel = levelName;
    }
}
