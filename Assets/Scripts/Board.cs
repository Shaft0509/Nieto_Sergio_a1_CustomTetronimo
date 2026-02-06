using UnityEngine;
using UnityEngine.Tilemaps;

// controls board logic and tile placement
public class Board : MonoBehaviour
{
    public Vector2Int boardSize = new Vector2Int(10, 20);

    // board bounds (centered like in lectures)
    public int Left => -boardSize.x / 2;
    public int Right => boardSize.x / 2;
    public int Bottom => -boardSize.y / 2;
    public int Top => boardSize.y / 2;

    public Tilemap tilemap;
    public Piece piecePrefab;
    public TetrisManager manager;

    // all piece definitions (set in inspector)
    public TetrominoData[] pieces;

    public Vector3Int spawnPosition;

    private Piece activePiece;

    private void Awake()
    {
        // spawn near top center
        spawnPosition = new Vector3Int(0, Top - 2, 0);
    }

    private void Start()
    {
        ResetBoard();
    }

    public void ResetBoard()
    {
        tilemap.ClearAllTiles();
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        if (manager.gameOver) return;

        if (activePiece == null)
            activePiece = Instantiate(piecePrefab);

        // pick random piece
        TetrominoData data = pieces[Random.Range(0, pieces.Length)];
        activePiece.Initialize(this, data, spawnPosition);

        // if spawn blocked -> game over
        if (!IsValidPosition(spawnPosition, activePiece.localCells))
        {
            manager.SetGameOver(true);
            return;
        }

        Set(activePiece);
    }

    // draws tiles for a piece
    public void Set(Piece piece)
    {
        foreach (var cell in piece.cells)
            tilemap.SetTile(cell, piece.data.tile);
    }

    // clears tiles so piece can move
    public void Clear(Piece piece)
    {
        foreach (var cell in piece.cells)
            tilemap.SetTile(cell, null);
    }

    // checks bounds + collision
    public bool IsValidPosition(Vector3Int pos, Vector2Int[] cells)
    {
        foreach (var c in cells)
        {
            Vector3Int tilePos = pos + (Vector3Int)c;

            if (tilePos.x < Left || tilePos.x >= Right ||
                tilePos.y < Bottom || tilePos.y >= Top)
                return false;

            if (tilemap.HasTile(tilePos))
                return false;
        }
        return true;
    }

    // called when a piece locks
    public void LockAndContinue(PieceType type)
    {
        int lines = ClearLines();

        if (lines > 0)
        {
            int score = ScoreForLines(lines);

            // custom bonus for PinkZ
            if (type == PieceType.PinkZ)
                score += 200;

            manager.AddScore(score);
        }

        SpawnPiece();
    }

    private int ScoreForLines(int lines)
    {
        if (lines == 1) return 100;
        if (lines == 2) return 300;
        if (lines == 3) return 500;
        return 800;
    }

    private int ClearLines()
    {
        int cleared = 0;

        for (int y = Bottom; y < Top; y++)
        {
            if (IsLineFull(y))
            {
                ClearLine(y);
                ShiftDown(y);
                y--;
                cleared++;
            }
        }
        return cleared;
    }

    private bool IsLineFull(int y)
    {
        for (int x = Left; x < Right; x++)
        {
            if (!tilemap.HasTile(new Vector3Int(x, y, 0)))
                return false;
        }
        return true;
    }

    private void ClearLine(int y)
    {
        for (int x = Left; x < Right; x++)
            tilemap.SetTile(new Vector3Int(x, y, 0), null);
    }

    private void ShiftDown(int startY)
    {
        for (int y = startY + 1; y < Top; y++)
        {
            for (int x = Left; x < Right; x++)
            {
                Vector3Int from = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(from);
                if (tile == null) continue;

                tilemap.SetTile(new Vector3Int(x, y - 1, 0), tile);
                tilemap.SetTile(from, null);
            }
        }
    }
}
