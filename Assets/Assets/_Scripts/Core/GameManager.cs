using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GridLayoutGroup gridLayout;
    public LevelData levelsData;

    [Header("Timing")]
    [SerializeField] private float cardSpawnDelay = 0.1f;
    [SerializeField] private float previewTime = 1.5f;
    [SerializeField] private float matchCheckDelay = 0.5f;
    [SerializeField] private float comboDuration = 5f;

    private List<Card> cards = new List<Card>();
    private List<Card> selectedCards = new List<Card>();
    private Transform grid;

    private int currentLevelIndex;
    private int totalPairs;
    private int matchedPairs;
    private int turnsTaken;
    private int score;
    private int comboCount;
    private float comboTimer;
    private bool comboActive;
    private bool checkingMatch;

    private void Awake()
    {
        Instance = this;
        grid = gridLayout.transform;
    }

    private void Start()
    {
        // Show total accumulated score on main menu / level select
        int totalScore = SaveSystem.GetTotalScore(levelsData.levels.Count);
        UIManager.Instance.UpdateScore(totalScore, false);
    }

    public void RestartGame()
    {
        ClearBoard();
        StartCoroutine(LoadLevelRoutine(currentLevelIndex));
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelsData.levels.Count)
        {
            Debug.LogError("Invalid level index");
            return;
        }

        currentLevelIndex = levelIndex;
        StartCoroutine(LoadLevelRoutine(levelIndex));
    }

    IEnumerator LoadLevelRoutine(int levelIndex)
    {
        ClearBoard();

        turnsTaken = 0;
        matchedPairs = 0;
        comboCount = 0;
        score = 0; // Reset level score to 0

        var level = levelsData.levels[levelIndex];
        totalPairs = level.cardImages.Count;

        UIManager.Instance.UpdateTurns(turnsTaken);
        UIManager.Instance.UpdateMatches(matchedPairs, totalPairs);
        UIManager.Instance.UpdateCombo(comboCount);
        UIManager.Instance.UpdateScore(score, true); // Show level score (0) on gameplay screen

        // Duck background music during gameplay
        AudioManager.Instance.DuckMusicForGameplay();

        grid.GetComponent<DynamicGridScaler>().UpdateGrid(
            level.rows, level.columns, level.cardSpacing, level.padding);

        List<int> ids = new List<int>();
        for (int i = 0; i < level.cardImages.Count; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        Shuffle(ids);

        foreach (int id in ids)
        {
            GameObject cardObj = Instantiate(cardPrefab, grid);
            Card card = cardObj.GetComponent<Card>();
            card.Init(id, level.cardImages[id]);
            cards.Add(card);
            yield return new WaitForSeconds(cardSpawnDelay);
        }

        yield return new WaitForSeconds(previewTime);

        foreach (Card card in cards)
            card.FlipDown();
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    void ClearBoard()
    {
        foreach (Transform child in grid)
            Destroy(child.gameObject);

        cards.Clear();
        selectedCards.Clear();
    }

    public void SelectCard(Card card)
    {
        if (checkingMatch || selectedCards.Contains(card)) return;

        card.FlipUp();
        AudioManager.Instance.Play(SoundType.Flip);
        selectedCards.Add(card);

        if (selectedCards.Count == 2)
        {
            StartCoroutine(CheckMatchRoutine());
        }
    }

    IEnumerator CheckMatchRoutine()
    {
        checkingMatch = true;
        yield return new WaitForSeconds(matchCheckDelay);

        Card first = selectedCards[0];
        Card second = selectedCards[1];

        turnsTaken++;
        UIManager.Instance.UpdateTurns(turnsTaken);

        if (first.cardId == second.cardId)
            HandleMatch(first, second);
        else
            HandleMismatch(first, second);

        selectedCards.Clear();
        checkingMatch = false;
    }

    void HandleMatch(Card a, Card b)
    {
        AudioManager.Instance.Play(SoundType.Match);
        a.Hide();
        b.Hide();
        matchedPairs++;
        UpdateScore(true);
        UIManager.Instance.UpdateMatches(matchedPairs, totalPairs);

        if (matchedPairs >= totalPairs)
            StartCoroutine(LevelCompleteRoutine());
    }

    void HandleMismatch(Card a, Card b)
    {
        AudioManager.Instance.Play(SoundType.Fail);
        a.FlipDown();
        b.FlipDown();
        comboActive = false;
        UIManager.Instance.ResetComboTimer();
    }

    void UpdateScore(bool match)
    {
        if (match)
        {
            if (comboActive)
            {
                comboCount++;
                score += 50;
            }
            else
            {
                comboActive = true;
                score += 10;
            }
            comboTimer = comboDuration;
        }

        // Update the gameplay score text specifically
        UIManager.Instance.UpdateScore(score, true);
        UIManager.Instance.UpdateCombo(comboCount);
        UIManager.Instance.ResetComboTimer();
    }

    IEnumerator LevelCompleteRoutine()
    {
        comboActive = false;
        UIManager.Instance.ResetComboTimer();

        yield return new WaitForSeconds(1f);

        // Save level stats
        SaveSystem.UnlockNextLevel(currentLevelIndex);
        SaveSystem.SaveLevelTurn(currentLevelIndex, turnsTaken);
        SaveSystem.SaveLevelCombo(currentLevelIndex, comboCount);

        // Save high score for this specific level (only updates if beaten)
        SaveSystem.SaveLevelHighScore(currentLevelIndex, score);

        // Update total score text on Level Select screen in background
        int totalScore = SaveSystem.GetTotalScore(levelsData.levels.Count);
        UIManager.Instance.UpdateScore(totalScore, false);

        // Restore music volume when level is complete
        AudioManager.Instance.RestoreMusicVolume();

        UIManager.Instance.ShowNextLevelUI();
        AudioManager.Instance.Play(SoundType.LevelComplete);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.D))
        {
            print("Data reseted!");
            UIManager.Instance.ResetLevels();
        }

        if (!comboActive) return;

        comboTimer -= Time.deltaTime;
        UIManager.Instance.UpdateComboTimer(comboTimer);

        if (comboTimer <= 0)
            comboActive = false;
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levelsData.levels.Count)
        {
            StartCoroutine(LoadLevelRoutine(currentLevelIndex));
        }
        else
        {
            // All levels completed - show game over screen (no GameOver sound, just show UI)
            AudioManager.Instance.RestoreMusicVolume();
            UIManager.Instance.ShowGameOver();
        }
    }
}