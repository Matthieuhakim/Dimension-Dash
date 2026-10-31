using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A button that flips one saved setting and shows its state ("MUSIC: ON").
[RequireComponent(typeof(Button))]
public class SettingToggle : MonoBehaviour
{
    [SerializeField]
    private SaveData.Setting setting;
    [SerializeField]
    private string label = "SFX";
    [SerializeField]
    private TMP_Text text;

    private void Awake()
    {
        GetComponent<Button>().OnClick(() =>
        {
            SaveData.Set(setting, !SaveData.Get(setting));
            SoundManager.ApplySettings();
            Refresh();
        });
    }

    // Another toggle (pause menu vs. menu) may have changed the setting meanwhile.
    private void OnEnable() => Refresh();

    private void Refresh()
    {
        text.text = $"{label}: {(SaveData.Get(setting) ? "ON" : "OFF")}";
    }
}
