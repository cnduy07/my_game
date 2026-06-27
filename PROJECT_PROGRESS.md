# Coreline Defense — Tiến độ dự án

**Chủ đề:** Game thủ thành phong cách sci-fi tương lai (KHÔNG gắn tên thương hiệu có bản quyền).
Phòng thủ: súng turret hiện đại + lô cốt bọc giáp + lõi năng lượng. Địch: **robot ngoài hành tinh (alien robot invader)** tiến từ phải sang trái.
> Lưu ý IP: tránh nêu thương hiệu thật ("Iron Man", "Plants vs Zombies"…) trong prompt/asset. Chỉ dùng làm cảm hứng, mô tả bằng từ chung chung (powered exo-armor, arc-reactor sci-fi).
**Engine:** Unity 2D. **Lưới:** 5 hàng × 9 cột. **Quy ước:** 1 ô = 1 unit.
**Game title:** Coreline Defense.
**Store listing name target:** Coreline Defense: Robot Siege.

## Project protocol snapshot — 2026-06-22
- Workflow source-of-truth hien tai nam trong `PROJECT_CONTEXT.md`; file nay chi ghi lai lich su tien do/quyet dinh.
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

## Strict UI spec correction - 2026-06-24
- Read `ui_spec_generator.html` and synced the enforceable runtime rules into `UI_SPEC.md`.
- Initial strict-spec pass used a square legacy button; later UI direction superseded it with `Assets/UI/button_command.png`, and the legacy square button asset has been removed.
- Added `UiSpec.cs` for the exact palette, button sizes, and button state colors from the spec.
- Added `PixelButtonPressOffset.cs` for the required pressed 2px down feedback.
- Frontend and GameScene UI factories use the shared command button skin with ColorBlock tint states and no code-drawn button frames/notches/stripes.
- Main menu runtime path now follows the spec layout: left title/status only, right `COMMAND` panel, 4 vertical buttons `START GAME`, `SETTING`, `HOW TO PLAY`, `EXIT`, 240x60 with 12px gap.
- Victory/Defeat modal path is constrained to the spec-style 640x420 panel and 3 popup buttons `RESTART`, `MISSION`, `MAIN MENU` at 180x50; the next-mission button is hidden in terminal modal state.
- Fixed `Assets/Prefabs/Unit 1.controller` by adding the missing `Walking` bool parameter used by the `turret_attack` transition.
- Verify: `dotnet build Assembly-CSharp.csproj` pass; `dotnet build Assembly-CSharp-Editor.csproj` pass.

## UI sizing/readability correction - 2026-06-25
- Fixed main menu button sizing: menu VerticalLayoutGroups now control preferred width/height, so buttons render at fixed `UI_SPEC` size instead of falling back to square `100x100` defaults.
- Tightened main menu composition by reducing the empty COMMAND panel footprint and making title/status placement more deliberate while keeping the left/right spec layout.
- Reworked Settings scene audio sliders: labels/value text are separated from the slider track, slider track uses a simple color panel, and handle size is reduced so it no longer overlaps Music/SFX text.
- Mission map deploy buttons in frontend and in-game mission overlay no longer stretch full panel width; they are fixed-size and centered.
- Victory/Defeat modal reduced excess vertical empty space, increased result/stat text readability, and keeps the three terminal action buttons at fixed 180x50.
- Seed tray selected item remains readable when the player lacks energy: label/icon stay bright and the cost uses the danger accent.
- Board grid overlay is stronger and slightly thicker so the true gameplay cells read over generated board art even when the background image cell art does not perfectly align.
- Verify: `dotnet build Assembly-CSharp.csproj` pass; `dotnet build Assembly-CSharp-Editor.csproj` pass.

