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

    private void Awake()
    {
        panel.SetActive(false);
        restartButton.onClick.AddListener(() => GameManager.Instance.ReplayLevel());
    }

    public void Show(int attempts)
    {
        attemptText.text = $"Total attempts: {attempts}";
        panel.SetActive(true);
    }
}
