using UnityEngine;

[RequireComponent(typeof(GridManager))]
public class BoardVisualController : MonoBehaviour
{
    [Header("Palette")]
    public Color backdropColor = new Color(0.015f, 0.028f, 0.045f, 1f);
    public Color boardBaseColor = new Color(0.07f, 0.095f, 0.12f, 1f);
    public Color laneColorA = new Color(0.09f, 0.13f, 0.16f, 1f);
    public Color laneColorB = new Color(0.075f, 0.105f, 0.135f, 1f);
    public Color gridLineColor = new Color(0.16f, 0.29f, 0.34f, 0.68f);
    public Color accentLineColor = new Color(0.08f, 0.85f, 0.96f, 0.72f);
    public Color dangerLineColor = new Color(1f, 0.24f, 0.12f, 0.62f);

    [Header("Geometry")]
    public float outerPadding = 0.36f;
    public float backdropPaddingX = 2.35f;
    public float backdropPaddingY = 1.2f;
    public float gridLineWidth = 0.025f;
    public float accentLineWidth = 0.055f;

    private static Sprite pixelSprite;
    private GridManager grid;
    private Transform visualRoot;

    void Start()
    {
        grid = GetComponent<GridManager>();
        Build();
    }

    void Build()
    {
        if (grid == null) return;

        if (visualRoot != null)
            Destroy(visualRoot.gameObject);

        visualRoot = new GameObject("BoardVisuals").transform;
        visualRoot.SetParent(transform, false);

        Vector2 center = BoardCenter();
        Vector2 boardSize = BoardSize();

        AddRect("Backdrop", center, boardSize + new Vector2(backdropPaddingX, backdropPaddingY), backdropColor, -40);
        AddBackdropPanels(center, boardSize);
        AddRect("BoardBase", center, boardSize + Vector2.one * outerPadding, boardBaseColor, -35);

        for (int row = 0; row < grid.rows; row++)
        {
            Vector2 laneCenter = new Vector2(center.x, grid.origin.y + row * grid.cellSize);
            Color laneColor = row % 2 == 0 ? laneColorA : laneColorB;
            AddRect($"Lane_{row}", laneCenter, new Vector2(boardSize.x, grid.cellSize * 0.92f), laneColor, -32);
        }

        AddDefenseZone(center, boardSize);
        AddSpawnZone(center, boardSize);
        AddGridLines(center, boardSize);
        AddLaneLabels(center, boardSize);
        AddEntryChevrons(center, boardSize);
        AddFrame(center, boardSize);
    }

    void AddBackdropPanels(Vector2 center, Vector2 boardSize)
    {
        Color panelA = new Color(0.025f, 0.055f, 0.075f, 0.78f);
        Color panelB = new Color(0.015f, 0.038f, 0.058f, 0.72f);

        AddRect("BackdropPanel_Left", center + new Vector2(-boardSize.x * 0.44f, boardSize.y * 0.68f), new Vector2(2.4f, 0.28f), panelA, -39);
        AddRect("BackdropPanel_Right", center + new Vector2(boardSize.x * 0.34f, -boardSize.y * 0.7f), new Vector2(3.4f, 0.24f), panelB, -39);
        AddRect("BackdropPanel_Top", center + new Vector2(boardSize.x * 0.16f, boardSize.y * 0.78f), new Vector2(4.2f, 0.16f), new Color(accentLineColor.r, accentLineColor.g, accentLineColor.b, 0.16f), -38);
        AddRect("BackdropPanel_Bottom", center + new Vector2(-boardSize.x * 0.1f, -boardSize.y * 0.82f), new Vector2(4.8f, 0.14f), new Color(dangerLineColor.r, dangerLineColor.g, dangerLineColor.b, 0.12f), -38);
    }

    void AddDefenseZone(Vector2 center, Vector2 boardSize)
    {
        Vector2 zoneCenter = new Vector2(grid.origin.x - grid.cellSize * 0.75f, center.y);
        AddRect("DefenseRail", zoneCenter, new Vector2(grid.cellSize * 0.42f, boardSize.y + outerPadding), new Color(0.04f, 0.12f, 0.16f, 0.95f), -31);
        AddRect("DefenseRailGlow", zoneCenter + Vector2.right * 0.24f, new Vector2(0.04f, boardSize.y + outerPadding), accentLineColor, -29);
    }

    void AddSpawnZone(Vector2 center, Vector2 boardSize)
    {
        Vector2 zoneCenter = new Vector2(grid.origin.x + grid.cols * grid.cellSize - grid.cellSize * 0.25f, center.y);
        AddRect("EnemyEntryZone", zoneCenter, new Vector2(grid.cellSize * 0.42f, boardSize.y + outerPadding), new Color(0.18f, 0.05f, 0.035f, 0.72f), -31);
        AddRect("EnemyEntryGlow", zoneCenter - Vector2.right * 0.25f, new Vector2(0.04f, boardSize.y + outerPadding), dangerLineColor, -29);
    }