## Command button and UI motion pass - 2026-06-25
- Added new canonical button asset `Assets/UI/button_command.png` as a horizontal pixel-art sci-fi metal command button; the old square legacy asset has been removed.
- Updated all frontend/game scene `buttonSprite` references to the new `button_command` GUID.
- Updated `UI_SPEC.md`: menu buttons `220x72`, popup buttons `172x58`, secondary/deploy/back buttons `188x58`, with Point filter and tint states preserved.
- Added reusable UI motion components: hover/touch scale, title flicker, diagonal ambient pixel rain, deploy scene transition, and game stats tracking.
- Main menu title is larger and flickers subtly like unstable attack lighting; frontend scenes now have subtle diagonal dark pixel rain/debris.
- Mission map nodes now have stronger hover/touch scale and selected pulse; `DEPLOY` plays a short tactical transition before loading/reloading gameplay.
- Victory/Defeat terminal modal now uses a mission report layout with icon-chip rows for mission, wave reached, enemies killed, energy collected, and play time.

## Generated art import audit - 2026-06-24
- Chu project da them nhieu generated PNG vao `Assets/Art`.
- Audit phat hien sprite moi chua co `.meta` va mot so runtime sprite cu dang bi delete: `bullet.png`, `bunker_1/2/3.png`, `droneemp.png`, `energyorb.png`, `lawnmower.png`.
- Cac prefab `Bullet`, `Bunker`, `DroneEMP`, `EnergyOrb`, `Lawnmower` dang tham chieu GUID cua nhung file cu nay, nen se missing sprite cho den khi map sang asset moi.
- Chon huong an toan: khong sua YAML/GUID thu cong; them Editor tool `Tools > Art > Apply Generated Sprites To Prefabs` de Unity import PNG, tao `.meta`, set TextureImporter Sprite/Point/Uncompressed/PPU theo canh anh, roi gan sprite moi vao prefab bang AssetDatabase/PrefabUtility.
- Tool da chay thanh cong bang Unity batchmode, map cac prefab dang co missing sprite risk: Bullet -> `sprite_bullet_turret_256`, Bunker -> `sprite_bunker_stage_1/2/3_256`, DroneEMP -> `sprite_drone_emp_256`, EnergyOrb -> `sprite_energy_orb_256`, Lawnmower -> `sprite_rail_cannon_256`.
- AI QA sau generated art import: `0` fail, `1` expected warn (`LevelManager.unlockAllLevelsForTesting`).
- Static preview pass tiep theo da map them `Unit` -> `sprite_turret_256`, `SnowGun` -> `sprite_snowgun_256`, `Enemy` -> `sprite_enemy_basic_base_256`, `ArmorEnemy` -> `sprite_enemy_armored_256`.
- Tao `FrostProjectile.prefab` tu `Bullet.prefab`, gan `sprite_projectile_frost_256`, va doi `SnowGun.prefab` sang dung projectile băng rieng.
- Tao them `FastEnemy.prefab` va `ShieldEnemy.prefab` tu prefab enemy hien co, gan `sprite_enemy_fast_256`/`sprite_enemy_shield_256`, roi wire vao `GameScene` va `SampleScene`.
- Cac prefab co `SpriteSkin` dang tam tat SpriteSkin vi PNG moi la static preview, chua co rig/bone/weights. Khi co asset rig final, bat lai SpriteSkin/Animator theo pipeline 2D Animation.
- `EnemySpawner` chi ve badge fallback cho Armored/Fast/Shield khi prefab variant bi thieu; khi variant prefab da gan thi dung sprite rieng va khong them badge rui.
- UI/background/VFX PNG moi (`menu_hero`, official `board_coreline_combat_grid_5x9`, `button_command`, `ui_panel`, seed `icon_*`, `vfx_*`) da wire vao runtime UI/board/VFX qua `GeneratedArtApplier`, scene references, va sprite VFX prefab rebuild.
- Batchmode art apply da rerun thanh cong; AI QA nen chay lai sau Play Mode smoke test de bat regression runtime.

