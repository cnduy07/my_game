# Game Tower Defense (PvZ Reskin) — Tiến độ dự án

**Chủ đề:** Game thủ thành phong cách sci-fi tương lai (KHÔNG gắn tên thương hiệu có bản quyền).
Phòng thủ: súng turret hiện đại + lô cốt bọc giáp + lõi năng lượng. Địch: **robot ngoài hành tinh (alien robot invader)** tiến từ phải sang trái.
> Lưu ý IP: tránh nêu thương hiệu thật ("Iron Man", "Plants vs Zombies"…) trong prompt/asset. Chỉ dùng làm cảm hứng, mô tả bằng từ chung chung (powered exo-armor, arc-reactor sci-fi).
**Engine:** Unity 2D. **Lưới:** 5 hàng × 9 cột. **Quy ước:** 1 ô = 1 unit.

---

## ✅ ĐÃ LÀM

### Thiết kế & định hướng
- Chốt 5 cơ chế lõi PvZ phải giữ: lưới 5×9, kinh tế năng lượng (sun), seed packet + cooldown, địch ăn đơn vị, tuyến cứu cuối (lawnmower), wave tăng dần.
- Bảng reskin chủ đề sci-fi (chung chung, không brand): Sunflower → lõi năng lượng (Arc Reactor sci-fi); Peashooter → súng turret; Wall-nut → lô cốt giáp; Cherry bomb → drone EMP; **Zombie → robot ngoài hành tinh (alien robot)**; Lawnmower → laser tuyến cuối.
- Chốt hướng art: **skeletal rig** (Unity 2D Animation / DragonBones) thay vì spritesheet frame-by-frame. Canvas chung 256×256, pivot canh đáy giữa, PPU đồng nhất.
- Kết luận về AI tạo art: dùng để sinh **concept tĩnh** từng object, sau đó cắt rời bộ phận và rig thủ công — KHÔNG dùng AI tạo spritesheet animation hoàn chỉnh (lỗi consistency).

### Unity — dựng khung "bản xám"
- Tạo project Unity 2D, làm việc trong SampleScene.
- Canh camera Orthographic, Size 4, Position (4,2,-10) để ôm trọn lưới.
- Tạo 4 prefab placeholder: Tile (xám), Unit (xanh), Enemy (vòng đỏ), Bullet (vòng vàng).
- Tạo object quản lý: GridManager và GameSystems.

### Unity — viết 7 script C# (không dùng physics/collider)
- `Health.cs` — máu chung cho Unit & Enemy.
- `GridManager.cs` — lưới 9×5, đổi (cột,hàng) ↔ world, theo dõi ô có Unit.
- `PlacementController.cs` — click chuột đặt Unit vào ô trống.
- `EnemySpawner.cs` — sinh địch theo thời gian ở cột phải, hàng ngẫu nhiên.
- `EnemyMover.cs` — địch đi sang trái, gặp Unit thì đập, vượt mép trái → thua hàng.
- `Shooter.cs` — Unit bắn khi có địch cùng hàng phía trước.
- `Projectile.cs` — đạn bay phải, trúng địch cùng hàng → gây damage.
- Đã xử lý lỗi **Input System** mới: hoặc đổi `Active Input Handling = Both`, hoặc dùng bản `PlacementController` viết theo `Mouse.current`.

---

### Unity — verify bản xám (xong)
- Chạy Play OK: enemy xuất hiện, click đặt được player xanh, bắn bullet vàng, enemy vượt hàng có log "THUA HÀNG".

### Kinh tế năng lượng + seed packet (✅ chạy được)
- Đã verify Play: chọn packet → đủ năng lượng mới đặt → trừ tiền + cooldown; ArcReactor sản năng lượng; đặt được cả ArcReactor lẫn Turret.
- Đã fix: chặn Instantiate prefab null; đẩy unit/enemy về z=-1 để không bị Tile che.

