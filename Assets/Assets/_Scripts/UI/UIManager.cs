using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Score Texts")]
    [SerializeField] private TMP_Text scoreTextGameplay;
    [SerializeField] private TMP_Text scoreTextLevelSelect;

    [Header("Texts")]
    [SerializeField] private TMP_Text matchText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text comboTimerText;
    [SerializeField] private TMP_Text turnText;

    [Header("UI Panels (CanvasGroups)")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup levelSelect;
    [SerializeField] private CanvasGroup gamePlay;
    [SerializeField] private CanvasGroup nextLevel;
    [SerializeField] private CanvasGroup gameOver;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button levelsButton;

    [Header("Level")]
    [SerializeField] private Transform levelContentTransform;
    [SerializeField] private GameObject levelButtonPrefab;

    // Helper list to manage all canvases easily
    public List<CanvasGroup> allPages;

    // Track the current screen for music ducking logic
    private CanvasGroup currentScreen;

    private void Awake()
    {
        Instance = this;

        UpdateScore(SaveSystem.GetTotalScore(6), false);
        RegisterButtons();

        // Initialize UI state
        SwitchScreen(mainMenu); // Show main menu
    }

    private void RegisterButtons()
    {
        startButton.onClick.AddListener(OnStartPressed);
        nextLevelButton.onClick.AddListener(OnNextLevelPressed);
        restartButton.onClick.AddListener(OnRestartPressed);
        levelsButton.onClick.AddListener(OnLevelsSelectPressed);
    }

    #region Button Handlers
    private void OnStartPressed()
    {
        SwitchScreen(levelSelect);
        LoadLevelButtons();
    }

    private void OnNextLevelPressed()
    {
        SwitchScreen(gamePlay);
        GameManager.Instance.NextLevel();
    }

    private void OnRestartPressed()
    {
        SwitchScreen(gamePlay);
        GameManager.Instance.RestartGame();
    }

    public void OnLevelsSelectPressed()
    {
        SwitchScreen(levelSelect);

        // Refresh total score when returning to level select
        UpdateScore(SaveSystem.GetTotalScore(GameManager.Instance.levelsData.levels.Count), false);
        LoadLevelButtons();
    }
    #endregion

    #region UI Updates

    public void ResetLevels()
    {
        SaveSystem.ResetProgress();
        UpdateScore(0, false);
    }

    /// <summary>
    /// Updates score on Gameplay and/or LevelSelect pages.
    /// isGameplayScore = true  -> Updates the current level score during gameplay
    /// isGameplayScore = false -> Updates the total accumulated score on Level Select
    /// </summary>
    public void UpdateScore(int score, bool isGameplayScore)
    {
        if (isGameplayScore)
        {
            if (scoreTextGameplay != null)
                scoreTextGameplay.text = $"Score: {score}";
        }
        else
        {
            if (scoreTextLevelSelect != null)
                scoreTextLevelSelect.text = $"Total Score: {score}";
        }
    }

    public void UpdateMatches(int matched, int total) => matchText.text = $"Matched: {matched}/{total}";
    public void UpdateTurns(int turns) => turnText.text = $"Turn: {turns}";
    public void UpdateCombo(int combo) => comboText.text = $"Combos: {combo}";
    public void UpdateComboTimer(float time) => comboTimerText.text = "Combo Timer: " + Mathf.CeilToInt(time);
    public void ResetComboTimer() => comboTimerText.text = "Combo Timer: 0";

    public void ShowNextLevelUI() => SwitchScreen(nextLevel);
    public void ShowGameOver() => SwitchScreen(gameOver);

    // Helper to be called by LevelButton
    public void ShowGameplay() => SwitchScreen(gamePlay);
    #endregion

    #region Fade Logic
    public void SwitchScreen(CanvasGroup targetCanvas)
    {
        currentScreen = targetCanvas;

        // Handle background music ducking based on which screen we're switching to
        HandleMusicDucking(targetCanvas);

        foreach (var canvas in allPages)
        {
            if (canvas == targetCanvas)
            {
                Fade(canvas, true); // Fade In
            }
            else
            {
                Fade(canvas, false); // Fade Out
            }
        }
    }

    public void Fade(CanvasGroup canvasGroup, bool visible, UnityAction callback = null)
    {
        // Immediately disable interaction to prevent double clicks
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        float targetAlpha = visible ? 1 : 0;

        canvasGroup.DOFade(targetAlpha, 0.3f).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                if (visible)
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                }
                callback?.Invoke();
            });
    }

    /// <summary>
    /// Manages background music volume based on the active screen.
    /// </summary>
    private void HandleMusicDucking(CanvasGroup targetCanvas)
    {
        if (AudioManager.Instance == null) return;

        if (targetCanvas == gamePlay)
        {
            AudioManager.Instance.DuckMusicForGameplay();
        }
        else if (targetCanvas == mainMenu || targetCanvas == levelSelect ||
                 targetCanvas == nextLevel || targetCanvas == gameOver)
        {
            AudioManager.Instance.RestoreMusicVolume();
        }
    }
    #endregion

    #region Level Buttons
    public void LoadLevelButtons()
    {
        // Clear existing buttons
        foreach (Transform child in levelContentTransform)
            Destroy(child.gameObject);

        for (int i = 0; i < GameManager.Instance.levelsData.levels.Count; i++)
        {
            var levelData = GameManager.Instance.levelsData.levels[i];
            var levelButtonObj = Instantiate(levelButtonPrefab, levelContentTransform);
            var levelButton = levelButtonObj.GetComponent<LevelButton>();

            if (levelButton != null)
            {
                levelButton.SetLevelIndex(i, levelData.levelName);
                levelButton.SetLocked(SaveSystem.GetUnlockedLevelIndex() < i);
                levelButton.turmAmountTXT.text = $"Turn: {SaveSystem.GetSavedLevelTurn(i)}";
                levelButton.comboAmountTXT.text = $"Combo: {SaveSystem.GetSavedLevelCombo(i)}";

                // Show the saved High Score for this specific level
                if (levelButton.scoreAmountTXT != null)
                    levelButton.scoreAmountTXT.text = $"Score: {SaveSystem.GetLevelHighScore(i)}";
            }
        }
    }
    #endregion

    public void ApplicationQuit() => Application.Quit();
}