using UnityEngine;

// Everything saved between sessions (PlayerPrefs): settings, unlocked levels and per-level records.
// Levels are keyed by scene name; level numbers are build indices (see GameManager).
public static class SaveData
{
    public enum Setting { Sfx, Music, Haptics }

    private const string UNLOCKED_KEY = "UnlockedLevel";

    public static bool Get(Setting setting) => PlayerPrefs.GetInt(SettingKey(setting), 1) == 1;

    public static void Set(Setting setting, bool on) => SetInt(SettingKey(setting), on ? 1 : 0);

    public static bool SfxOn => Get(Setting.Sfx);
    public static bool MusicOn => Get(Setting.Music);
    public static bool HapticsOn => Get(Setting.Haptics);

    // Highest level number that can be played.
    public static int UnlockedLevel => Mathf.Max(1, PlayerPrefs.GetInt(UNLOCKED_KEY, 1));

    public static void Unlock(int levelNumber)
    {
        if (levelNumber > UnlockedLevel) SetInt(UNLOCKED_KEY, levelNumber);
    }

    // Best progress through the level, 0–1.
    public static float BestProgress(string level) => PlayerPrefs.GetFloat(level + ".Best", 0f);

    // Returns true when progress beats the saved best.
    public static bool RecordProgress(string level, float progress)
    {
        if (progress <= BestProgress(level)) return false;

        PlayerPrefs.SetFloat(level + ".Best", progress);
        PlayerPrefs.Save();
        return true;
    }

    public static int Attempts(string level) => PlayerPrefs.GetInt(level + ".Attempts", 0);

    public static void AddAttempt(string level) => SetInt(level + ".Attempts", Attempts(level) + 1);

    public static bool Completed(string level) => PlayerPrefs.GetInt(level + ".Completed", 0) == 1;

    public static void MarkCompleted(string level) => SetInt(level + ".Completed", 1);

    // Clears unlocks and every level's records; settings are kept.
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(UNLOCKED_KEY);
        for (int i = 1; i <= GameManager.LevelCount; i++)
        {
            string level = GameManager.LevelName(i);
            PlayerPrefs.DeleteKey(level + ".Best");
            PlayerPrefs.DeleteKey(level + ".Attempts");
            PlayerPrefs.DeleteKey(level + ".Completed");
        }
        PlayerPrefs.Save();
    }

    private static string SettingKey(Setting setting) => "Setting." + setting;

    private static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }
}
