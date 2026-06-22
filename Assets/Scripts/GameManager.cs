using UnityEngine;
using UnityEngine.SceneManagement;

// Gắn vào object "GameSystems".
// Quản lý trạng thái ván: sinh 1 lawnmower mỗi hàng lúc bắt đầu; xử lý THUA khi địch
// vượt tuyến trái mà hàng đó không còn lawnmower.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GridManager grid;             // kéo object GridManager vào
    public GameObject lawnmowerPrefab;   // kéo prefab Lawnmower vào

    private Lawnmower[] perRow;
    public bool IsGameOver { get; private set; }
    public bool IsWon { get; private set; }
    public bool showDebugImGui;
    private GUIStyle bigStyle;

    void Awake() { Instance = this; }

    void Start()
    {
        if (grid == null) return;
        perRow = new Lawnmower[grid.rows];
        if (lawnmowerPrefab == null) return;

        for (int r = 0; r < grid.rows; r++)
        {
            Vector3 pos = grid.CellToWorld(-1, r);   // ô ngay bên trái cột 0
            pos.z = -1f;
            GameObject m = Instantiate(lawnmowerPrefab, pos, Quaternion.identity);
            var lm = m.GetComponent<Lawnmower>();
            if (lm != null) { lm.grid = grid; lm.row = r; }
            perRow[r] = lm;
        }
    }

    // EnemyMover gọi khi vượt tuyến trái. Trả true nếu hàng còn lawnmower (nó sẽ dọn).
    public bool TryLawnmower(int row)
    {
        if (perRow == null || row < 0 || row >= perRow.Length) return false;
        var lm = perRow[row];
        if (lm == null) return false;   // đã dùng rồi
        lm.Activate();
        return true;
    }

    // Lawnmower gọi sau khi chạy xong: nhả slot (hàng đó hết đường cứu).
    public void ClearLawnmower(int row)
    {
        if (perRow != null && row >= 0 && row < perRow.Length) perRow[row] = null;
    }

    public void GameOver(int row)
    {
        if (IsGameOver || IsWon) return;
        if (GameUiController.Instance != null && GameUiController.Instance.isPaused) Time.timeScale = 1f;
        IsGameOver = true;
        Debug.Log($"GAME OVER - enemy breached row {row}");
        AudioManager.PlaySfx(SfxType.GameOver);
        Time.timeScale = 0f;
    }

    // EnemySpawner gọi khi qua đợt cuối và đã sạch màn.
    public void Win()
    {
        if (IsGameOver || IsWon) return;
        if (GameUiController.Instance != null && GameUiController.Instance.isPaused) Time.timeScale = 1f;
        IsWon = true;
        Debug.Log("YOU WIN!");
        if (LevelManager.Instance != null) LevelManager.Instance.MarkCurrentLevelCompleted();
        AudioManager.PlaySfx(SfxType.Win);
        Time.timeScale = 0f;
    }

    void OnGUI()
    {
        if (!showDebugImGui) return;
        if (!IsGameOver && !IsWon) return;

        if (bigStyle == null)
            bigStyle = new GUIStyle(GUI.skin.label)
            { fontSize = 48, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };

        GUI.Label(new Rect(0, 0, Screen.width, Screen.height - 60), IsWon ? "YOU WIN!" : "GAME OVER", bigStyle);

        if (GUI.Button(new Rect(Screen.width / 2f - 70, Screen.height / 2f + 40, 140, 40), "Restart"))
        {
            AudioManager.PlaySfx(SfxType.UiClick);
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
