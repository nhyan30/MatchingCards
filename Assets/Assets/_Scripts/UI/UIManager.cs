using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Score Texts")]
    [SerializeField] private TMP_Text scoreTextGameplay;
    [SerializeField] private TMP_Text scoreTextLevelSelect;

    [Header("Level Info")]
    [SerializeField] private TMP_Text levelNameText;

    [Header("Score Effects")]
    [SerializeField] private GameObject scorePopupPrefab;
    [SerializeField] private Transform gameplayCanvas;

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

    public List<CanvasGroup> allPages;

    private CanvasGroup currentScreen;

    private void Awake()
    {
        Instance = this;

        UpdateScore(SaveSystem.GetTotalScore(6), false);
        RegisterButtons();

        SwitchScreenImmediate(mainMenu);
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
        SwitchScreen(levelSelect, () => LoadLevelButtons());
    }

    private void OnNextLevelPressed()
    {
        SwitchScreenImmediate(gamePlay);
        GameManager.Instance.NextLevel();
    }

    private void OnRestartPressed()
    {
        SwitchScreenImmediate(gamePlay);
        GameManager.Instance.RestartGame();
    }

    public void OnBackPressed()
    {
        SwitchScreen(mainMenu);
    }

    public void OnLevelsSelectPressed()
    {
        SwitchScreen(levelSelect, () =>
        {
            UpdateScore(SaveSystem.GetTotalScore(GameManager.Instance.levelsData.levels.Count), false);
            LoadLevelButtons();
        });
    }
    #endregion

    #region UI Updates

    public void ResetLevels()
    {
        SaveSystem.ResetProgress();
        UpdateScore(0, false);
    }

    public void UpdateScore(int score, bool isGameplayScore)
    {
        if (isGameplayScore)
        {
            if (scoreTextGameplay != null)
                scoreTextGameplay.text = $"Score {score}";
        }
        else
        {
            if (scoreTextLevelSelect != null)
                scoreTextLevelSelect.text = $"Total Score {score}";
        }
    }

    public void UpdateLevelName(string levelName)
    {
        if (levelNameText != null)
        {
            levelNameText.text = $"Level {levelName}";
        }
    }

    public void UpdateMatches(int matched, int total) => matchText.text = $"Matched {matched}/{total}";
    public void UpdateTurns(int turns) => turnText.text = $"Turn {turns}";
    public void UpdateCombo(int combo) => comboText.text = $"Combos {combo}";
    public void UpdateComboTimer(float time) => comboTimerText.text = "Combo Timer " + Mathf.CeilToInt(time);
    public void ResetComboTimer() => comboTimerText.text = "Combo Timer 0";

    public void ShowNextLevelUI() => SwitchScreenImmediate(nextLevel);
    public void ShowGameOver() => SwitchScreen(gameOver);

    public void ShowGameplay() => SwitchScreen(gamePlay);

    // Changed Vector2 to Vector3
    public void ShowScorePopup(string text, Vector3 position)
    {
        if (scorePopupPrefab == null || gameplayCanvas == null) return;

        GameObject popupObj = Instantiate(scorePopupPrefab, gameplayCanvas);
        ScorePopup popup = popupObj.GetComponent<ScorePopup>();

        if (popup != null)
        {
            popup.Show(text, position, Color.yellow);
        }
    }

    // Changed Vector2 to Vector3
    public void ShowComboPopupWithDelay(string text, Vector3 position, float delay)
    {
        StartCoroutine(ComboPopupRoutine(text, position, delay));
    }

    // Changed Vector2 to Vector3
    private IEnumerator ComboPopupRoutine(string text, Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (scorePopupPrefab == null || gameplayCanvas == null) yield break;

        GameObject popupObj = Instantiate(scorePopupPrefab, gameplayCanvas);
        ScorePopup popup = popupObj.GetComponent<ScorePopup>();

        if (popup != null)
        {
            // Using yOffset parameter so it plays nicely with UI scaling
            popup.Show(text, position, Color.red, yOffset: 40f);
        }
    }
    #endregion

    #region Fade/Wipe Logic

    public void SwitchScreen(CanvasGroup targetCanvas, System.Action onSwitchContent = null)
    {
        currentScreen = targetCanvas;

        if (WipeController.Instance != null)
        {
            WipeController.Instance.PlayTransition(() =>
            {
                HandleMusicDucking(targetCanvas);
                SetCanvasVisibility(targetCanvas);
                onSwitchContent?.Invoke();
            });
        }
        else
        {
            HandleMusicDucking(targetCanvas);
            SetCanvasVisibility(targetCanvas);
            onSwitchContent?.Invoke();
        }
    }

    private void SwitchScreenImmediate(CanvasGroup targetCanvas)
    {
        currentScreen = targetCanvas;
        HandleMusicDucking(targetCanvas);
        SetCanvasVisibility(targetCanvas);
    }

    private void SetCanvasVisibility(CanvasGroup targetCanvas)
    {
        foreach (var canvas in allPages)
        {
            if (canvas == targetCanvas)
            {
                Fade(canvas, true);
            }
            else
            {
                Fade(canvas, false);
            }
        }
    }

    public void Fade(CanvasGroup canvasGroup, bool visible, UnityAction callback = null)
    {
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
                levelButton.turnAmountTXT.text = $"Turn {SaveSystem.GetSavedLevelTurn(i)}";
                levelButton.comboAmountTXT.text = $"Combo {SaveSystem.GetSavedLevelCombo(i)}";

                if (levelButton.scoreAmountTXT != null)
                    levelButton.scoreAmountTXT.text = $"Score {SaveSystem.GetLevelHighScore(i)}";
            }
        }
    }
    #endregion

    public void ApplicationQuit() => Application.Quit();
}