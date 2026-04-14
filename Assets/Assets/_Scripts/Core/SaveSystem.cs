using UnityEngine;

public static class SaveSystem
{
    const string ScoreKey = "Score"; // Kept for legacy/total score if needed, but we will use GetTotalScore
    const string LevelKey = "UnlockedLevel";
    const string MusicVolumeKey = "MusicVolume";
    const string SFXVolumeKey = "SFXVolume";
    const string HighScoreLevelKey = "HighScoreLevel";

    // Default volume values
    const float DefaultMusicVolume = 0.5f;
    const float DefaultSFXVolume = 1f;

    #region Score

    public static void SaveScore(int score)
    {
        PlayerPrefs.SetInt(ScoreKey, score);
        PlayerPrefs.Save();
    }

    public static int GetSavedScore()
    {
        return PlayerPrefs.GetInt(ScoreKey, 0);
    }

    // --- NEW: Per-Level High Score Methods ---
    public static void SaveLevelHighScore(int level, int score)
    {
        int currentHighScore = GetLevelHighScore(level);
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(HighScoreLevelKey + level, score);
            PlayerPrefs.Save();
        }
    }

    public static int GetLevelHighScore(int level)
    {
        return PlayerPrefs.GetInt(HighScoreLevelKey + level, 0);
    }

    // --- NEW: Sum of all level high scores ---
    public static int GetTotalScore(int totalLevels)
    {
        int total = 0;
        for (int i = 0; i < totalLevels; i++)
        {
            total += GetLevelHighScore(i);
        }
        return total;
    }

    #endregion

    #region Levels

    public static int GetUnlockedLevelIndex()
    {
        return PlayerPrefs.GetInt(LevelKey, 0);
    }

    public static void UnlockNextLevel(int level)
    {
        int highest = GetUnlockedLevelIndex();

        if (level + 1 > highest)
        {
            PlayerPrefs.SetInt(LevelKey, level + 1);
            PlayerPrefs.Save();
        }
    }

    public static void SaveLevelTurn(int level, int turns)
    {
        // Optional: You could also add logic here to only save if turns are lower (best turns)
        PlayerPrefs.SetInt("Turn" + level, turns);
    }

    public static int GetSavedLevelTurn(int level)
    {
        return PlayerPrefs.GetInt("Turn" + level, 0);
    }

    public static void SaveLevelCombo(int level, int combo)
    {
        // Optional: You could also add logic here to only save if combo is higher (best combo)
        PlayerPrefs.SetInt("Combo" + level, combo);
    }

    public static int GetSavedLevelCombo(int level)
    {
        return PlayerPrefs.GetInt("Combo" + level, 0);
    }

    #endregion

    #region Audio Settings

    public static void SaveMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public static float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
    }

    public static void SaveSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public static float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFXVolumeKey, DefaultSFXVolume);
    }

    #endregion

    #region Reset

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }

    #endregion
}