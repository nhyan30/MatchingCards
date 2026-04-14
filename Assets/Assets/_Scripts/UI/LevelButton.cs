using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;
    public TMP_Text levelText;
    public TMP_Text turmAmountTXT;
    public TMP_Text comboAmountTXT;
    public TMP_Text scoreAmountTXT; // NEW: Drag this in the Inspector
    public GameObject lockedSprite;

    private Button button;
    private bool isLocked = true;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void OnClick()
    {
        // Use the centralized UI transition method
        UIManager.Instance.ShowGameplay();
        GameManager.Instance.LoadLevel(levelIndex);
    }

    public void SetLevelIndex(int index, string levelName)
    {
        levelIndex = index;
        levelText.text = levelName;
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        lockedSprite.SetActive(locked);
        button.interactable = !locked;
    }
}