## Generated UI/board/VFX integration - 2026-06-24
- `FrontendUiController` va `GameUiController` co serialized sprite refs cho `menuHeroSprite`, `boardBackgroundSprite`, `buttonSprite`, `panelSprite`; UI factories dung button/panel sprite skin thay rectangle thuan.
- `GameUiController` seed cards hien icon: ArcReactor/Bunker/DroneEMP dung `icon_*`, Turret/SnowGun tam dung gameplay sprite khi chua co icon rieng.
- `BoardVisualController` co `boardBackgroundSprite`; khi co generated board art thi dung sprite art lam board base va giu overlay grid/rail/spawn markers de gameplay van doc ro.
- `VfxPrefabBuilder` rebuild `Assets/Prefabs/VFX/*` bang generated `vfx_*_256.png` thanh SpriteRenderer prefab co fade/scale qua `VfxAutoDestroy`; fallback particle van giu neu thieu sprite.
- `GeneratedArtApplier.ApplyGeneratedSpritesToPrefabs` hien la tool tong: import PNG, map gameplay prefab sprites, assign scene UI/board refs, rebuild VFX prefabs, va gan VFX settings cho `GameScene`/`SampleScene`.
- Verify: `dotnet build Assembly-CSharp.csproj --no-restore` pass; `dotnet build Assembly-CSharp-Editor.csproj --no-restore` pass; Unity batch art apply log pass voi `Generated art apply complete. Updated 19 prefab sprite mapping(s)`.

## VFX/result modal tuning - 2026-06-24
- Issues tu Play Mode screenshot: board art dang tranh doc voi gameplay, muzzle flash qua lon, energy collect/EMP/death VFX con nen vuong toi, Victory/Defeat modal qua phang.
- Fix da lam: clean alpha cho cac PNG VFX chinh (`vfx_muzzle_flash_256`, `vfx_emp_pulse_256`, `vfx_enemy_death_burst_256`, `vfx_hit_spark_256`, `vfx_rail_beam_source_256`) de loai dark square artifact khi Unity render sprite.
- `VfxPrefabBuilder` da giam scale/timing/alpha cho muzzle, EMP, enemy death va bunker break; `GeneratedArtApplier` da rebuild prefab VFX va apply vao `GameScene`/`SampleScene`.
- `CombatVfx.PlayMuzzleFlash` spawn tai `muzzlePoint` va offset nhe sang phai de flash nam o dau nong; `PlayEnergyCollect` dung pulse nho hon va particle ngan hon.
- `GameUiController` victory/defeat modal luc dau co result glow/sweep/core panel; pass sau da thay bang dark neutral panel va bo terminal backdrop toan man hinh.
- `BoardVisualController.boardBackgroundOpacity` hien tai la `1.0` cho `Assets/Art/board_coreline_combat_grid_5x9.png`; overlay grid that cua gameplay se dam nhiem readability.
- Correction: board GameScene hien tai dung official board `Assets/Art/board_coreline_combat_grid_5x9.png` tu ban PTS edit full opacity. `GeneratedArtApplier` khong tu ghi de board runtime khi chua duoc duyet.
- Verify: `dotnet build Assembly-CSharp.csproj` pass; `dotnet build Assembly-CSharp-Editor.csproj` pass; Unity batch art apply pass; AI QA `0` fail, `1` expected warn (`LevelManager.unlockAllLevelsForTesting`).

## Runtime board/UI/VFX feedback pass - 2026-06-24
- Board variant test da dong: official runtime board la `Assets/Art/board_coreline_combat_grid_5x9.png`; cac file test board tam va board swap editor tool da duoc remove de tranh ap nham.
- Sau feedback Play Mode, energy collect VFX va DroneEMP/EMP pulse bi qua be nen `CombatVfx` da tang lai sprite pulse scale/particle count/lifetime o muc vua phai.
- Bottom seed tray doi tu card/button app-like sang dark command slots: card nho gon hon, icon bay rieng, selected glow/strip thay vi full cyan background/text den.
- Frontend/main-menu buttons va in-game generated buttons dung shared `button_command` asset skin; mission map selected node, deploy button va route line duoc lam bot phang/cung.
- Verify: `dotnet build Assembly-CSharp.csproj` pass; `dotnet build Assembly-CSharp-Editor.csproj` pass; `git diff --check` pass. AI QA chua rerun vi Unity Editor dang duoc dung de visual test.

