using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text attemptText;

    [SerializeField]
    private Button restartButton;

    private void SetChildrenState(bool state)
    {
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(state);
        }
    }

    public void ActivateScreen()
    {
        SetChildrenState(true);
        attemptText.text = "Total attempts: " + GameManager.Instance.GetAttempt();
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(GameManager.Instance.ResetLevel);
    }
}
