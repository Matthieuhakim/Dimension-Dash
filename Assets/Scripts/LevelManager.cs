using System;
using System.Collections;
using UnityEngine;

// Per-scene level flow: current dimension, play state, death and completion.
// Runs before other scripts so Instance exists when listeners enable.
[DefaultExecutionOrder(-100)]
public class LevelManager : MonoBehaviour
{
    public enum State { Playing, Dead, Complete }

    public static LevelManager Instance { get; private set; }

    public static event Action<Dimension> DimensionChanged;

    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private LevelCompleteUI levelCompleteUI;

    [SerializeField]
    private Dimension startDimension = Dimension.Reality;
    [SerializeField]
    private float restartDelay = 1f;
    [Tooltip("The player dies when falling below this height.")]
    [SerializeField]
    private float killHeight = -10f;

    public Dimension CurrentDimension { get; private set; }
    public State CurrentState { get; private set; } = State.Playing;
    public bool IsPlaying => CurrentState == State.Playing;
    public float KillHeight => killHeight;

    private void Awake()
    {
        Instance = this;
        CurrentDimension = startDimension;
        GameManager.Instance.BeginAttempt(gameObject.scene.name);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
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
        player.Explode();
        StartCoroutine(RestartAfterDelay());
    }

    public void CompleteLevel()
    {
        if (!IsPlaying) return;

        CurrentState = State.Complete;
        player.Stop();
        levelCompleteUI.Show(GameManager.Instance.Attempt);
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        GameManager.Instance.RestartLevel();
    }
}