## End modal/button/pacing correction - 2026-06-24
- Victory/Defeat modal bo terminal backdrop xanh/do toan man hinh, tat result glow/sweep/core rectangle va dung dark panel trung tinh voi title/result accent nhe.
- GameScene/Frontend button factories apply `button_command` cho runtime buttons; `AddButtonAccent` khong con tao notch/stripe/line rectangle tren button.
- Seed select card dung `button_command` lam background de dong bo voi shared button skin, giu icon bay/selected glow o muc nhe.
- Enemy pacing: base speed trong `GameBalance`, enemy prefabs va `GameScene`/`SampleScene` giam tu `0.3` xuong `0.27`; Fast enemy speed modifier giam tu `1.55x` xuong `1.28x`.
- Verify: `dotnet build Assembly-CSharp.csproj` pass; `dotnet build Assembly-CSharp-Editor.csproj` pass; `git diff --check` pass. Play Mode/AI QA trong Unity can confirm end modal/button va Level 4 pacing.

## Asset cleanup and board naming - 2026-06-25
- Renamed the approved runtime board art to `Assets/Art/board_coreline_combat_grid_5x9.png` while preserving its Unity GUID, so scene/prefab references stay stable.
- Removed temporary board test/reference assets and the board swap editor helper; board variant testing is closed unless a new approved board candidate is introduced deliberately.
- Removed unused legacy sprite PNGs whose GUIDs no longer had references after generated-art mapping: old bullet, bunker stages, drone EMP, energy orb, lawnmower, basic enemy variants, snowgun, turret, and unused reference art.
- Removed the old reference-image folder and the legacy square UI button asset. Runtime UI source of truth remains `Assets/UI/button_command.png`.
- Updated `ASSET_GENERATION_PROMPTS.md`, `CONTENT_PLAN.md`, `PROJECT_CONTEXT.md`, `PROJECT_PROGRESS.md`, and `TASKS.md` so future work points at the official board/button assets instead of stale test/reference names.
- Verify: remaining PNG assets all have runtime/editor references or explicit tool references; deleted PNG GUIDs have `0` scene/prefab refs; `dotnet build Assembly-CSharp.csproj` pass; `dotnet build Assembly-CSharp-Editor.csproj` pass.

## GameScene underlay and end scene pass - 2026-06-25
- `BoardVisualController` now builds a larger hangar/command-deck underlay behind the board so the black zone around the board has dim panels, service bays, cables, signal lamps, and side bays instead of flat empty black.
- GameScene/SampleScene board backdrop padding increased to match the new underlay.
- Enemy entry/right rail procedural red shapes were reduced to subtle gate glow and small signal ticks; the active rail/last-defense identity should come from sprites, with code only providing restrained background cues.
- Victory/Defeat terminal state now uses a full-screen end scene backdrop with dim board art, vignette, scan sweep, ambient debris, large pulsing result title, mission status subtitle, and stat report rows with icon chips.
- End-state action buttons remain fixed-size `button_command` buttons on the report panel.
- Verify: `dotnet build Assembly-CSharp.csproj` pass; `dotnet build Assembly-CSharp-Editor.csproj` pass.

