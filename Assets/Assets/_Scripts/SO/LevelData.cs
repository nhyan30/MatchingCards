using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LevelDataStruct
{
    public string levelName;
    public int columns;
    public int rows;
    public int padding;
    public int cardSpacing;
    public List<Sprite> cardImages;

    // NEW: List of grid coordinates (x=column, y=row) that should be empty
    // Example to remove 4 corners in a 4x5 grid: (0,0), (3,0), (0,4), (3,4)
    public List<Vector2Int> disabledPositions;
}

[CreateAssetMenu(menuName = "MemoryGame/LevelData")]
public class LevelData : ScriptableObject
{
    public List<LevelDataStruct> levels;
}