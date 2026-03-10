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
    public GameObject mainMenu;
    public GameObject levelSelect;
    public GameObject gamePlay;
    public GameObject nextLevel;
    public GameObject gameOver;

    [Header("Buttons")]
    public Button startButton;
    public Button nextLevelButton;
    public Button restartButton;
    public Button levelsButton;

    [Header("Level")]
    public Transform levelContentTransform;
    public GameObject levelButtonPrefab;

    private void Awake()
    {
        Instance = this;
        UpdateScore(SaveSystem.GetSavedScore());
        ActivateUI(mainMenu);
        RegisterButtons();
    }

    void RegisterButtons()
    {
        startButton.onClick.AddListener(OnStartPressed);
        nextLevelButton.onClick.AddListener(OnNextLevelPressed);
        restartButton.onClick.AddListener(OnRestartPressed);
        levelsButton.onClick.AddListener(OnLevelsSelectPressed);
    }

    void OnStartPressed()
    {
        ActivateUI(levelSelect);
        LoadLevelButtons();
    }

    void OnNextLevelPressed()
    {
        ActivateUI(gamePlay);
        GameManager.Instance.NextLevel();
    }

    void OnRestartPressed()
    {
        ActivateUI(gamePlay);
        GameManager.Instance.RestartGame();
    }

    public void OnLevelsSelectPressed()
    {
        ActivateUI(levelSelect);
        LoadLevelButtons();
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
        ActivateUI(nextLevel);
    }

    public void ShowGameOver()
    {
        ActivateUI(gameOver);
    }

    public void ActivateUI(GameObject target)
    {
        mainMenu.SetActive(target == mainMenu);
        levelSelect.SetActive(target == levelSelect);
        gamePlay.SetActive(target == gamePlay);
        nextLevel.SetActive(target == nextLevel);
        gameOver.SetActive(target == gameOver);
    }

    public void LoadLevelButtons()
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

    public void ApplicationQuit()
    {
        Application.Quit();
    }
}