## Screenshot feedback pass - 2026-06-26
- Added generated `Assets/Art/coreline_defense_logo_1024.png` game logo at exact 1024x1024 with Unity Sprite importer metadata.
- Configured user-added `Assets/Art/logo.png` as a single point-filtered Sprite and kept `SnowGun` firing `FrostProjectile` with `sprite_projectile_frost_256.png` without the copied Bullet animator tint.
- Widened GameScene/SampleScene board backdrop padding and expanded `BoardVisualController` hangar side panels, deck ribs, side lights, and far wall layers so wide phone aspect ratios no longer expose flat black zones outside the board.
- Raised small text in GameScene HUD/status panels, pause settings popup, pause action buttons, Settings scene audio/comfort labels, Mission Map detail copy, mission nodes, and the in-game mission overlay.
- `AudioManager` now bootstraps before scene load, persists across scenes, loads `hidden_labs` from `Assets/Resources/Audio/Music`, and absorbs GameScene serialized SFX clips without destroying the shared `GameSystems` object.
- Follow-up readability pass added centralized font scaling for generated GameScene/frontend/end-scene labels and scene-authored frontend UI, excluding the hand-tuned main menu scene. Labels below title size now use a 22px mobile-readable floor with stronger scaling, while autosizing can shrink to 65% for tight buttons/nodes.
- Overflow correction split fixed-size button text from general readable text scaling, capping long button labels like `HOW TO PLAY` and `RESTART SECTOR` and adding wider label insets inside command buttons.
- Replaced rejected procedural explosion/rocket flash strips with `Assets/Art/airplane_256.png` patrol sprites flying around the GameScene board underlay/black-zone edges.

## GameScene visual correction pass - 2026-06-25
- Board underlay contrast increased and extra outer catwalk/machinery blocks were added above, below, left, and right of the board so the outer black zone reads as a dim hangar rather than empty background.
- Rail cannon laser beam restored to the stronger previous red laser/prefab style after review; only the static board-side red entry shapes stay toned down.
- End scene title/subtitle moved lower for better centering, title pulse scale added, report panel narrowed/centered, and report rows now have backplates plus detailed icon chips instead of plain single-letter boxes.
- Verify: `dotnet build Assembly-CSharp.csproj` pass; `dotnet build Assembly-CSharp-Editor.csproj` pass.

## Responsive UI and dedicated EndScene pass - 2026-06-25
- Fixed authored frontend ambient FX rebuild so MainMenu bullet rain uses moving runtime streaks instead of static baked children.
- MissionMap detail panel now uses compact recommended-tools text and tighter anchored text/button bands to avoid iOS simulator text overlap.
- GameScene now fits the world camera and HUD layout to tablet/4:3 aspect, reducing board cropping and keeping the OC panel outside the board.
- Added real `Assets/Scenes/EndScene.unity` plus `EndRunSummary`/`EndSceneController`; `GameManager` captures win/loss stats and loads `EndScene` instead of showing the GameScene terminal popup.
- Follow-up correction: GameScene hides the terminal modal immediately while EndScene is pending, and EndScene now mirrors MainMenu composition: title/mission/buttons on the left, command report stats on the right, and the same bullet-rain/title-flicker visual language.
- Follow-up device fix: EndScene background and ambient FX now render under a full-screen root outside SafeArea, while interactive content remains SafeArea-fitted; horizontal bullet rain sprite display size doubled.
- Verify: `dotnet build Assembly-CSharp.csproj --no-restore` pass with 0 warnings/errors.

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
- Legacy `OnGUI` runtime debug trong `EnergySystem`, `SeedBar`, `OverchargeSystem`, `EnemySpawner`, `GameManager` da bi xoa; runtime UI nguoi choi chi di qua `GameUiController`.
- `SeedBar`, `OverchargeSystem`, `EnemySpawner` expose API nho de HUD doc state va goi action thay vi tu ve UI trong tung script.
- `GameUiController` tu tao `EventSystem` neu scene chua co, va `PlacementController` van hoi `PointerOverPanel` de chan click UI xuyen xuong board.
- **Unity verify:** chu project da test cac chuc nang chinh hoat dong binh thuong: HUD hien dung, seed tray/OC/pause/settings co ban chay duoc.
- TextMeshPro Essentials da duoc Unity import sau khi mo project. Khong can import `Examples & Extras` vao ban release.

