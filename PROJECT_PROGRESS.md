# Game Tower Defense (PvZ Reskin) — Tiến độ dự án

**Chủ đề:** Game thủ thành phong cách sci-fi tương lai (KHÔNG gắn tên thương hiệu có bản quyền).
Phòng thủ: súng turret hiện đại + lô cốt bọc giáp + lõi năng lượng. Địch: **robot ngoài hành tinh (alien robot invader)** tiến từ phải sang trái.
> Lưu ý IP: tránh nêu thương hiệu thật ("Iron Man", "Plants vs Zombies"…) trong prompt/asset. Chỉ dùng làm cảm hứng, mô tả bằng từ chung chung (powered exo-armor, arc-reactor sci-fi).
**Engine:** Unity 2D. **Lưới:** 5 hàng × 9 cột. **Quy ước:** 1 ô = 1 unit.
**Game title:** Coreline Defense.
**Store listing name target:** Coreline Defense: Robot Siege.

## Working protocol — 2026-06-22
- Truoc moi feature lon, Codex phai:
  - phan tich boi canh hien tai;
  - neu cac phuong an kha thi va tradeoff;
  - chon phuong an toi uu theo codebase hien co;
  - cap nhat `.md` lien quan trong cung pass;
  - commit thanh checkpoint ro rang.
- Runtime player-facing UI/copy dung **English-first**.
- Vietnamese, Chinese, French se them sau bang localization table khi gameplay/menu flow on dinh.
- Khong them Vietnamese khong dau vao UI runtime nua.

## Production roadmap — 2026-06-22
- Phase 1: gameplay vertical slice — enemy variety, projectile effects/resistance, level 1-3 tuning, reward/unlock panel.
- Phase 2: visual/audio production — production board, UI skin/icons, VFX prefabs, animation polish, audio layering.
- Phase 3: campaign content — level 4-10, level select polish, tutorial callouts, unlock/reward copy.
- Phase 4: mobile/release hardening — safe area, device performance, pooling, build validation, Android/iOS dev builds, store assets.

## Naming — 2026-06-22
- Chot ten game runtime/product: **Coreline Defense**.
- Ten App Store / Google Play listing target: **Coreline Defense: Robot Siege**.
- Unity `PlayerSettings.productName` da doi sang `Coreline Defense`.
- Bundle identifier target hien tai: `com.duycaonguyen.corelinedefense`.

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

### Feature pass — muzzle, audio, Rail Cannon, Overcharge — 2026-06-21
- **MuzzlePoint:** `Shooter` bắn từ `muzzlePoint` nếu có, fallback về root nếu trống. `Unit.prefab` và `SnowGun.prefab` đã có child `MuzzlePoint` để chỉnh đúng đầu nòng trong Prefab/Scene view.
- **Audio:** thêm `AudioManager` null-safe với hooks cơ bản: shoot, hit, enemy death, unit break, energy collect, EMP, Rail Cannon, UI click, win/game over. Chưa gán clip thì game vẫn chạy im lặng.
- **Rail Cannon:** script cũ `Lawnmower` đổi hành vi thành rail cannon một phát quét hàng, giữ tên class để prefab cũ không bị missing script. Không còn xe chạy ngang kiểu PvZ.
- **Overcharge:** thêm `OverchargeSystem` trên `GameSystems`; tốn energy để buff một row trong thời gian ngắn, tăng fire rate và damage cho shooter cùng row.
- **Cần verify trong Unity:** chỉnh `MuzzlePoint` bằng mắt; gán audio clips vào `AudioManager`; test Rail Cannon khi enemy breach; test nút `OC`/phím số 1-5.

### Unity verify — 2026-06-22
- **Đã làm trong Unity:** chỉnh animation cho Unit; thêm rig cho SnowGun; thêm Animator cho SnowGun và dùng chung controller của Unit.
- **Đã test OK:** Rail Cannon hoạt động; Overcharge hoạt động và có trừ energy.
- **Cần tuning:** Overcharge hiện chưa làm tốc độ bắn tăng đủ rõ; cần tăng `fireRateMultiplier` hoặc giảm `energyCost`/tăng `duration` sau khi test thêm.
- **Audio trước khi import:** project chưa có audio asset; sau đó đã thêm Kenney SFX và gán clips trong `AudioManager`.
- **Audio bugfix:** clips trong `AudioManager` từng bị serialize với `volume/pitch = 0` nên im lặng; đã thêm default guard và chỉnh scene về volume 1.
- **Audio hooks thêm:** click seed packet, đặt unit thành công, enemy đánh unit/bunker theo interval.

