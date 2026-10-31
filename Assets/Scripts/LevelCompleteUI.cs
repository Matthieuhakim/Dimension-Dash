using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    [Tooltip("Root of the screen; hidden until the level is completed.")]
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private TMP_Text attemptText;
    [SerializeField]
    private Button restartButton;
    [Tooltip("Hidden when this is the last level.")]
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button menuButton;

    private void Awake()
    {
        panel.SetActive(false);
        restartButton.OnClick(() => GameManager.Instance.ReplayLevel());
        nextButton.OnClick(() => GameManager.Instance.LoadNextLevel());
        menuButton.OnClick(() => GameManager.Instance.LoadMenu());
    }

    public void Show(int attempts)
    {
        attemptText.text = $"Total attempts: {attempts}";
        nextButton.gameObject.SetActive(GameManager.HasNextLevel);
        panel.SetActive(true);
    }
}
