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

}


[CreateAssetMenu(menuName = "MemoryGame/LevelData")]
public class LevelData : ScriptableObject
{
  public List<LevelDataStruct> levels;
}