### Mặt trời nhặt được (đang ráp trong Editor)
- Đổi từ "cộng thẳng năng lượng" sang cơ chế PvZ thật: `EnergyOrb.cs` — mặt trời rơi/nở ra, click chuột để nhặt mới cộng năng lượng, không nhặt thì tự biến mất.
- `EnergySystem` thả mặt trời từ trời theo `skyInterval`; `EnergyProducer` (ArcReactor) nở mặt trời tại chỗ. Dùng chung `EnergySystem.SpawnOrb`.
- `PlacementController` ưu tiên nhặt mặt trời trước khi xét đặt unit.
- Đã verify nhặt được; fix mặt trời z=-2 để luôn nằm trước unit/enemy.

### Bước #4 — nhiều loại unit & địch (đang ráp trong Editor)
- **Lô cốt**: thuần wiring (prefab máu cao, không Shooter) — 0 code.
- **Súng băng**: `EnemyMover.ApplySlow`, `Projectile.slowFactor/slowDuration`, `Shooter.bulletSlowFactor/bulletSlowDuration`.
- **Địch giáp**: `EnemySpawner.enemyPrefabs[]` random nhiều loại địch (giữ `enemyPrefab` cũ làm mặc định).
- **Drone EMP**: `BombUnit.cs` — đếm fuse rồi nổ diện rộng theo bán kính, tự huỷ.
- Đã verify chạy ổn.

### Bước #5a — tuyến cứu cuối + thua (đang ráp trong Editor)
- `Lawnmower.cs` — 1 lawnmower/hàng, kích hoạt khi địch vượt tuyến, phóng phải huỷ sạch hàng, dùng 1 lần.
- `GameManager.cs` — sinh lawnmower mỗi hàng; xử lý THUA khi hết lawnmower; màn GAME OVER + nút "Chơi lại" (reload scene); `Time.timeScale = 0`.
- `EnemyMover` — vượt tuyến: gọi lawnmower trước, hết mới thua; cờ `caught` để đứng im chờ bị huỷ.

> **Roadmap đã tách**: #5 → #5a (lawnmower + THUA, đã làm) và #5b (điều kiện THẮNG, gộp vào #6 wave vì thắng = hết đợt cuối).

### Bước #6 — hệ thống wave + THẮNG (đang ráp trong Editor)
- `EnemySpawner` viết lại theo đợt: PreStart → Spawning → WaitingClear → BetweenWaves → Won. Mỗi đợt nhiều địch hơn, tỉ lệ địch giáp tăng, đợt cuối là huge wave. Hiển thị "Wave x/N" góc phải.
- `GameManager.Win()` + màn "YOU WIN!" + nút Chơi lại. Thua/Thắng loại trừ nhau.
- Field mới: `armoredPrefab` (gán ArmorEnemy), `waveCount`, `baseEnemies`, `startDelay`, `timeBetweenSpawns/Waves`, `finalWaveMultiplier`.

### Bước #7 — art skeletal (mới làm phần CODE-BRIDGE)
- `CharacterAnimator.cs` — lớp đệm null-safe: `SetWalking/TriggerAttack/TriggerDie/FaceLeft`. Chưa có Animator thì no-op.
- Đã hook: `EnemyMover` (walking/attack/face left), `Shooter` (attack khi bắn). Bản xám chạy y nguyên.
- **CÒN LẠI (thủ công, art thật):** sinh concept → cắt bộ phận → rig (2D Animation/DragonBones) → tạo clip idle/walk/attack/death → tạo Animator Controller (Bool "Walking", Trigger "Attack"/"Die") → gán vào CharacterAnimator trên prefab → thay sprite xám.

> 📎 Toàn bộ phong cách + prompt sinh sprite từng object: xem **`ART_STYLE.md`**.

