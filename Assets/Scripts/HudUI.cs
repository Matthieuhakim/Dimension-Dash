using TMPro;
using UnityEngine;
using UnityEngine.UI;

// In-game HUD: attempt number, level progress and a "new best" label on a record-breaking death. Display only.
public class HudUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text attemptText;
    [SerializeField]
    private Image progressFill;
    [Tooltip("Hidden until a death beats the level's best progress.")]
    [SerializeField]
    private TMP_Text newBestText;

    private static LevelManager Level => LevelManager.Instance;

    private void Start()
    {
        attemptText.text = $"ATTEMPT {GameManager.Instance.Attempt}";
        newBestText.gameObject.SetActive(false);
    }

    private void Update()
    {
        progressFill.fillAmount = Level.Progress;

        if (Level.IsNewBest && !newBestText.gameObject.activeSelf)
        {
            newBestText.text = $"NEW BEST! {Mathf.FloorToInt(Level.Progress * 100f)}%";
            newBestText.gameObject.SetActive(true);
        }
    }
}