### Audio + central balance config — 2026-06-22
- **Đã commit audio/gameplay polish:** Kenney SFX + license, `AudioManager` hooks, MuzzlePoint, Rail Cannon, Overcharge.
- **Central config:** thêm `GameBalance.cs` trên object `GameSystems` trong `SampleScene`.
- `GameBalance` hiện là nơi tune chính cho:
  - unit seed label/cost/cooldown/hp/death time;
  - shooter fire interval, bullet speed/damage/slow;
  - ArcReactor energy amount/interval;
  - DroneEMP fuse/radius/damage;
  - enemy hp/speed/attackDamage/attackSfxInterval/death time;
  - start energy, sky orb, wave setup, Overcharge.
- Runtime sẽ apply config khi game chạy: `SeedBar`, `PlacementController`, `EnemySpawner`, `EnergySystem`, `OverchargeSystem` đọc từ `GameBalance` nếu có.
- Prefab vẫn giữ default để không bị phụ thuộc cứng; khi Play Mode thì `GameBalance` thắng các số gameplay chính.
- **Cần verify trong Unity:** mở `GameSystems > GameBalance`, test 1-2 wave, chỉnh thử `overchargeFireRateMultiplier` hoặc `Turret.fireInterval` để xác nhận config đang có hiệu lực.
- **Runtime tuning fix:** chỉnh field trên `GameBalance` trong Play Mode sẽ apply lại cho `OverchargeSystem`, seed/energy/wave config, và các unit/enemy runtime có `BalanceIdentity`; máu hiện tại không bị refill khi live-tune.

### Combat feedback pass — 2026-06-22
- **Đang triển khai:** VFX tạm bằng code để tăng cảm giác va chạm trước khi có prefab VFX art riêng.
- Thêm `CombatVfx`: muzzle flash, hit spark, EMP pulse, death burst code-generated.
- Thêm `DamageFeedback`: object có `Health` tự flash/rung nhẹ khi nhận damage.
- Hook hiện có:
  - `Shooter.Fire()` -> muzzle flash tại `muzzlePoint`.
  - `Projectile` hit -> hit spark.
  - `BombUnit` explode -> EMP pulse + spark trên enemy trong vùng.
  - `Lawnmower`/Rail Cannon hit -> spark trên enemy trong row.
  - `Health` non-enemy death -> death burst cơ bản.
- **Cần verify trong Unity:** chờ script compile, Play test bắn/trúng/bunker bị đánh/EMP/Rail Cannon; xem có Console error hay VFX quá sáng/rối không.
- **Feedback bugfix:** enemy không còn shake transform khi bị bắn để tránh cảm giác bị knockback/giật X; placement chặn đặt unit vào ô đang có enemy.

### Roadmap expansion — 2026-06-22
- Bo sung roadmap ngoai 4 buoc gan nhat:
  - mobile UI/input pass;
  - tutorial/onboarding;
  - level/progression/save;
  - content expansion co vai tro ro;
  - performance/device optimization;
  - build/release pipeline;
  - store/compliance/post-launch readiness.
- Quyet dinh: knockback/freeze/stun se la projectile effects rieng sau nay, khong nam trong hit feedback mac dinh.

### Systems v1 — death VFX, projectile effects, balance report, AI QA — 2026-06-22
- Them `DeathEffect` optional va VFX code-generated cho static break/bunker break.
- Them `ProjectileHitEffect[]` voi effect type `Slow`, `Knockback`, `Stun`; slow cu van giu de tuong thich.
- Them `BalanceReportGenerator` de xuat metrics markdown/json tu `GameBalance`.
- Them `AiQaReportRunner` trong Editor:
  - menu `Tools > AI QA > Run Full Check`;
  - batchmode method `AiQaReportRunner.RunFullCheck`;
  - output vao `AIReports/latest_ai_qa_report.md` va `AIReports/latest_balance_metrics.json`.
- Them `AIReports/` vao `.gitignore`.

### Level/progression foundation v1 — 2026-06-22
- Them `LevelDefinition` ScriptableObject va `Assets/Levels/Level_01.asset`.
- Them `LevelManager` tren `GameSystems`, apply level balance vao `GameBalance` luc Awake.
- Them `PlayerProgress` dung PlayerPrefs; `GameManager.Win()` mark current level complete.
- AI QA v1 check them `LevelManager.currentLevel`.
- Them authored wave data vao `LevelDefinition` va `Level_01.asset`; hien tai `useAuthoredWaves` dang tat de giu gameplay formula cu cho an toan.

