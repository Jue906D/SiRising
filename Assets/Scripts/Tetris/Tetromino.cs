using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I, J, L, O, S, T, Z
}
public enum Element
{//ÂÌÀ¶ºì»Æ
    None = 0,
    Grass = 1,
    Water = 2,
    Fire = 3,
    Earth = 4
}

[System.Serializable]
public struct TetrominoData
{
    public Tile tile;
    public Tetromino tetromino;

    public Element elem;//0

    public Vector2Int[] cells { get; private set; }//hide
    public Vector2Int[,] wallKicks { get; private set; }
    

    public void Initialize()
    {
        cells = Data.Cells[tetromino];
        wallKicks = Data.WallKicks[tetromino];
        wallKicks = Data.WallKicks[tetromino];
    }

}