### Overnight tactical campaign sprint — 2026-06-23
- Them `OVERNIGHT_SPRINT.md` lam checkpoint cho phien lam viec tu chu 6 gio: objective, non-negotiables, work blocks, desired morning state.
- Them `CampaignIntel` lam shared logic cho campaign map, enemy mix, threat label, recommended tools va pressure score.
- Dang thay mission select scroll list bang **campaign map**:
  - node theo sector type (`Outpost`, `Armor Gate`, `Shield Column`, `Coreline Stand`...);
  - route line giua cac level;
  - detail panel hien status, briefing/reward, enemy mix, tool recommendation, pressure score;
  - click node chi select, nut `DEPLOY` moi reload mission.
- Them wave intel HUD nho duoi top bar: doc wave hien tai/ke tiep tu `EnemySpawner` va `CampaignIntel`.
- Balance/AI QA duoc mo rong de report campaign pressure, enemy mix, tool recommendation va canh bao map position/catalog mismatch.
- Them command feedback strip tren seed tray:
  - selected seed status khi san sang dat;
  - thong bao thieu energy/cooldown/o bi chan/o da co unit;
  - thong bao Overcharge locked/thieu energy/row da active/activate thanh cong.
- Them lane pressure readability tren OC panel:
  - row button hien enemy count khi co threat;
  - threat fill cam/do tinh theo enemy gan coreline va so luong trong lane;
  - khong thay doi combat stat/rule.
- Them spawn fairness:
  - enemy spawn mac dinh uu tien lane dang it enemy hon;
  - gioi han streak cung lane bang `maxSameRowStreak`;
  - config di qua `LevelDefinition` -> `GameBalance` -> `EnemySpawner`, khong hardcode rieng.
- AI QA runner sinh them `AIReports/latest_playtest_checklist.md` de huong dan test thu cong sau moi pass lon.

### Campaign/UI/visual foundation pass — 2026-06-23
- Campaign map chuyen tu fixed 10-node layout sang horizontal `ScrollRect` viewport:
  - content width tinh theo so level;
  - node/route dat theo content coordinates;
  - mo map tu focus selected/current mission;
  - san sang mo rong 20+ level ma khong tran mep card.
- Them `TEST MODE: ALL MISSIONS UNLOCKED` badge trong mission detail khi `LevelManager.unlockAllLevelsForTesting` dang bat.
- Nang cap main menu runtime overlay theo direction "2D shooter command menu": full-screen dark tactical background, title block lon, command panel doc, `CONTINUE`, `CAMPAIGN`, `SETTINGS`, `RESTART MISSION`; main menu pause game nhung khong chong pause modal.
- Pause modal co `MAIN MENU`: pause van giu nguyen state ban choi; chi khi bam `MAIN MENU` moi reload scene sach, giu lai selected level/progress/settings qua persistent data.
- Tach main menu sang `GameUiController.MainMenu.cs` de giam trach nhiem cua `GameUiController.cs`.
- Cleanup pass:
  - xoa legacy IMGUI debug UI trong `EnergySystem`, `SeedBar`, `OverchargeSystem`, `EnemySpawner`, `GameManager`;
  - xoa serialized `showDebugImGui` orphan trong `SampleScene`;
  - xoa `.DS_Store` trong `Assets`;
  - xoa bunker legacy assets khong con reference: `bunker_4.png`, `Bunker.controller`, `bunker.anim`, `bunker_death.anim` va `.meta`;
  - tach UI factory/helper sang `GameUiController.UiFactory.cs`;
  - tach campaign map/mission navigation sang `GameUiController.Campaign.cs`;
  - reset static runtime registries/pools khi load scene de tranh stale state sau reload/main menu.
- UI runtime co accent strip cho buttons/cards de bot cam giac rectangle prototype.
- Board procedural them backdrop panels, lane signals va entry chevrons de doc sci-fi battlefield hon.
- Enemy type visual fallback: Armored/Fast/Shield co badge mau nho khi chua co prefab art rieng.
- Enemy type badge da doi tu cot doc sang chip nho o chan enemy de tranh nhin nhu visual artifact.
- Them `VfxAutoDestroy` va `VfxPrefabBuilder` Editor tool de tao/gan VFX prefab vao `CombatVfxSettings`; ban moi uu tien sprite VFX, fallback ParticleSystem khi thieu art.
- Da generate `Assets/Prefabs/VFX/*` va gan vao `CombatVfxSettings` cua `SampleScene`.
- AI QA sau VFX generation: `0` fail, `1` expected warning do `LevelManager.unlockAllLevelsForTesting` dang bat.