### Mobile UI/settings/progression UI v1 — 2026-06-22
- Bat `Level_01.useAuthoredWaves` mac dinh sau khi playtest OK.
- Them `GameSettings` luu SFX volume, reduce shake, vibration bang PlayerPrefs.
- Them `GameUiController` tren `GameSystems`:
  - nut pause/resume;
  - panel settings tam thoi;
  - hien current level va highest completed;
  - chan click UI xuyen xuong placement.
- `AudioManager` nhan `GameSettings.SfxVolume`.
- `DamageFeedback` nhan `GameSettings.ReduceShake`.
- AI QA check them `GameUiController`; balance report hien authored wave summary.

### Runtime HUD rebuild — uGUI/TextMeshPro — 2026-06-22
- Quyet dinh: dung **uGUI Canvas + CanvasScaler + TextMeshPro** cho runtime HUD thay cho IMGUI. UI Toolkit de danh cho editor/tooling sau nay, khong dung lam gameplay HUD chinh luc nay.
- `GameUiController` duoc rebuild thanh runtime HUD builder:
  - top status bar: energy, level name, wave status, pause button;
  - seed tray nam phia duoi, co selected/disabled/cooldown overlay;
  - overcharge panel tach ben phai, khong con de len pause/wave;
  - modal pause/win/lose gom SFX slider, reduce shake, vibration, resume, restart.
- `EnergySystem`, `SeedBar`, `OverchargeSystem`, `EnemySpawner`, `GameManager` van giu `OnGUI` nhung chi chay khi bat `showDebugImGui`; mac dinh tat de khong con UI chong cheo.
- `SeedBar`, `OverchargeSystem`, `EnemySpawner` expose API nho de HUD doc state va goi action thay vi tu ve UI trong tung script.
- `GameUiController` tu tao `EventSystem` neu scene chua co, va `PlacementController` van hoi `PointerOverPanel` de chan click UI xuyen xuong board.
- **Unity verify:** chu project da test cac chuc nang chinh hoat dong binh thuong: HUD hien dung, seed tray/OC/pause/settings co ban chay duoc.
- TextMeshPro Essentials da duoc Unity import sau khi mo project. Khong can import `Examples & Extras` vao ban release.

### Production foundation pass — 2026-06-22
- Xoa `Assets/TextMesh Pro/Examples & Extras` thua sau khi import TMP Essentials; chi giu TMP runtime essentials.
- Them `ObjectPooler` + `PooledObject`, da ap dung cho projectile va energy orb de giam Instantiate/Destroy lap lai.
- Them `CombatVfxSettings` tren `GameSystems`: co the gan prefab VFX that cho muzzle, hit, death, static break, bunker break, EMP pulse, Rail Cannon beam; neu chua gan thi fallback code-generated van chay.
- Them `TutorialCoach` tren `GameSystems`, HUD co the hien hint nhe neu coach enabled.
- Them `RuntimeQualitySettings` tren `GameSystems`: target 60 FPS, tat v-sync runtime, khong sleep man hinh, tat multitouch neu khong can.
- Them `LevelCatalog.asset` va gan vao `LevelManager`; LevelManager co API `NextLevel`, `HasNextLevel`, `SelectLevel`.
- AI QA kiem tra them VFX settings, TutorialCoach, RuntimeQualitySettings, LevelCatalog.
- Gioi han con lai: cac task "game xịn App Store" can asset/UI/VFX/audio art that; code foundation khong thay the duoc visual production.

### Board visual polish pass — 2026-06-22
- Them `BoardVisualController` tren `GridManager` de tao board/backdrop runtime bang SpriteRenderer rectangles:
  - nen scene toi hon;
  - board base sci-fi;
  - lane bands xen ke;
  - grid lines;
  - defense rail ben trai va enemy entry zone ben phai;
  - neon frame/caps tam thoi.
- Chinh `Tile.prefab` thanh cell tint trong suot hon va sorting order thap hon de board art doc duoc.
- Doi camera background sang mau toi hop voi HUD moi.
- Luu y: day la visual foundation/procedural placeholder, chua phai final App Store art. Khi co background/board sprite that, co the thay `BoardVisualController` bang prefab art hoac dung no lam guide layout.

