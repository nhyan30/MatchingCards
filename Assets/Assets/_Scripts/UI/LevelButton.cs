using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;
    public TMP_Text levelText;
    public TMP_Text turmAmountTXT;
    public TMP_Text comboAmountTXT;
    public GameObject lockedSprite;
    public bool isLocked = true;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button component not found on LevelButton.");
        }
        else
        {
            button.onClick.AddListener(OnClick);
            button.interactable = !isLocked;
        }
    }

    public void OnClick()
    {
        UIManager.Instance.ActivateUI(UIManager.Instance.gamePlay);
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
