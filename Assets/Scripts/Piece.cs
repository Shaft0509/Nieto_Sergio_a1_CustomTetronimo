using UnityEngine;

// controls movement + rotation of a piece
public class Piece : MonoBehaviour
{
    public Board board;
    public TetrominoData data;

    public Vector3Int position;

    public Vector2Int[] localCells; // rotated version
    public Vector3Int[] cells;      // world positions

    public float stepDelay = 0.8f;
    private float stepTime;

    // --- PinkZ custom mechanic (auto sideways drift) ---
    [Header("PinkZ Drift")]
    public float driftDelay = 0.35f;   // how often it slides sideways
    private float driftTime;
    private int driftDir = 1;          // 1 = right, -1 = left
    private bool driftEnabled;
    // ---------------------------------------------------

    public void Initialize(Board board, TetrominoData data, Vector3Int spawn)
    {
        this.board = board;
        this.data = data;
        position = spawn;

        // copy shape data
        localCells = new Vector2Int[data.cells.Length];
        data.cells.CopyTo(localCells, 0);

        cells = new Vector3Int[localCells.Length];

        stepTime = Time.time + stepDelay;

        // PinkZ drift setup
        driftEnabled = (data.type == PieceType.PinkZ);
        driftTime = Time.time + driftDelay;
        driftDir = 1;

        UpdateCells();
    }

    private void Update()
    {
        if (board.manager.gameOver) return;

        board.Clear(this);

        // --- PinkZ drift happens automatically ---
        if (driftEnabled && Time.time >= driftTime)
        {
            driftTime = Time.time + driftDelay;

            // try to drift in current direction
            bool moved = Move(new Vector2Int(driftDir, 0));

            // if blocked, bounce direction and try once
            if (!moved)
            {
                driftDir *= -1;
                Move(new Vector2Int(driftDir, 0));
            }
        }

        // player input (still works)
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Rotate();
        if (Input.GetKeyDown(KeyCode.Space)) HardDrop();

        // gravity
        if (Time.time >= stepTime)
        {
            stepTime = Time.time + stepDelay;

            if (!Move(Vector2Int.down))
                Lock();
        }

        board.Set(this);
    }

    private void UpdateCells()
    {
        for (int i = 0; i < localCells.Length; i++)
            cells[i] = position + (Vector3Int)localCells[i];
    }

    private bool Move(Vector2Int dir)
    {
        Vector3Int newPos = position + (Vector3Int)dir;

        if (board.IsValidPosition(newPos, localCells))
        {
            position = newPos;
            UpdateCells();
            return true;
        }

        return false;
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down)) { }
        Lock();
    }

    private void Rotate()
    {
        if (data.lockRotation) return;

        Vector2Int[] backup = (Vector2Int[])localCells.Clone();

        // rotate 90 degrees
        for (int i = 0; i < localCells.Length; i++)
            localCells[i] = new Vector2Int(localCells[i].y, -localCells[i].x);

        // basic wall kicks
        if (!board.IsValidPosition(position, localCells))
        {
            if (!Move(Vector2Int.left) && !Move(Vector2Int.right))
                localCells = backup;
        }

        UpdateCells();
    }

    private void Lock()
    {
        board.Set(this);
        board.LockAndContinue(data.type);
    }
}