### Mission progression loop v1 — 2026-06-22
- Them 2 level data moi: `Level_02` ("Armor Probe") va `Level_03` ("Signal Siege") voi authored waves rieng.
- `LevelCatalog.asset` hien co 3 level theo thu tu 1 -> 3.
- `PlayerProgress` luu:
  - level da hoan thanh;
  - highest completed level;
  - selected level hien tai.
- `LevelManager` doc selected level khi scene load, chi cho chon level da unlock, reload scene sach khi doi level.
- `GameUiController` them:
  - nut `LVL` tren top bar;
  - overlay chon nhiem vu;
  - nut `MAN TIEP` trong modal win khi level tiep theo da unlock;
  - trang thai level: dang choi, da xong, mo khoa, khoa.
- AI QA kiem tra them catalog: slot null, trung `levelId`, sai thu tu `levelNumber`, authored waves rong.
- Muc tieu: chuyen game tu single scene prototype sang loop co progression co ban, van giu mot scene duy nhat de it rui ro asset/scene management.
- **Unity verify:** chu project da test 7 buoc progression loop, tat ca hoat dong OK.

### Mission unlock/gating v1 — 2026-06-22
- `LevelDefinition` co them:
  - `allowedUnitLabels`;
  - `overchargeUnlocked`.
- `SeedBar` giu seed list goc va filter active seeds theo level hien tai.
- Unlock hien tai:
  - Level 1 / First Contact: `ArcReactor`, `Turret`, khong OC.
  - Level 2 / Armor Probe: them `Bunker`, khong OC.
  - Level 3 / Signal Siege: them `SnowGun`, `DroneEMP`, bat OC.
- `OverchargeSystem` co state `unlocked`; khi khoa se an panel OC, chan phim so, clear timers va khong chan click board.
- `TutorialCoach` khong goi y OC neu level chua mo OC; level co Bunker se goi y dung Bunker khi bi ep.
- AI QA kiem tra `allowedUnitLabels` co khop voi `GameBalance.units`.

### Phase 1 slice — enemy traits/resistance foundation — 2026-06-22
- Them `EnemyTraits` runtime component:
  - projectile damage multiplier;
  - EMP damage multiplier;
  - slow effect multiplier;
  - knockback multiplier;
  - stun duration multiplier.
- `GameBalance.EnemyBalance` la source tune trait; `ApplyEnemy` tu add/config `EnemyTraits` khi enemy spawn.
- `Projectile` dung projectile damage multiplier truoc khi gay damage.
- `BombUnit` dung EMP damage multiplier.
- `EnemyMover.ApplySlow/ApplyKnockback/ApplyStun` ton trong trait resistance.
- ArmorEnemy hien duoc tune thanh:
  - nhan projectile damage x0.85;
  - nhan EMP damage x1.25;
  - slow x0.65;
  - knockback x0.35;
  - stun x0.75.
- `BalanceReportGenerator` tinh TTK theo projectile multiplier va in enemy trait summary.
- AI QA bao loi neu enemy trait multiplier am.
- Muc tieu: tao nen counter-play cho SnowGun/EMP/OC truoc khi them enemy variant moi nhu Fast/Shield/Heavy.

### UI scale + early pacing pass — 2026-06-22
- Fix TMP warning: thay `enableWordWrapping` bang `textWrappingMode`.
- Runtime HUD duoc scale lon hon:
  - top bar cao hon, Energy/mission/wave text lon hon;
  - mission/pause button lon hon;
  - seed tray rong va cao hon, seed cards lon hon;
  - OC panel va row buttons lon hon;
  - tutorial hint panel lon hon.
- Muc tieu UI: bot cam giac cac nut la overlay nho tach roi, gan hon voi game surface. Day van la code-generated UI, chua phai final UI skin.
- Level pacing sau unlock gating:
  - Level 1 start delay tang, wave dau it enemy hon, spawn interval cham hon.
  - Level 2 start delay/spawn interval duoc noi de nguoi choi kip dat Bunker/economy.
  - Level 3 van kho hon nhung wave dau cham hon de kip dung full toolkit.
- Ly do: sau khi level 1/2 khoa bot unit va OC, authored waves cu tro nen de thuong nha qua som.

### Board framing + adaptive command deck — 2026-06-22
- Phong to board/cell tren man hinh bang camera framing:
  - camera orthographic size `4` -> `3.6`;
  - khong doi `GridManager.cellSize` de tranh anh huong placement/spawn/projectile math.
