using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One level on the level select screen: number, saved records, and a lock until it is unlocked.
// Slots past the last built level show as "coming soon".
public class LevelCard : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private TMP_Text numberText;
    [SerializeField]
    private TMP_Text statsText;
    [Tooltip("Shown once the level has been completed.")]
    [SerializeField]
    private GameObject completedMark;
    [Tooltip("Shown instead of the number while the level is locked.")]
    [SerializeField]
    private GameObject lockMark;

    public void Show(int levelNumber)
    {
        string level = GameManager.LevelName(levelNumber);
        bool unlocked = level != null && levelNumber <= SaveData.UnlockedLevel;

        numberText.text = levelNumber.ToString();
        numberText.gameObject.SetActive(unlocked);
        button.interactable = unlocked;
        lockMark.SetActive(!unlocked);
        completedMark.SetActive(unlocked && SaveData.Completed(level));

        if (level == null) statsText.text = "COMING SOON";
        else if (!unlocked) statsText.text = "LOCKED";
        else
        {
            int best = Mathf.FloorToInt(SaveData.BestProgress(level) * 100f);
            statsText.text = $"BEST {best}%\n{SaveData.Attempts(level)} ATTEMPTS";
        }

        button.onClick.RemoveAllListeners();
        if (unlocked) button.OnClick(() => GameManager.Instance.LoadLevel(levelNumber));
    }
}