#### Pipeline art đã chốt (làm từng nấc, 1 nhân vật trọn trước rồi nhân ra)
- Package đã đủ sẵn: 2d.animation 15.1, psdimporter 14.0, aseprite 5.0, particlesystem. KHÔNG cần cài thêm.
- **Quy ước:** canvas 256×256 nền trong; PPU=256; Pivot=Bottom Center; địch nhìn trái, unit nhìn phải.
- **GĐ1 — sprite tĩnh:** import → thay ô Sprite trên prefab. Game chạy nguyên, chỉ có hình.
- **GĐ2 — rig + anim (làm robot zombie/Enemy trước):** .psb tách layer → Skinning Editor (bones+weights) → clip Idle/Walk/Attack/Death → Animator Controller (Bool "Walking", Trigger "Attack"/"Die") → gắn CharacterAnimator (kéo Animator + SpriteRenderer).
- **GĐ2 — ĐÃ XONG cho con địch đầu tiên (alien robot):** rig 11 bones, Sprite Skin, geometry+weights; clip Idle(empty)/Walk/Attack/Death; Animator "Enemy 1" (Bool Walking, Trigger Attack/Die); CharacterAnimator gán animator + spriteToFlip. Walk/Attack/Death + Death code chạy đúng.
- **Bài học rig (áp cho mọi con sau):**
  - Sau Auto Geometry + Auto Weights phải bấm **Apply** trong Skinning Editor.
  - Animation tạo bằng cách xoay **bone transform ngoài scene** (không phải trong Skinning Editor); phải bật **Record/Preview** mới ghi/preview được.
  - Transition vào (Idle→Walk, AnyState→Attack/Death): **tắt Has Exit Time** + có condition. Attack→Idle: **bật Has Exit Time** ~0.9, không condition.
  - Clip Attack/Death **tắt Loop Time**. Clip Death **không keyframe Transform Position của root** (kẻo xác trôi).
  - Phải gán ô **animator** trong CharacterAnimator (trống = no-op im lặng).
  - **Art vẽ sẵn đúng hướng** (địch quay trái) → KHÔNG lật; đã bỏ FaceLeft trong EnemyMover.Awake.
- **GĐ3 — nhân quy trình** ra turret, arc reactor, bunker, snow gun, drone, địch giáp, lawnmower.
- **DamageStages.cs (mới):** đổi sprite theo % máu (Wall-nut nứt dần), generic cho mọi object KHÔNG rig (bunker…). `Health.Normalized` trả 0..1. KHÔNG dùng cho object có Sprite Skin. Đừng để Animator cũng tráo Sprite trên cùng object (xung đột).
- **Death code ĐÃ LÀM:** `Health` giờ phát trigger `Die` → tắt EnemyMover/Shooter (ngừng di chuyển/bắn, tự gỡ khỏi All) → chờ `deathAnimTime` rồi Destroy. Không có CharacterAnimator thì huỷ ngay như cũ. Field `deathAnimTime` chỉnh khớp độ dài clip Death.

### Bugfix animation/prefab — 2026-06-21
- **Triệu chứng:** Enemy runtime chỉ đứng `idle`, không chuyển `walk`; khi chết không thấy rõ `death`.
- **Nguyên nhân:** `CharacterAnimator` gọi Animator params `Walking`/`Attack`/`Die`, nhưng `Enemy.controller` đang dùng lowercase `walking`/`attack`/`die`. Animator parameter phân biệt hoa/thường nên transition không nhận tín hiệu.
- **Fix:** Chuẩn hoá `Enemy.controller` sang `Walking`, `Attack`, `Die`. `Enemy` và `ArmorEnemy` đều dùng chung controller này nên cùng được sửa.
- **Fix thêm:** `Unit.prefab` có Animator/SpriteSkin và `Unit.controller`, nhưng thiếu `CharacterAnimator`; đã thêm bridge để `Shooter`/`Health` trigger attack/death được.
- **Bunker:** dùng `DamageStages` theo phần trăm máu với 3 stage `bunker_1` → `bunker_2` → `bunker_3`; death effect nên tách thành VFX động, không dùng sprite nổ tĩnh.
- **Cần verify trong Unity:** Play Mode, enemy spawn từ wave phải `idle → walk`; khi HP về 0 phải vào `death` trước khi Destroy; turret attack/death trigger phải chạy nếu prefab có controller đúng.