- Seed command deck tu co theo so unit da unlock:
  - level 1/2 khong con tray rong thua qua nhieu khi chi co 2-3 seed;
  - level 3 van du cho 5 seed.
- `HorizontalLayoutGroup.childForceExpandWidth` tat de seed cards giu kich thuoc thiet ke, khong bi keo gian bat thuong.
- Muc tieu: board nhin full-screen hon, command UI gon hon va gan voi game surface hon.

### Mission reward/unlock copy v1 — 2026-06-22
- Them `missionBriefing` va `completionReward` vao `LevelDefinition`.
- `Level_01`, `Level_02`, `Level_03` da co English-first briefing/reward copy:
  - Level 1 reward: Bunker.
  - Level 2 reward: SnowGun, DroneEMP, Overcharge.
  - Level 3 reward: sector secured / more missions later.
- Level select hien mo ta ngan theo tung mission; mission bi khoa hien yeu cau clear mission truoc.
- Victory modal hien reward/unlock copy; Pause modal van la noi duy nhat hien settings SFX/reduce shake/vibration de tranh modal ket qua bi roi.
- AI QA canh bao level moi neu thieu mission briefing hoac completion reward copy.

### Progress save hardening v1 — 2026-06-22
- `PlayerProgress` van dung PlayerPrefs de giu don gian cho vertical slice, nhung da co `progress.saveVersion`.
- Them metadata progression:
  - `CompletedCount`;
  - `LastCompletedLevelId`;
  - `SaveVersion`.
- Save cu migrate `CompletedCount` tu `HighestCompletedLevel` de giu hop ly voi campaign tuyen tinh hien tai.
- `MarkLevelCompleted` khong tang duplicate count neu replay level da clear.
- Muc tieu: chuan bi cho campaign/unlock/reward sau nay ma khong phai reset progress nguoi choi khi save schema lon hon.

### AI QA release-readiness expansion — 2026-06-22
- `AiQaReportRunner` ngoai gameplay/prefab checks nay kiem tra them:
  - Build Settings co enabled scene va co `Assets/Scenes/SampleScene.unity`;
  - `PlayerSettings.productName`/`bundleVersion` co gia tri hop le toi thieu;
  - tat ca `LevelDefinition` assets trong `Assets/Levels` co id khong trung;
  - level asset nam ngoai `LevelCatalog` se canh bao.
- Muc tieu: giam loi cau hinh khi bat dau build Android/iOS dev build.

### Mobile safe-area HUD foundation — 2026-06-22
- Them `SafeAreaFitter` de fit RectTransform vao `Screen.safeArea`.
- `GameUiController` tao `SafeAreaRoot` duoi `RuntimeHUD` Canvas va dung root nay cho top bar, seed tray, OC panel, tutorial, modal, level select.
- Muc tieu: tranh HUD bi notch/cutout che khi test iOS/Android ma khong thay doi toa do gameplay world.
- Can test sau tren Game view aspect co notch/safe-area hoac tren device that.

### Wave status clarity pass — 2026-06-22
- `EnemySpawner.DisplayText` hien ro phase:
  - `Deploy: Ns` truoc wave dau;
  - `Wave X/Y` khi dang spawn;
  - `Clear wave X/Y` khi cho diet sach enemy;
  - `Next wave: Ns` giua cac wave.
- HUD doi mau warning cho wave countdown khi con 3 giay, giup nguoi choi nhan biet dot sap toi.

### Combat end-state + tutorial reliability pass — 2026-06-22
- Tutorial hint khong con chan click gameplay:
  - `TutorialHint` panel tat `raycastTarget`;
  - `TutorialCoach` chi hien hint trong thoi gian ngan bang `hintDisplayDuration`, khong bam man hinh vinh vien.
- Victory timing fix:
  - `EnemyMover.All` tiep tuc la danh sach enemy con song/co the bi target;
  - them `EnemyMover.ActiveOrDyingCount` de dem enemy con ton tai ke ca dang death animation;
  - `EnemySpawner` chi goi `GameManager.Win()` khi `ActiveOrDyingCount == 0`, tuc la enemy da destroy xong sau death flow/VFX delay.
- AI QA canh bao neu `TutorialCoach.hintDisplayDuration <= 0`.

