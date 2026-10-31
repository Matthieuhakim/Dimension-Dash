using System;
using System.Collections;
using UnityEngine;

// Per-scene level flow: current dimension, play state (incl. pause), progress, death and completion.
// Runs before other scripts so Instance exists when listeners enable.
[DefaultExecutionOrder(-100)]
public class LevelManager : MonoBehaviour
{
    public enum State { Playing, Paused, Dead, Complete }

    public static LevelManager Instance { get; private set; }

    public static event Action<Dimension> DimensionChanged;

    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private LevelCompleteUI levelCompleteUI;
    [SerializeField]
    private CameraController cameraRig;
    [SerializeField]
    private Transform finishLine;

    [SerializeField]
    private Dimension startDimension = Dimension.Reality;
    [SerializeField]
    private float restartDelay = 1f;
    [Tooltip("Pause between the player bursting at the finish and the level complete screen.")]
    [SerializeField]
    private float completeScreenDelay = 0.8f;
    [Tooltip("The player dies when falling below this height.")]
    [SerializeField]
    private float killHeight = -10f;

    public Dimension CurrentDimension { get; private set; }
    public State CurrentState { get; private set; } = State.Playing;
    public bool IsPlaying => CurrentState == State.Playing;
    public float KillHeight => killHeight;
    // Player x between its start and the finish line, 0–1.
    public float Progress => Mathf.InverseLerp(startX, finishLine.position.x, player.transform.position.x);
    // True after a death that beat the level's best progress.
    public bool IsNewBest { get; private set; }

    private string LevelName => gameObject.scene.name;

    private float startX;

    private void Awake()
    {
        Instance = this;
        CurrentDimension = startDimension;
        startX = player.transform.position.x;
        GameManager.Instance.BeginAttempt(LevelName);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Pause()
    {
        if (!IsPlaying) return;

        CurrentState = State.Paused;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (CurrentState != State.Paused) return;

        CurrentState = State.Playing;
        Time.timeScale = 1f;
    }

    public void SwitchDimension()
    {
        if (!IsPlaying) return;

        CurrentDimension = CurrentDimension == Dimension.Reality ? Dimension.Shadow : Dimension.Reality;
        DimensionChanged?.Invoke(CurrentDimension);
    }

    public void KillPlayer()
    {
        if (!IsPlaying) return;

        CurrentState = State.Dead;
        IsNewBest = SaveData.RecordProgress(LevelName, Progress);
        player.Explode();
        cameraRig.Shake(0.35f, 0.35f);
        SoundManager.Play(SoundManager.Sfx.Death);
        Vibrate();
        StartCoroutine(RestartAfterDelay());
    }

    public void CompleteLevel()
    {
        if (!IsPlaying) return;

        CurrentState = State.Complete;
        SaveData.RecordProgress(LevelName, 1f);
        SaveData.MarkCompleted(LevelName);
        SaveData.Unlock(GameManager.CurrentLevelNumber + 1);
        player.Explode();
        cameraRig.Shake(0.25f, 0.2f);
        SoundManager.Play(SoundManager.Sfx.Finish);
        Vibrate();
        StartCoroutine(ShowCompleteScreenAfterDelay());
    }

    private IEnumerator ShowCompleteScreenAfterDelay()
    {
        yield return new WaitForSeconds(completeScreenDelay);
        levelCompleteUI.Show(GameManager.Instance.Attempt);
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        GameManager.Instance.RestartLevel();
    }

    private static void Vibrate()
    {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (SaveData.HapticsOn) Handheld.Vibrate();
#endif
    }
}
