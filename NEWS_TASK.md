# Coreline Defense — UI/UX Upgrade Instructions

## Architecture (đọc trước khi làm bất cứ việc gì)

Toàn bộ UI được sinh ra bằng code C# thuần. Không có prefab UI, không có Canvas trong scene.
Hai file chính:
- `Assets/Scripts/UI/FrontendUiController.cs` — MainMenu, Settings, HowToPlay, MissionMap screens
- `Assets/Scripts/UI/GameUiController_UiFactory.cs` — factory methods cho GameScene UI (slider, toggle, button)

FrontendUiController có factory methods RIÊNG ở cuối file (CreateSlider, CreateToggle, CreateButton, v.v.)
GameUiController_UiFactory.cs có factory methods RIÊNG tương tự.
Hai file này KHÔNG share factory — phải sửa CẢ HAI khi đổi component style.

## Design tokens (áp dụng nhất quán)

```
accent (cyan)   : new Color(0.12f, 0.82f, 0.95f, 1f)   // đã có trong FrontendUiController
hot (đỏ)        : new Color(1f, 0.28f, 0.22f, 1f)       // CHỈ dùng cho danger/destructive
success (xanh)  : new Color(0.3f, 0.9f, 0.62f, 0.92f)  // completed state
warning (amber) : new Color(1f, 0.68f, 0.22f, 1f)       // pressure score cao
text primary    : Color.white
text secondary  : new Color(0.82f, 0.93f, 0.97f, 1f)
text muted      : new Color(0.48f, 0.75f, 0.82f, 0.74f)
```

## Rule tuyệt đối

- Slider handle KHÔNG ĐƯỢC dùng `hot` (đỏ). Phải dùng `accent` (cyan).
- Toggle/checkbox KHÔNG ĐƯỢC dùng `hot`. Phải dùng `accent`.
- `hot` chỉ dùng cho EXIT button hoặc destructive action.
- Mọi thay đổi slider/toggle phải sửa ở CẢ HAI file: FrontendUiController.cs VÀ GameUiController_UiFactory.cs


## Tasks — Phase 1 (Critical, làm trước)

### TASK-01: Đồng bộ màu slider handle — FrontendUiController.cs
File: `Assets/Scripts/UI/FrontendUiController.cs`
Method: `CreateSlider(string name, Transform parent, float initialValue)` — khoảng dòng 655–678

Vấn đề: Handle màu `hot` (đỏ). Sai ngữ nghĩa.

Thay dòng:
```csharp
Image handle = CreateImage("Handle", rootObj.transform, hot);
```
Thành:
```csharp
Image handle = CreateImage("Handle", rootObj.transform, accent);
```

Không thay đổi gì khác trong method này.

---

### TASK-02: Đồng bộ màu slider handle — GameUiController_UiFactory.cs
File: `Assets/Scripts/UI/GameUiController_UiFactory.cs`
Method: `CreateSlider(string name, Transform parent, float initialValue)` — tìm dòng có `Image handle = CreateImage("Handle", ...)`

Vấn đề: Handle màu `Color.white`. Không nhất quán với FrontendUiController.

Thay dòng:
```csharp
Image handle = CreateImage("Handle", root.transform, Color.white);
```
Thành:
```csharp
Image handle = CreateImage("Handle", root.transform, accentColor);
```

Không thay đổi gì khác.

---

### TASK-03: Đồng bộ màu toggle checkmark — FrontendUiController.cs
File: `Assets/Scripts/UI/FrontendUiController.cs`
Method: `CreateToggle(string name, Transform parent, string labelText)` — khoảng dòng 680–698

Vấn đề: Checkmark dùng `hot` (đỏ).

Thay dòng:
```csharp
Image check = CreateImage("Checkmark", box.transform, hot);
```
Thành:
```csharp
Image check = CreateImage("Checkmark", box.transform, accent);
```

---

### TASK-04: Đổi màu nút START GAME sang accent (cyan)
File: `Assets/Scripts/UI/FrontendUiController.cs`
Method: `BuildMainMenu()` — tìm dòng:
```csharp
CreateMenuButton("StartGame", buttons, "START GAME", true, () => Load(SceneNames.MissionMap));
```

Tìm method `CreateMenuButton`. Hiện tại nút primary (`isPrimary=true`) dùng màu gì thì đổi thành `accent`.
Tìm trong `CreateMenuButton`:
```csharp
Color normal = primary ? hot : panelSoft;
```
Thành:
```csharp
Color normal = primary ? accent : panelSoft;
```
Đồng thời đổi text color của primary button thành đen để readable trên nền cyan:
Trong cùng method, khi `primary == true`, truyền `new Color(0.02f, 0.06f, 0.08f, 1f)` thay vì `Color.white` cho textColor.

---

### TASK-05: Đổi màu nút EXIT sang hot (đỏ) — semantic đúng
File: `Assets/Scripts/UI/FrontendUiController.cs`
Method: `BuildMainMenu()`

Tìm dòng:
```csharp
CreateMenuButton("Exit", buttons, "EXIT", false, Quit);
```
Thêm overload hoặc sửa để EXIT button dùng border màu `hot` thay vì màu muted hiện tại.
Thêm sau dòng đó:
```csharp
// Tô border đỏ cho EXIT
var exitBtn = buttons.GetChild(buttons.childCount - 1);
if (exitBtn != null)
{
    var outline = exitBtn.gameObject.GetComponent<Outline>();
    if (outline != null) outline.effectColor = new Color(hot.r, hot.g, hot.b, 0.7f);
}
```

---

### TASK-06: Cập nhật footer text
File: `Assets/Scripts/UI/FrontendUiController.cs`
Method: `BuildMainMenu()`

