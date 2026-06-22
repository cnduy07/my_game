using UnityEngine;

// Gắn vào prefab tuyến cứu cuối mỗi hàng.
// Hành vi hiện tại là Rail Cannon: khi bị breach, đứng yên và bắn một beam xuyên cả hàng.
// Giữ tên class Lawnmower để prefab cũ không bị missing script.
public class Lawnmower : MonoBehaviour
{
    public GridManager grid;
    public int row;
    public float warningDelay = 0.18f;
    public float beamDuration = 0.35f;
    public float damage = 99999f;
    public LineRenderer beamLine;
    public Color beamColor = new Color(1f, 0.18f, 0.05f, 1f);
    public float beamWidth = 0.08f;

    private bool running;
    private bool fired;
    private float timer;

    public void Activate()
    {
        if (running) return;
        running = true;
        timer = 0f;
        PrepareBeam();
        AudioManager.PlaySfx(SfxType.RailCannon);
    }

    void Update()
    {
        if (!running) return;

        timer += Time.deltaTime;

        if (!fired && timer >= warningDelay)
            Fire();

        if (fired && timer >= warningDelay + beamDuration)
        {
            if (GameManager.Instance != null) GameManager.Instance.ClearLawnmower(row);
            Destroy(gameObject);
        }
    }

    void Fire()
    {
        fired = true;
        SetBeamVisible(true);
        CombatVfx.PlayRailCannonBeam(transform.position);

        // Duyệt trên bản sao: TakeDamage có thể đổi danh sách All ngay trong lúc lặp.
        var list = EnemyMover.All.ToArray();
        foreach (var e in list)
        {
            if (e == null || e.row != row) continue;
            CombatVfx.PlayHitSpark(e.transform.position);
            var hp = e.GetComponent<Health>();
            if (hp != null) hp.TakeDamage(damage);
        }
    }

    void PrepareBeam()
    {
        if (beamLine == null)
            beamLine = GetComponent<LineRenderer>();

        if (beamLine == null)
            beamLine = gameObject.AddComponent<LineRenderer>();

        beamLine.useWorldSpace = true;
        beamLine.positionCount = 2;
        beamLine.startWidth = beamWidth;
        beamLine.endWidth = beamWidth;
        beamLine.startColor = beamColor;
        beamLine.endColor = beamColor;
        beamLine.sortingOrder = 20;

        if (beamLine.sharedMaterial == null)
        {
            var shader = Shader.Find("Sprites/Default");
            if (shader != null) beamLine.sharedMaterial = new Material(shader);
        }

        if (grid != null)
        {
            Vector3 start = grid.CellToWorld(0, row);
            Vector3 end = grid.CellToWorld(grid.cols - 1, row);
            start.x = grid.origin.x - 0.35f;
            end.x = grid.origin.x + grid.cols * grid.cellSize;
            start.z = -3f;
            end.z = -3f;
            beamLine.SetPosition(0, start);
            beamLine.SetPosition(1, end);
        }

        SetBeamVisible(false);
    }

    void SetBeamVisible(bool visible)
    {
        if (beamLine != null) beamLine.enabled = visible;
    }
}
