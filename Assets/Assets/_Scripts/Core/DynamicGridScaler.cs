using UnityEngine;
using UnityEngine.UI;

public class DynamicGridScaler : MonoBehaviour
{
    private RectTransform rect;
    private GridLayoutGroup grid;

    [Tooltip("Width divided by Height. Standard playing cards are roughly 0.7 (2.5:3.5 ratio)")]
    [SerializeField] private float cardAspectRatio = 0.7f;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        grid = GetComponent<GridLayoutGroup>();
    }

    public void UpdateGrid(int rows, int columns, float spacing, float padding)
    {
        Vector2 size = rect.rect.size;

        float availableWidth = (size.x - padding * 2 - spacing * (columns - 1)) / columns;
        float availableHeight = (size.y - padding * 2 - spacing * (rows - 1)) / rows;

        float cellWidth, cellHeight;

        // Determine which dimension limits the scaling to maintain the aspect ratio
        if (availableWidth / cardAspectRatio <= availableHeight)
        {
            // Width is the limiting factor (or equal)
            cellWidth = availableWidth;
            cellHeight = cellWidth / cardAspectRatio;
        }
        else
        {
            // Height is the limiting factor
            cellHeight = availableHeight;
            cellWidth = cellHeight * cardAspectRatio;
        }

        grid.cellSize = new Vector2(cellWidth, cellHeight);
        grid.spacing = new Vector2(spacing, spacing);
        grid.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
    }
}