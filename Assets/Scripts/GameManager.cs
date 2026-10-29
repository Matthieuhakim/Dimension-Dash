using UnityEngine;
using UnityEngine.SceneManagement;

// Persistent, self-bootstrapping singleton: only state that outlives a scene (attempts) and scene loading.
// Levels don't need to contain it.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Attempt { get; private set; }

    private string currentLevel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;
        new GameObject(nameof(GameManager)).AddComponent<GameManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Mobile defaults to 30 fps; run at the display's refresh rate instead.
        Application.targetFrameRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
    }

    // Called by each level on load. Attempts reset when a different level is started.
    public void BeginAttempt(string levelName)
    {
        if (levelName != currentLevel)
        {
            currentLevel = levelName;
            Attempt = 0;
        }

        Attempt++;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Restart from scratch, resetting the attempt counter.
    public void ReplayLevel()
    {
        currentLevel = null;
        RestartLevel();
    }
}
