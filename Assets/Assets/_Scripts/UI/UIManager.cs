using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Texts")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text matchText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text comboTimerText;
    [SerializeField] private TMP_Text turnText;

    [Header("UI Panels (CanvasGroups)")]
    [SerializeField] private CanvasGroup mainMenuCanvas;
    [SerializeField] private CanvasGroup levelSelectCanvas;
    [SerializeField] private CanvasGroup gamePlayCanvas;
    [SerializeField] private CanvasGroup nextLevelCanvas;
    [SerializeField] private CanvasGroup gameOverCanvas;

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

    private void Awake()
    {
        Instance = this;

        UpdateScore(SaveSystem.GetSavedScore());
        RegisterButtons();

        // Initialize UI state
        SwitchScreen(mainMenuCanvas); // Show main menu 
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
        SwitchScreen(levelSelectCanvas);
        LoadLevelButtons();
    }

    private void OnNextLevelPressed()
    {
        SwitchScreen(gamePlayCanvas);
        GameManager.Instance.NextLevel();
    }

    private void OnRestartPressed()
    {
        SwitchScreen(gamePlayCanvas);
        GameManager.Instance.RestartGame();
    }

    public void OnLevelsSelectPressed()
    {
        SwitchScreen(levelSelectCanvas);
        LoadLevelButtons();
    }
    #endregion

    #region UI Updates
    public void UpdateScore(int score) => scoreText.text = $"Score: {score}";
    public void UpdateMatches(int matched, int total) => matchText.text = $"Matched: {matched}/{total}";
    public void UpdateTurns(int turns) => turnText.text = $"Turn: {turns}";
    public void UpdateCombo(int combo) => comboText.text = $"Combos: {combo}";
    public void UpdateComboTimer(float time) => comboTimerText.text = "Combo Timer: " + Mathf.CeilToInt(time);
    public void ResetComboTimer() => comboTimerText.text = "Combo Timer: 0";

    public void ShowNextLevelUI() => SwitchScreen(nextLevelCanvas);
    public void ShowGameOver() => SwitchScreen(gameOverCanvas);

    // Helper to be called by LevelButton
    public void ShowGameplay() => SwitchScreen(gamePlayCanvas);
    #endregion

    #region Fade Logic
    public void SwitchScreen(CanvasGroup targetCanvas)
    {
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
            }
        }
    }
    #endregion

    public void ApplicationQuit() => Application.Quit();
}