### Frontend scene split + settings redesign — 2026-06-23
- Them scene rieng: `MainMenuScene`, `SettingsScene`, `HowToPlayScene`, `MissionMapScene`, `GameScene`.
- Them `FrontendUiController` de ve frontend bang runtime uGUI/TMP, dung chung `LevelCatalog`, `CampaignIntel`, `PlayerProgress`.
- Flow moi:
  - `START GAME` trong main menu -> `MissionMapScene`;
  - `DEPLOY` trong mission map -> `GameScene`;
  - `SETTING` va `HOW TO PLAY` vao scene rieng;
  - `MAIN MENU` trong gameplay -> `MainMenuScene`.
- Main menu khong hien mission map backdrop nua; mission map la man rieng.
- Settings scene duoc thiet ke lai thanh card rong, row slider/toggle tach khoang cach lon de tranh text chong len nhau.
- `EditorBuildSettings` chay tu `MainMenuScene`; AI QA/VFX editor helper doi default gameplay scene sang `GameScene`.
- Follow-up UX fix:
  - them `Frontend Camera` cho cac frontend scene va runtime camera fallback de tranh `Display 1 No cameras rendering`;
  - frontend `BACK` dung navigation history nhe, fallback ve `MainMenuScene`;
  - trong `GameScene`, nut `MISSION` mo mission map overlay trong cung scene, `BACK` dong overlay va giu state/pause state thay vi load `MissionMapScene`.
- UI overlap fix:
  - mission detail `Recommended tools` xuong dong theo tung tool va co vung text cao hon;
  - pause modal duoc noi card, tach progress/music/SFX/toggle thanh cac hang rieng de tranh chong chu.
- `NEWS_TASK.md` UI/UX pass TASK-01..10:
  - cap nhat docs de `NEWS_TASK.md` la current UI/UX rule source cho runtime-generated UI;
  - slider handle/toggle knob dung cyan trong ca frontend va in-game factories;
  - `START GAME` doi sang primary cyan, `EXIT` co danger red border, footer debug text duoc doi thanh `MISSION MAP ONLINE`;
  - seed card selected state ro rang bang cyan background/text den;
  - toggle doi sang pill track/knob, mission nodes tang size, deploy button cao hon trong mission map scene va gameplay overlay.

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
- Luu y lich su: ban dau la visual foundation/procedural placeholder. Hien tai generated board sprite da duoc wire vao `BoardVisualController`, con procedural grid/rail/marker giu vai tro gameplay overlay.

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
- Muc tieu UI: bot cam giac cac nut la overlay nho tach roi, gan hon voi game surface. Hien tai UI van build bang code-generated uGUI, nhung button/panel/hero/icon da co generated sprite skin baseline.
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
  - Build Settings co enabled scene va co gameplay scene;
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
- Them `LevelManager.unlockAllLevelsForTesting`; tai thoi diem nay scene bat flag de test nhanh tat ca level ma khong ghi gia progress completed vao `PlayerPrefs`.
- Production unlock rule van giu trong `PlayerProgress.IsLevelUnlocked`; khi gan release co the tat test flag de quay lai unlock tuan tu.
- Chuan hoa Fast/Shield tuning vao `EnemySpawner.GetTypeModifier`, de runtime va QA report dung cung mot source so lieu.
- AI QA check them:
  - authored waves co group Fast/Shield khi level pack da co 4-5 mission;
  - Fast phai health thap hon + speed cao hon baseline;
  - Shield phai health cao hon + speed thap hon + resist projectile + vulnerable EMP.
- Balance report hien `Enemy Type Tuning` de Codex kiem soat stat identity bang so lieu, khong bat chu project do bang cam giac.

