using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Pause button and menu (resume, main menu, audio toggles). Also pauses on Escape / Start / Android Back
// and when the app goes to the background. Resuming counts down 3-2-1 before the run continues.
public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Button pauseButton;
    [Tooltip("Root of the menu; hidden while playing.")]
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Button menuButton;
    [SerializeField]
    private TMP_Text countdownText;
    [SerializeField]
    private int countdownSeconds = 3;
    [Tooltip("Seconds each countdown number stays on screen.")]
    [SerializeField]
    private float countdownStep = 0.6f;

    [Tooltip("Android Back arrives as Escape.")]
    [SerializeField]
    private InputAction pauseAction = CreatePauseAction();

    private Coroutine countdown;

    private static LevelManager Level => LevelManager.Instance;

    private void Awake()
    {
        panel.SetActive(false);
        countdownText.gameObject.SetActive(false);

        pauseButton.OnClick(Pause);
        resumeButton.OnClick(Resume);
        menuButton.OnClick(() => GameManager.Instance.LoadMenu());
    }

    private void OnEnable() => pauseAction.Enable();

    private void OnDisable() => pauseAction.Disable();

    private void Update()
    {
        // Only while the run is going: no pausing a death or the complete screen.
        pauseButton.gameObject.SetActive(Level.CurrentState is LevelManager.State.Playing or LevelManager.State.Paused);

        if (!pauseAction.WasPressedThisFrame()) return;

        if (Level.IsPlaying || countdown != null) Pause();
        else if (panel.activeSelf) Resume();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) Pause();
    }

    private void OnApplicationFocus(bool focused)
    {
        // Not in the Editor: clicking another window while testing shouldn't pause.
        if (!focused && !Application.isEditor) Pause();
    }

    private void Pause()
    {
        if (countdown != null)
        {
            // Pausing again during the countdown.
            StopCoroutine(countdown);
            countdown = null;
            countdownText.gameObject.SetActive(false);
            panel.SetActive(true);
            return;
        }

        if (Level == null || !Level.IsPlaying) return;

        Level.Pause();
        panel.SetActive(true);
    }

    private void Resume()
    {
        if (countdown != null) return;

        panel.SetActive(false);
        countdown = StartCoroutine(CountdownThenResume());
    }

    private IEnumerator CountdownThenResume()
    {
        countdownText.gameObject.SetActive(true);
        for (int i = countdownSeconds; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(countdownStep);
        }
        countdownText.gameObject.SetActive(false);
        countdown = null;
        Level.Resume();
    }

    private static InputAction CreatePauseAction()
    {
        var action = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
        action.AddBinding("<Gamepad>/start");
        return action;
    }
}
