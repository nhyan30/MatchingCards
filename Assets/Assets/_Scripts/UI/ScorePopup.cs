using UnityEngine;
using DG.Tweening;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float scaleUpAmount = 1.5f;
    [SerializeField] private float floatUpAmount = 50f;

    private TMP_Text popupText;
    private RectTransform rectTransform;

    private void Awake()
    {
        popupText = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Starts the popup animation at a specific screen position.
    /// </summary>
    /// <param name="text">The text to display (e.g., "+10" or "x2")</param>
    /// <param name="position">World space position to spawn (usually between the two matched cards)</param>
    /// <param name="color">Optional color for the text (defaults to original color)</param>
    public void Show(string text, Vector2 position, Color? color = null)
    {
        popupText.text = text;
        popupText.alpha = 0f;
        popupText.rectTransform.localScale = Vector3.one;

        // Apply color if provided, otherwise keep the default prefab color
        if (color.HasValue)
        {
            popupText.color = color.Value;
        }

        // Position the popup at the midpoint between the matched cards
        rectTransform.position = position;

        // Kill any existing tweens on this object to prevent overlapping glitches
        DOTween.Kill(this);

        // Sequence Animation
        Sequence seq = DOTween.Sequence();
        seq.SetTarget(this);

        // 1. Fade in and Scale up quickly
        seq.Append(popupText.DOFade(1f, 0.2f));
        seq.Join(rectTransform.DOScale(Vector3.one * scaleUpAmount, 0.3f).SetEase(Ease.OutBack));

        // 2. Hold for a moment while floating up slightly
        seq.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + floatUpAmount, animationDuration / 2).SetEase(Ease.OutQuad));

        // 3. Fade out and Scale down
        seq.Append(popupText.DOFade(0f, 0.4f));
        seq.Join(rectTransform.DOScale(Vector3.one * 0.5f, 0.4f).SetEase(Ease.InQuad));

        // 4. Destroy when done
        seq.OnComplete(() => Destroy(gameObject));
    }
}