using UnityEngine;

public static class SaveSystem
{
    const string ScoreKey = "Score";
    const string LevelKey = "UnlockedLevel";
    const string MusicVolumeKey = "MusicVolume";
    const string SFXVolumeKey = "SFXVolume";

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
        PlayerPrefs.SetInt("Turn" + level, turns);
    }

    public static int GetSavedLevelTurn(int level)
    {
        return PlayerPrefs.GetInt("Turn" + level, 0);
    }

    public static void SaveLevelCombo(int level, int combo)
    {
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