using TMPro;
using UnityEngine;
using UnityEngine.UI;

// In-game HUD: attempt number and level progress (player x between its start and the finish line). Display only.
public class HudUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text attemptText;
    [SerializeField]
    private Image progressFill;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform finishLine;

    private float startX;

    private void Start()
    {
        attemptText.text = $"ATTEMPT {GameManager.Instance.Attempt}";
        startX = player.position.x;
    }

    private void Update()
    {
        progressFill.fillAmount = Mathf.InverseLerp(startX, finishLine.position.x, player.position.x);
    }
}
