using UnityEngine;

// Owns match terminal state and lane failsafes.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GridManager grid;
    public GameObject lawnmowerPrefab;

    private Lawnmower[] perRow;
    public bool IsGameOver { get; private set; }
    public bool IsWon { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (grid == null) return;
        perRow = new Lawnmower[grid.rows];
        if (lawnmowerPrefab == null) return;

        for (int r = 0; r < grid.rows; r++)
        {
            Vector3 pos = grid.CellToWorld(-1, r);
            pos.z = -1f;
            GameObject m = Instantiate(lawnmowerPrefab, pos, Quaternion.identity);
            var lm = m.GetComponent<Lawnmower>();
            if (lm != null) { lm.grid = grid; lm.row = r; }
            perRow[r] = lm;
        }
    }

    public bool TryLawnmower(int row)
    {
        if (perRow == null || row < 0 || row >= perRow.Length) return false;
        var lm = perRow[row];
        if (lm == null) return false;   // đã dùng rồi
        lm.Activate();
        return true;
    }

    public void ClearLawnmower(int row)
    {
        if (perRow != null && row >= 0 && row < perRow.Length) perRow[row] = null;
    }

    public void GameOver(int row)
    {
        if (IsGameOver || IsWon) return;
        IsGameOver = true;
        AudioManager.PlaySfx(SfxType.GameOver);
        Time.timeScale = 0f;
    }

    public void Win()
    {
        if (IsGameOver || IsWon) return;
        if (GameUiController.Instance != null && GameUiController.Instance.isPaused) Time.timeScale = 1f;
        IsWon = true;
        if (LevelManager.Instance != null) LevelManager.Instance.MarkCurrentLevelCompleted();
        AudioManager.PlaySfx(SfxType.Win);
        Time.timeScale = 0f;
    }
}