### Compact HUD + modal layout pass — 2026-06-22
- Giam footprint runtime HUD de mo rong cam giac ban choi:
  - top status bar `116px` -> `84px`;
  - seed tray `148px` -> `110px`;
  - seed card `226x112` -> `188x86`;
  - camera orthographic size `3.6` -> `3.35`.
- Energy, mission name, wave status duoc dua vao status box co outline nhe; Energy font giam de bot ap dao.
- Seed command deck nho hon, co frame cho tung card, van giu touch target du lon.
- Overcharge panel gon hon va row buttons nho hon.
- Pause/Victory/Defeat modal compact hon:
  - card nho hon, co outline;
  - settings chi o Pause;
  - action buttons tu layout theo state, tranh lech hang/nut roi rac.
- Level select popup co outline/list frame va typography gon hon.
- Luu y: day van la runtime generated UI polish. Final release van can UI sprite/icon/font skin rieng de dat muc App Store/Google Play.

### Enemy variety + campaign extension v1 — 2026-06-22
- Them enemy archetype moi trong `LevelEnemyType`:
  - `Fast`: mau thap hon, di nhanh hon, yeu hon khi tan cong, nhay cam hon voi slow/EMP.
  - `Shield`: mau cao hon, cham hon, resist bullet/control, vulnerable hon voi EMP.
- `EnemySpawner` build authored wave theo enemy type queue, map prefab tu type, sau do apply type modifiers sau `GameBalance`.
- `fastPrefab`/`shieldPrefab` optional; hien tai co the fallback ve `Enemy`/`ArmorEnemy` de gameplay chay duoc truoc khi co art/prefab rieng.
- Them `Level_04` ("Velocity Breach") gioi thieu Fast enemy.
- Them `Level_05` ("Shield Column") gioi thieu Shield enemy.
- `LevelCatalog` nay co level 1-5.

### Mission select dev unlock + QA tuning checks — 2026-06-23
- Sua level select popup thanh `ScrollRect`/viewport/content dung uGUI de list 5+ mission khong tran khoi card.
- Them `LevelManager.unlockAllLevelsForTesting`; scene hien dang bat flag nay de test nhanh tat ca level ma khong ghi gia progress completed vao `PlayerPrefs`.
- Production unlock rule van giu trong `PlayerProgress.IsLevelUnlocked`; khi gan release co the tat test flag de quay lai unlock tuan tu.
- Chuan hoa Fast/Shield tuning vao `EnemySpawner.GetTypeModifier`, de runtime va QA report dung cung mot source so lieu.
- AI QA check them:
  - authored waves co group Fast/Shield khi level pack da co 4-5 mission;
  - Fast phai health thap hon + speed cao hon baseline;
  - Shield phai health cao hon + speed thap hon + resist projectile + vulnerable EMP.
- Balance report hien `Enemy Type Tuning` de Codex kiem soat stat identity bang so lieu, khong bat chu project do bang cam giac.

### Projectile effect content pass — 2026-06-23
- Fix QA warning Level_05 completion reward bang cach quote YAML reward co dau `:`.
- `Shooter` co `ProjectileHitEffect[] hitEffects`; khi ban se clone effect vao projectile de pooled bullet khong giu state cu.
- `GameBalance.UnitBalance` co `projectileHitEffects` de tune effect theo unit tu config trung tam.
- SnowGun chuyen tu legacy `bulletSlowFactor/bulletSlowDuration` sang `ProjectileHitEffect Slow(value=0.5, duration=3)` trong scene balance va prefab.
- Legacy slow fields van ton tai de khong pha prefab cu, nhung content moi nen dung effect list.
- AI QA check SnowGun phai co Slow ProjectileHitEffect; balance report hien projectile effects cua shooter.

### Campaign pack 1 data baseline — 2026-06-23
- Them `Level_06` den `Level_10` ScriptableObject YAML va meta.
- `LevelCatalog` nay co level 1-10.
- Level 6-10 dung enemy type hien co (`Basic`, `Armored`, `Fast`, `Shield`) va authored waves rieng.
- Chu dich thiet ke:
  - Level 6: EMP vs shield-heavy waves.
  - Level 7: SnowGun vs fast-heavy pressure.
  - Level 8: economy vs mixed heavy waves.
  - Level 9: bunker/patching weak lanes vs staggered breaches.
  - Level 10: final mixed formation baseline.
- Chua them art/enemy moi; day la data baseline de test pacing, fairness, va campaign length.

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
