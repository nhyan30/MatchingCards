using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Texts")]
    public TMP_Text scoreText;
    public TMP_Text matchText;
    public TMP_Text comboText;
    public TMP_Text comboTimerText;
    public TMP_Text turnText;

    [Header("UI Panels")]
    public GameObject startGameUI;
    public GameObject levelLoadUI;
    public GameObject gameUI;
    public GameObject nextLevelUI;
    public GameObject gameOverUI;

    [Header("Buttons")]
    public Button startButton;
    public Button nextLevelButton;
    public Button restartButton;
    public Button levelsButton;

    public Transform levelContentTransform;
    public GameObject levelButtonPrefab;

    private void Awake()
    {
        Instance = this;

        UpdateScore(SaveSystem.GetSavedScore());

        ActivateUI(startGameUI);

        RegisterButtons();
    }

    void RegisterButtons()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelPressed);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartPressed);

        if (levelsButton != null)
            levelsButton.onClick.AddListener(OnLevelsSelectPressed);
    }

    void OnStartPressed()
    {
        ActivateUI(levelLoadUI);
        LoadLevelButttons();
    }

    void OnNextLevelPressed()
    {
        ActivateUI(gameUI);
        GameManager.Instance.NextLevel();
    }

    void OnRestartPressed()
    {
        ActivateUI(gameUI);
        GameManager.Instance.RestartGame();
    }

    public void OnLevelsSelectPressed()
    {
        ActivateUI(levelLoadUI);
        LoadLevelButttons();
    }

    public void ResetLevels()
    {
        SaveSystem.ResetProgress();
        UpdateScore(SaveSystem.GetSavedScore());
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateMatches(int matched, int total)
    {
        matchText.text = $"Matched: {matched}/{total}";
    }

    public void UpdateTurns(int turns)
    {
        turnText.text = $"Turn: {turns}";
    }

    public void UpdateCombo(int combo)
    {
        comboText.text = $"Combos: {combo}";
    }

    public void UpdateComboTimer(float time)
    {
        comboTimerText.text = "Combo Timer: " + Mathf.CeilToInt(time);
    }

    public void ResetComboTimer()
    {
        comboTimerText.text = "Combo Timer: 0";
    }

    public void ShowNextLevelUI()
    {
        ActivateUI(nextLevelUI);
    }

    public void ShowGameOver()
    {
        ActivateUI(gameOverUI);
    }

    public void ActivateUI(GameObject target)
    {
        startGameUI.SetActive(target == startGameUI);
        levelLoadUI.SetActive(target == levelLoadUI);
        gameUI.SetActive(target == gameUI);
        nextLevelUI.SetActive(target == nextLevelUI);
        gameOverUI.SetActive(target == gameOverUI);
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }

    public void LoadLevelButttons()
    {
        foreach (Transform child in levelContentTransform)
        {
            Destroy(child.gameObject);
        }
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
}