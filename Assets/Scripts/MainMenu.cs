using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Main menu: home (play, settings), level select and settings panels (with a confirmed progress reset). Lives in the MainMenu scene (build index 0).
public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField]
    private GameObject homePanel;
    [SerializeField]
    private GameObject levelSelectPanel;
    [SerializeField]
    private GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button[] backButtons;

    [Header("Reset progress")]
    [SerializeField]
    private Button resetButton;
    [Tooltip("Warning dialog shown before progress is erased.")]
    [SerializeField]
    private GameObject resetConfirmPanel;
    [SerializeField]
    private Button confirmResetButton;
    [SerializeField]
    private Button cancelResetButton;

    [Header("Level select")]
    [Tooltip("Duplicated once per slot; its parent lays the cards out.")]
    [SerializeField]
    private LevelCard cardTemplate;
    [Tooltip("Cards shown, including locked 'coming soon' slots past the last built level.")]
    [SerializeField]
    private int levelSlots = 5;

    [Tooltip("The camera drifts right so the background's parallax layers move.")]
    [SerializeField]
    private float cameraDrift = 1.5f;

    private Transform cam;
    private readonly List<LevelCard> cards = new List<LevelCard>();

    private void Awake()
    {
        playButton.OnClick(() => ShowPanel(levelSelectPanel));
        settingsButton.OnClick(() => ShowPanel(settingsPanel));
        foreach (Button back in backButtons)
        {
            back.OnClick(() => ShowPanel(homePanel));
        }

        resetButton.OnClick(() => resetConfirmPanel.SetActive(true));
        cancelResetButton.OnClick(() => resetConfirmPanel.SetActive(false));
        confirmResetButton.OnClick(() =>
        {
            SaveData.ResetProgress();
            RefreshLevelCards();
            resetConfirmPanel.SetActive(false);
        });

        BuildLevelCards();
        ShowPanel(homePanel);
        cam = Camera.main.transform;
    }

    private void Update()
    {
        cam.position += Vector3.right * (cameraDrift * Time.deltaTime);
    }

    private void BuildLevelCards()
    {
        int slots = Mathf.Max(levelSlots, GameManager.LevelCount);
        for (int i = 1; i <= slots; i++)
        {
            LevelCard card = i == 1 ? cardTemplate : Instantiate(cardTemplate, cardTemplate.transform.parent);
            card.name = $"Level {i}";
            cards.Add(card);
        }
        RefreshLevelCards();
    }

    private void RefreshLevelCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].Show(i + 1);
        }
    }

    private void ShowPanel(GameObject panel)
    {
        homePanel.SetActive(panel == homePanel);
        levelSelectPanel.SetActive(panel == levelSelectPanel);
        settingsPanel.SetActive(panel == settingsPanel);
        resetConfirmPanel.SetActive(false);
    }
}
