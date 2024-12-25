///<summary>
/// Cell.cs Script of Minesweeper Game
///</summary>
using UnityEngine;

public class Cell
{
    public enum Type
    {
        Empty,
        Mine,
        Number,
    }

    public Vector3Int position;
    public Type type;
    public int number;
    public bool revealed;
    public bool exploded;
}