### Campaign sequential unlock enforcement - 2026-06-27
- Tat `unlockAllLevelsForTesting` mac dinh trong `LevelManager` va `FrontendUiController`.
- Cap nhat `MainMenuScene`, `MissionMapScene`, `SettingsScene`, `HowToPlayScene`, `GameScene`, va `SampleScene` de serialized override deu la `0`.
- Current rule: save moi chi mo level 1; level 2 mo khi `GameManager.Win()` goi `LevelManager.MarkCurrentLevelCompleted()` va `PlayerProgress.HighestCompletedLevel` dat 1.
- AI QA playtest checklist bay gio yeu cau test override giu tat de campaign progression di tuan tu.

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

- `EnergySystem.cs` — tổng năng lượng, tự rơi energy orb theo thời gian; runtime HUD đọc state qua `GameUiController`.
- `SeedBar.cs` — state chọn unit/cost/cooldown; runtime HUD vẽ seed tray và gọi API chọn seed.
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

### Procedural visual polish pass - 2026-06-23
- Chon huong code/procedural polish truoc khi co asset final: khong download asset ngoai, khong tao UI prefab song song voi runtime UI dang dung.
- Frontend scenes:
  - them background tactical grid, panel corner ticks, map field grid, mission node status strip/dot;
  - mission routes co base line + glow line de doc hon;
  - selected mission node doi text den tren nen cyan.
- GameScene runtime UI:
  - top bar, seed tray, overcharge panel, wave intel, command status, pause modal, main menu overlay va campaign map overlay co chung panel chrome/corner ticks;
  - campaign overlay dong bo mission node selected/cleared/locked state voi frontend mission map.
- Board procedural:
  - `BoardVisualController` them tech backdrop, cell nodes, defense ports va enemy hazard stripes;
  - muc tieu la lam board co chieu sau hon ma van giu cell/grid doc tren mobile.
- Combat VFX fallback:
  - muzzle embers, hit core burst, death cross spark, EMP sparks;
  - Rail Cannon co beam fallback neu prefab beam chua gan;
  - energy orb collect co pop/pulse VFX qua `CombatVfx.PlayEnergyCollect`.
- Verify: `dotnet build Assembly-CSharp.csproj` pass; `dotnet build Assembly-CSharp-Editor.csproj` pass khi chay tuan tu.

### Frontend scene-authored UI migration - 2026-06-25
- Chuyen 4 frontend scenes (`MainMenuScene`, `SettingsScene`, `HowToPlayScene`, `MissionMapScene`) tu UI chi sinh runtime sang co san `FrontendCanvas/SafeAreaRoot` trong scene.
- `FrontendUiController` them `useSceneAuthoredUi`: khi Play se bind vao UI authored neu co; neu scene thieu UI authored thi fallback ve runtime builder cu.
- Them context menu tren controller va editor menu:
  - `Tools > Coreline > Rebuild Current Frontend Authored UI`
  - `Tools > Coreline > Rebuild All Frontend Authored UI`
- Muc tieu: chu project mo scene trong Unity Editor la thay panel/button/text/background de tinh chinh bang tay; code chi xu ly navigation/settings/mission data.
- Luu y: rebuild tool se ghi de `FrontendCanvas`, nen cac tinh chinh tay can duoc review truoc khi chay generator lai.

### UI micro-polish pass - 2026-06-25
- Checkpoint font/authored UI da commit + push truoc khi polish tiep.
- GameScene OC row buttons:
  - dung command button sprite thay nen rectangle thô;
  - them lane pip, threat strip day, active duration strip, alert edge, va color state ro hon.
- MainMenu:
  - `FrontendUiController` co `bulletRainSprite` map toi `sprite_bullet_turret_256`;
  - authored/generator path deu them horizontal bullet rain nhe;
  - `UiTitleFlicker` them pha flicker do/vang kieu den canh bao trong dem.
- Deploy transition:
  - text transition tang size 28 -> 56 de doc ro hon khi vao GameScene.
