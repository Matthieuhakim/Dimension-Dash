using UnityEngine;
using UnityEngine.SceneManagement;

// Persistent, self-bootstrapping singleton: only state that outlives a scene (attempts) and scene loading.
// Levels don't need to contain it. Build index 0 is the main menu; build index N (N >= 1) is level N.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const int MENU_SCENE = 0;

    public int Attempt { get; private set; }

    public static int LevelCount => SceneManager.sceneCountInBuildSettings - 1;
    public static int CurrentLevelNumber => SceneManager.GetActiveScene().buildIndex;
    public static bool HasNextLevel => CurrentLevelNumber >= 1 && CurrentLevelNumber < LevelCount;

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
        SaveData.AddAttempt(levelName);
    }

    // Scene name of level N, the key for its saved records. Null if the level doesn't exist.
    public static string LevelName(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > LevelCount) return null;
        return System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(levelNumber));
    }

    // Leaving to the menu ends the session: the next run of any level starts at attempt 1.
    public void LoadMenu()
    {
        currentLevel = null;
        Load(MENU_SCENE);
    }

    public void LoadLevel(int levelNumber) => Load(levelNumber);

    public void LoadNextLevel() => Load(CurrentLevelNumber + 1);

    public void RestartLevel() => Load(CurrentLevelNumber);

    // Restart from scratch, resetting the attempt counter.
    public void ReplayLevel()
    {
        currentLevel = null;
        RestartLevel();
    }

    private static void Load(int buildIndex)
    {
        // A level left from the pause menu would otherwise stay frozen.
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);
    }
}