- `EnergySystem.cs` — tổng năng lượng, tự rơi theo thời gian, hiển thị IMGUI góc trên-trái.
- `SeedBar.cs` — thanh chọn unit (mỗi loại có giá + cooldown), vẽ nút bằng IMGUI, chặn click đặt khi bấm trúng nút.
- `EnergyProducer.cs` — unit "Arc Reactor" định kỳ sản năng lượng (reskin Sunflower).
- `PlacementController.cs` — sửa lại: đặt unit ĐANG CHỌN, kiểm tra đủ năng lượng + hết cooldown, trừ tiền khi đặt.

## 🔧 ĐANG DỞ / CẦN HOÀN TẤT NGAY

- Tạo prefab **ArcReactor** (nhân bản Unit, bỏ Shooter, thêm EnergyProducer, đổi màu cho dễ phân biệt).
- Trên object GameSystems: thêm component `EnergySystem` + `SeedBar`; điền 2 seed (ArcReactor, Turret) với giá & cooldown.
- Bấm Play kiểm tra: chọn packet → đủ năng lượng mới đặt được → đặt xong bị trừ tiền + vào cooldown; ArcReactor làm năng lượng tăng dần.
- Cân bằng số: startEnergy, passiveInterval, giá & cooldown từng seed, amount/interval của ArcReactor.

---

## 🎯 MỤC TIÊU TIẾP THEO

Hoàn thiện **gameplay loop bằng hình xám** trước khi đụng tới art. Cụ thể là bổ sung phần kinh tế và UI chọn đơn vị để game "chơi được" đúng nghĩa PvZ.

---

## 📋 CÁC BƯỚC TIẾP THEO (theo thứ tự)

1. **Verify bản xám**: chạy ổn vòng lặp đặt–bắn–địch–thua trước khi thêm gì mới.
2. **Hệ thống năng lượng (sun/energy)**:
   - Năng lượng rơi tự nhiên theo thời gian + đơn vị "Arc Reactor" tự sản sinh.
   - Biến đếm tổng năng lượng + hiển thị UI.
3. **Seed packet + cooldown**:
   - Thanh chọn loại đơn vị, mỗi loại có giá và thời gian hồi.
   - Đặt đơn vị chỉ khi đủ năng lượng; trừ năng lượng khi đặt.
4. **Nhiều loại đơn vị & địch**: thêm lô cốt (tank), súng băng (làm chậm), drone EMP (nổ diện rộng); địch giáp đầu (máu cao).
5. **Tuyến cứu cuối + điều kiện thua/thắng**: laser cứu hàng 1 lần; hết wave → thắng màn.
6. **Hệ thống wave**: spawn theo đợt, độ khó tăng dần, có huge wave.
7. **Thay art skeletal**: sinh concept Iron Man → cắt bộ phận → rig → animation idle/walk/attack/death → thay khối xám.
8. **Polish**: particle đạn/nổ, âm thanh, UI menu, màn chơi.
   - Hoạt cảnh chuyển động (để dành, làm sau khi xong gameplay + art):
     - EnergyOrb: rơi có easing + lắc ngang nhẹ, pop scale khi nhặt.
     - Lawnmower: rung khi kích hoạt, vệt tốc độ, âm thanh.
     - Đạn/nổ: particle; screen shake nhỏ cho DroneEMP.
   - Lưu ý: chuyển động nằm gọn trong Update từng script nên thêm tween/particle sau không đụng logic.

**Nguyên tắc xuyên suốt:** gameplay chạy đúng bằng hình xám trước, art vào sau cùng.

---

## 📂 Danh sách file script hiện có
`Health.cs` · `GridManager.cs` · `PlacementController.cs` · `EnemySpawner.cs` · `EnemyMover.cs` · `Shooter.cs` · `Projectile.cs`
