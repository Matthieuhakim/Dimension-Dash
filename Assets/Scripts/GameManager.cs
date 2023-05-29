using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private PlayerController player;

    private LevelCompleteUI levelCompleteUI;

    [HideInInspector]
    public bool isPlaying = true;

    private int attempt = 0;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            SceneManager.sceneLoaded += OnLevelStart;

        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
        }
    }



    private void OnLevelStart(Scene scene, LoadSceneMode mode)
    {

        player = GameObject.FindGameObjectWithTag(StringHelper.PLAYER_TAG).GetComponent<PlayerController>();
        levelCompleteUI = GameObject.FindGameObjectWithTag(StringHelper.LEVEL_COMPLETE_UI_TAG).GetComponent<LevelCompleteUI>();

        isPlaying = true;

        IncreaseAttempt();
    }

    public void GameOver()
    {
        isPlaying = false;
        player.Explode();
        StartCoroutine(WaitAndRestartLevel());
    }

    public void FinishLevel()
    {
        isPlaying = false;
        player.Explode();
        levelCompleteUI.ActivateScreen();
    }

    private IEnumerator WaitAndRestartLevel()
    {
        yield return new WaitForSeconds(1);
        RestartCurrentLevel();
    }

    public void IncreaseAttempt()
    {
        attempt += 1;
    }

    public int GetAttempt()
    {
        return attempt;
    }



    private void RestartCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetLevel()
    {

        attempt = 0;
        RestartCurrentLevel();

    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLevelStart;
    }
}