    void AddLaneLabels(Vector2 center, Vector2 boardSize)
    {
        float left = grid.origin.x - grid.cellSize * 1.02f;
        for (int row = 0; row < grid.rows; row++)
        {
            Vector2 labelCenter = new Vector2(left, grid.origin.y + row * grid.cellSize);
            AddRect($"LaneSignal_{row}", labelCenter, new Vector2(0.18f, 0.42f), new Color(accentLineColor.r, accentLineColor.g, accentLineColor.b, 0.2f + row * 0.025f), -27);
            AddRect($"LaneSignalCore_{row}", labelCenter + Vector2.right * 0.03f, new Vector2(0.06f, 0.24f), new Color(accentLineColor.r, accentLineColor.g, accentLineColor.b, 0.55f), -26);
        }
    }

    void AddEntryChevrons(Vector2 center, Vector2 boardSize)
    {
        float right = grid.origin.x + (grid.cols - 0.38f) * grid.cellSize;
        for (int row = 0; row < grid.rows; row++)
        {
            float y = grid.origin.y + row * grid.cellSize;
            AddRect($"EntryChevronA_{row}", new Vector2(right, y + 0.13f), new Vector2(0.32f, 0.055f), new Color(dangerLineColor.r, dangerLineColor.g, dangerLineColor.b, 0.48f), -26);
            AddRect($"EntryChevronB_{row}", new Vector2(right, y - 0.13f), new Vector2(0.32f, 0.055f), new Color(dangerLineColor.r, dangerLineColor.g, dangerLineColor.b, 0.32f), -26);
        }
    }

    void AddGridLines(Vector2 center, Vector2 boardSize)
    {
        float left = grid.origin.x - grid.cellSize * 0.5f;
        float right = grid.origin.x + (grid.cols - 0.5f) * grid.cellSize;
        float bottom = grid.origin.y - grid.cellSize * 0.5f;
        float top = grid.origin.y + (grid.rows - 0.5f) * grid.cellSize;

        for (int col = 0; col <= grid.cols; col++)
        {
            float x = left + col * grid.cellSize;
            float width = col % 3 == 0 ? gridLineWidth * 1.8f : gridLineWidth;
            AddRect($"GridVertical_{col}", new Vector2(x, center.y), new Vector2(width, boardSize.y), gridLineColor, -28);
        }

        for (int row = 0; row <= grid.rows; row++)
        {
            float y = bottom + row * grid.cellSize;
            AddRect($"GridHorizontal_{row}", new Vector2(center.x, y), new Vector2(boardSize.x, gridLineWidth), gridLineColor, -28);
        }

        AddRect("TopNeonLine", new Vector2(center.x, top + 0.08f), new Vector2(boardSize.x + outerPadding, accentLineWidth), accentLineColor, -27);
        AddRect("BottomNeonLine", new Vector2(center.x, bottom - 0.08f), new Vector2(boardSize.x + outerPadding, accentLineWidth), new Color(accentLineColor.r, accentLineColor.g, accentLineColor.b, 0.35f), -27);
        AddRect("LeftGridCap", new Vector2(left, center.y), new Vector2(accentLineWidth, boardSize.y + outerPadding), new Color(accentLineColor.r, accentLineColor.g, accentLineColor.b, 0.35f), -27);
        AddRect("RightGridCap", new Vector2(right, center.y), new Vector2(accentLineWidth, boardSize.y + outerPadding), new Color(dangerLineColor.r, dangerLineColor.g, dangerLineColor.b, 0.42f), -27);
    }

    void AddFrame(Vector2 center, Vector2 boardSize)
    {
        Vector2 frameSize = boardSize + Vector2.one * outerPadding;
        float halfW = frameSize.x * 0.5f;
        float halfH = frameSize.y * 0.5f;
        Color frameColor = new Color(0.02f, 0.035f, 0.05f, 1f);

        AddRect("FrameTop", center + Vector2.up * halfH, new Vector2(frameSize.x, 0.12f), frameColor, -26);
        AddRect("FrameBottom", center + Vector2.down * halfH, new Vector2(frameSize.x, 0.12f), frameColor, -26);
        AddRect("FrameLeft", center + Vector2.left * halfW, new Vector2(0.12f, frameSize.y), frameColor, -26);
        AddRect("FrameRight", center + Vector2.right * halfW, new Vector2(0.12f, frameSize.y), frameColor, -26);
    }

    Vector2 BoardCenter()
    {
        return new Vector2(
            grid.origin.x + (grid.cols - 1) * grid.cellSize * 0.5f,
            grid.origin.y + (grid.rows - 1) * grid.cellSize * 0.5f);
    }

    Vector2 BoardSize()
    {
        return new Vector2(grid.cols * grid.cellSize, grid.rows * grid.cellSize);
    }

    void AddRect(string name, Vector2 center, Vector2 size, Color color, int sortingOrder)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(visualRoot, false);
        go.transform.position = new Vector3(center.x, center.y, 0.15f);
        go.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = PixelSprite();
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
    }

    static Sprite PixelSprite()
    {
        if (pixelSprite != null) return pixelSprite;

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        pixelSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        pixelSprite.hideFlags = HideFlags.HideAndDontSave;
        return pixelSprite;
    }
}