Tìm dòng:
```csharp
TextMeshProUGUI footer = CreateText("Footer", command, "v0.1  /  FRONTEND ROUTE READY", ...
```
Thành:
```csharp
TextMeshProUGUI footer = CreateText("Footer", command, "MISSION MAP ONLINE", ...
```

---

## Tasks — Phase 2 (High priority)

### TASK-07: Tool selector selected state — GameScene
File: `Assets/Scripts/UI/GameUiController.cs` hoặc file partial liên quan
Tìm nơi build tool bar (ArcReactor, Turret, Bunker buttons ở bottom).

Thêm selected state: khi tool được chọn, đổi button background = `accentColor`, text = `Color.black`.
Khi unselected: giữ nguyên màu cũ.

Tìm method build bottom bar (có thể là `BuildToolBar` hoặc tương tự trong GameUiController_Campaign.cs).
Thêm field `Button[] toolButtons` và `int selectedToolIndex = -1`.
Thêm method:
```csharp
void SetSelectedTool(int index)
{
    selectedToolIndex = index;
    for (int i = 0; i < toolButtons.Length; i++)
    {
        bool sel = i == index;
        Image img = toolButtons[i].GetComponent<Image>();
        if (img != null) img.color = sel ? accentColor : new Color(0.04f, 0.08f, 0.12f, 0.95f);
        TextMeshProUGUI lbl = toolButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        if (lbl != null) lbl.color = sel ? Color.black : Color.white;
    }
}
```
Gọi `SetSelectedTool(i)` trong onClick của mỗi tool button.

---

### TASK-08: Toggle redesign — pill shape thay checkbox
File: `Assets/Scripts/UI/FrontendUiController.cs`
Method: `CreateToggle(string name, Transform parent, string labelText)` — dòng 680–698

Thay toàn bộ method bằng pill toggle (44×24px):
```csharp
Toggle CreateToggle(string name, Transform parent, string labelText)
{
    GameObject rootObj = new GameObject(name, typeof(RectTransform), typeof(Toggle));
    rootObj.transform.SetParent(parent, false);
    Toggle toggle = rootObj.GetComponent<Toggle>();

    // Track (pill shape)
    Image track = CreateImage("Track", rootObj.transform, new Color(0.08f, 0.12f, 0.16f, 1f));
    SetAnchor(track.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
        new Vector2(0f, -12f), new Vector2(44f, 12f));
    track.rectTransform.GetComponent<Image>().raycastTarget = true;
    // Pill border
    AddFrame(track.rectTransform, new Color(accent.r, accent.g, accent.b, 0.4f));

    // Knob
    Image knob = CreateImage("Knob", track.transform, new Color(0.55f, 0.65f, 0.72f, 1f));
    SetAnchor(knob.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
        new Vector2(3f, -9f), new Vector2(21f, 9f));

    // Label
    TextMeshProUGUI label = CreateText("Label", rootObj.transform, labelText, 20, FontStyle.Bold, TextAnchor.MiddleLeft);
    SetAnchor(label.rectTransform, new Vector2(0f, 0f), Vector2.one, new Vector2(52f, 0f), Vector2.zero);

    toggle.targetGraphic = track;
    toggle.graphic = knob;

    // Cập nhật màu khi toggle thay đổi
    toggle.onValueChanged.AddListener(on =>
    {
        track.color = on
            ? new Color(accent.r * 0.3f, accent.g * 0.3f, accent.b * 0.3f, 0.9f)
            : new Color(0.08f, 0.12f, 0.16f, 1f);
        knob.color = on ? accent : new Color(0.55f, 0.65f, 0.72f, 1f);
        // Slide knob
        var rt = knob.rectTransform;
        rt.offsetMin = on ? new Vector2(23f, rt.offsetMin.y) : new Vector2(3f, rt.offsetMin.y);
        rt.offsetMax = on ? new Vector2(41f, rt.offsetMax.y) : new Vector2(21f, rt.offsetMax.y);
    });

    // Trigger initial state
    toggle.onValueChanged.Invoke(toggle.isOn);
    return toggle;
}
```

Sau khi sửa FrontendUiController, áp dụng logic tương tự cho `CreateToggle` trong `GameUiController_UiFactory.cs`.

---

### TASK-09: Mission node size tăng lên
File: `Assets/Scripts/UI/FrontendUiController.cs`
Method: `RebuildMissionNodes()` — tìm dòng:
```csharp
rect.sizeDelta = new Vector2(118f, 88f);
```
Thành:
```csharp
rect.sizeDelta = new Vector2(136f, 96f);
```

---

### TASK-10: DEPLOY button height tăng lên
File: `Assets/Scripts/UI/FrontendUiController.cs`
Method: `BuildMissionDetail()` — tìm dòng SetAnchor của deployButton:
```csharp
SetAnchor((RectTransform)deployButton.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(24f, 24f), new Vector2(-24f, 78f));
```
Thành:
```csharp
SetAnchor((RectTransform)deployButton.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(24f, 20f), new Vector2(-24f, 84f));
```
(Tăng height từ 54px lên 64px)

---

## Thứ tự thực hiện

1. TASK-01 → TASK-02 → TASK-03 (slider/toggle color — fix ngay visual inconsistency lớn nhất)
2. TASK-04 → TASK-05 → TASK-06 (main menu hierarchy)
3. TASK-07 (tool selector — gameplay feedback)
4. TASK-08 (toggle redesign — sau khi color đã đúng)
5. TASK-09 → TASK-10 (mission map polish)

Sau mỗi task: build và chạy scene tương ứng để verify trước khi làm task tiếp theo.
