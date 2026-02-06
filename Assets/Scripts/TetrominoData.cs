using UnityEngine;
using UnityEngine.Tilemaps;

// holds data for one tetris piece
// this is filled in the Board inspector
[System.Serializable]
public class TetrominoData
{
    public PieceType type;     // which piece it is
    public TileBase tile;      // tile used to draw it
    public Vector2Int[] cells; // shape (local positions)
    public bool lockRotation;  // used for O piece
}
