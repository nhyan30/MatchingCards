using UnityEngine;
using UnityEngine.UI;

public class DynamicGridScaler : MonoBehaviour
{
    private RectTransform rect;
    private GridLayoutGroup grid;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        grid = GetComponent<GridLayoutGroup>();
    }

    public void UpdateGrid(int rows, int columns, float spacing, float padding)
    {
        Vector2 size = rect.rect.size;

        float width = (size.x - padding * 2 - spacing * (columns - 1)) / columns;
        float height = (size.y - padding * 2 - spacing * (rows - 1)) / rows;

        float cell = Mathf.Min(width, height);

        grid.cellSize = new Vector2(cell, cell);
        grid.spacing = new Vector2(spacing, spacing);
        grid.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
    }
}