# TASKS — Phan cong va uu tien

File nay la bang viec hien hanh. Cap nhat thuong xuyen sau moi phien lam viec.

---

## Doing

### Chu project

- Mac dinh UI/gameplay copy trong game la **English**.
- Localization sau release slice:
  - English la source copy chinh hien tai.
  - Vietnamese, Chinese, French them sau bang localization table.
  - Khong them Vietnamese khong dau vao runtime UI nua.
- Current validation focus:
  - Kiem tra level select khong tran khoi mission box khi co 5+ level.
  - Khi co thiet bi that: test safe area/notch/touch target.
  - Test level 4-5 moi ve pacing/fairness; Fast/Shield stat identity do AI QA kiem tra bang so lieu.
  - Neu tiep tuc lam art: uu tien board/background, seed icons, VFX prefab.

### Codex

- Truoc moi feature lon:
  - phan tich boi canh va constraint hien co;
  - neu co nhieu huong, neu ro tradeoff;
  - chon huong toi uu theo codebase hien tai;
  - cap nhat `.md` lien quan truoc/kem commit feature.
- Kiem tra/sua mismatch giua code C# va Unity data khi chu project bao loi.
- Neu production foundation pass co loi, sua ngay: pooling state, VFX fallback, tutorial hint, level catalog.
- Ho tro setup `DamageStages`, `CharacterAnimator`, Animator Controller, prefab references.
- Ho tro tinh chinh `GameBalance`, Rail Cannon/Overcharge/audio sau khi co feedback Play Mode.
- Tinh chinh combat feedback pass sau Play Mode: flash strength, shake amount, VFX duration/color.
- Tinh chinh runtime HUD sau Play Mode neu co feedback visual: safe area, spacing, text size, seed card state, modal layout.
- Nang cap VFX death rieng cho bunker/object tinh va VFX beam Rail Cannon khi co art/VFX prefab.
- Chay/cai tien AI QA report khi Unity Editor dong hoac qua menu trong Editor.
- Sau khi vertical slice on, danh gia co nen nang `GameBalance` len ScriptableObject/level config hay giu MonoBehaviour tren scene.

### Done gan day

- Runtime HUD rebuild da duoc chu project test OK trong Unity.
- TextMeshPro Essentials da duoc import; khong can dua `Examples & Extras` vao ban release.
- Production foundation pass da duoc chu project test OK trong Unity.
- Production foundation pass:
  - TMP examples da duoc xoa;
  - pooling cho projectile/energy orb;
  - VFX prefab hook qua `CombatVfxSettings`;
  - tutorial hint hook qua `TutorialCoach`;
  - runtime FPS/mobile settings;
  - `LevelCatalog.asset`;
  - AI QA mo rong.
- Board visual polish pass:
  - `BoardVisualController` tao board/background procedural.
  - `Tile.prefab` da doi tint/sorting de bot cam giac mau phang prototype.
  - Camera background da doi sang tone sci-fi toi.
- Board visual polish pass da duoc chu project test OK trong Unity.
- Mission progression loop v1:
  - them `Level_02`, `Level_03`;
  - `LevelCatalog` co 3 level;
  - `PlayerProgress` luu selected/unlocked/completed;
  - HUD co level select overlay va next level button;
  - AI QA check catalog integrity.
- Mission progression loop v1 da duoc chu project test OK trong Unity.
- Mission unlock/gating v1:
  - level data co `allowedUnitLabels` va `overchargeUnlocked`;
  - `SeedBar` filter seed theo level;
  - `OverchargeSystem` co lock state;
  - tutorial hint ton trong unlock state;
  - AI QA check unknown allowed unit labels.
- English-first runtime UI baseline:
  - HUD/modal/mission select/tutorial hints doi sang English;
  - docs ghi ro localization se them sau cho English/Vietnamese/Chinese/French.
- Phase 1 enemy traits/resistance foundation:
  - `EnemyTraits`;
  - `EnemyBalance` trait multipliers;
  - Projectile/EMP/control effect ton trong trait;
  - ArmorEnemy co resistance/vulnerability khac BasicEnemy;
  - balance report va AI QA cap nhat.
- UI scale + early pacing pass:
  - TMP wrapping warning fixed;
  - top bar/seed tray/OC panel scaled up;
  - level 1-3 early waves slowed/tuned after unlock gating.
- Board framing + adaptive command deck:
  - camera zoom nhe de board/cells lon hon;
  - seed tray width tu tinh theo seed count;
  - seed cards khong bi force-expand.
- Mission reward/unlock copy v1:
  - `LevelDefinition` co `missionBriefing` va `completionReward`;
  - level select hien briefing/lock detail;
  - victory modal hien reward;
  - AI QA canh bao level moi neu thieu copy.
- Progress save hardening v1:
  - `PlayerProgress` co save version;
  - completed count khong tang lai khi replay level cu;
  - selected/highest completed behavior giu nguyen.
- AI QA release-readiness expansion:
  - QA report check Build Settings enabled scene;
  - QA report check productName/bundleVersion placeholder;
  - QA report check duplicate/missing catalog level assets.
- Mobile safe-area HUD foundation:
  - `SafeAreaFitter` co trong runtime HUD;
  - top bar/seed tray/OC/modal dung `SafeAreaRoot`;
  - can test tren mobile/notch aspect sau.
- Wave status clarity pass:
  - top-right HUD hien `Deploy`, `Wave`, `Clear wave`, `Next wave` dung state;
  - countdown doi mau warning trong 3 giay cuoi truoc wave.
- Combat end-state + tutorial reliability pass:
  - Tutorial hint khong chan click ArcReactor/energy orb/placement.
  - Tutorial hint tu an sau `hintDisplayDuration`.
  - Victory chi hien sau khi enemy death object bien mat het, khong hien ngay luc HP ve 0.
  - AI QA warning neu tutorial duration cau hinh khong hop le.
- Compact HUD + modal layout pass:
  - top bar/energy font nho hon nhung van de doc;
  - mission/wave nam trong status box co outline;
  - seed cards nho hon va khong che qua nhieu board;
  - pause/victory/defeat buttons can doi theo state;
  - board/cell nhin lon hon sau camera zoom `3.35`.
- Player reported latest compact HUD pass looks more reasonable and core functionality remains OK.
- Naming + app metadata:
  - Chot game title `Coreline Defense`.
  - Store listing target `Coreline Defense: Robot Siege`.
  - `PlayerSettings.productName` doi sang `Coreline Defense`.
  - Bundle identifier target `com.duycaonguyen.corelinedefense`.
- Enemy variety + campaign extension v1:
  - `LevelEnemyType` co `Fast` va `Shield`.
  - `EnemySpawner` apply type modifiers sau khi spawn/apply balance.
  - Level 4 `Velocity Breach` gioi thieu Fast.
  - Level 5 `Shield Column` gioi thieu Shield.
  - `LevelCatalog` co 5 level.
- Mission select testability + QA tuning checks:
  - level select dung `ScrollRect` nen list 5+ level khong tran card;
  - `LevelManager.unlockAllLevelsForTesting` dang bat trong scene de test nhanh tat ca level;
  - Fast/Shield tuning doc tu `EnemySpawner.GetTypeModifier`;
  - AI QA fail/warn neu Fast/Shield mat identity so lieu.
- Projectile effect content pass:
  - SnowGun dung `ProjectileHitEffect Slow` tu `GameBalance`, khong chi legacy slow fields;
  - `Shooter` clone hit effects vao projectile khi ban de pooling khong giu state cu;
  - AI QA fail neu SnowGun mat slow projectile effect;
  - Level_05 reward YAML da quote de xu ly QA warning.

---

## Next

### Production roadmap phases

1. Phase 1 — Gameplay vertical slice:
   - DONE baseline: level 1-5, unlock/gating, reward copy, authored waves, enemy trait foundation, Fast/Shield type modifiers, dev unlock all levels for testing, SnowGun projectile effect content.
   - Remaining: art/prefab identity rieng cho Fast/Shield.
   - Remaining: one more balance pass after level 4-5 playtest.
2. Phase 2 — Visual/audio production pass:
   - production board/background for First Contact;
   - UI skin/icon set replacing code-generated rectangles;
   - VFX prefab replacements for generated effects;
   - animation polish for core unit/enemy set;
   - audio layering / music pass.
3. Phase 3 — Campaign content:
   - level 4-10;
   - level select polish with preview/rewards;
   - tutorial callouts;
   - unlock/reward copy.
4. Phase 4 — Mobile/release hardening:
   - safe area and touch validation;
   - enemy/VFX pooling;
   - AI QA/build validator;
   - Android/iOS dev builds;
   - store asset checklist.

### Gameplay/Tech

- Them he thong DeathEffect/VFX prefab cho object tinh:
  - Bunker death: smoke puff, sparks, metal bits, sound.
  - DroneEMP: EMP pulse.
  - Enemy death: co the giu animation rig truoc, VFX sau.
- Thay code-generated VFX bang prefab VFX dep hon khi visual direction on dinh.
- Them weapon moi de dung `ProjectileHitEffect` knockback/stun khi can mo rong counter-play.
- Chay `Tools > AI QA > Run Full Check` va doc `AIReports/latest_ai_qa_report.md`.
- Khi test xong level pack, tat `LevelManager.unlockAllLevelsForTesting` truoc release/build review neu muon restore unlock tuan tu.
- Them object pooling cho projectile, enemy, energy orb, VFX khi bat dau toi uu mobile.
- Tach `GameBalance` thanh ScriptableObject/level data khi bat dau co nhieu level.
- Them/replace hit flash/hit sound bang prefab/audio final cho enemy va bunker.
- Polish UI runtime moi: icon seed card, energy icon, OC row feedback, pause modal visual.
- Them speed control neu gameplay can.
- Save/progress/settings baseline da co; chi nang len JSON/full SaveData khi co currency/upgrades/inventory.
- Level definition va authored wave workflow baseline da co.
- Polish level select UI: icon/preview/reward text/unlock copy.
- Runtime text cleanup: keep visible UI in English until localization system exists.
- Sau runtime HUD on dinh, tach thanh prefab UI/skinning pipeline neu can art UI rieng.

### Art/Content

- Lam production board art cho First Contact:
  - background/base board co chieu sau;
  - lane/grid markers ro o kich thuoc mobile;
  - defense rail va enemy entry gate;
  - khong de board tranh doc voi sprite unit/enemy.
- Hoan thien vertical slice:
  - 1 enemy basic rig dep.
  - 1 armored enemy bien the ro.
  - 1 turret co attack ro.
  - 1 bunker co 3 damage stages.
  - 1 ArcReactor co idle/pulse.
- Lam VFX co ban: muzzle flash, hit spark, death smoke.
- Chon SFX tam thoi cho shoot, hit, death, collect energy.
- Lam UI art: seed cards, icons, energy counter, wave banner, pause/settings.
- Lam menu/title/icon tam thoi khi bat dau build mobile.

---

## Backlog

- Fast/Shield enemy prefab/art identity rieng.
- Heavy enemy neu sau level 5 can them nhịp tank khac ArmorEnemy.
- Projectile effects: knockback, freeze/slow variants, stun/EMP variants.
- Boss/mini-boss.
- Level select art/icon/preview polish.
- Upgrade system.
- Level pack 1: 5-10 level dau voi do kho tang dan.
- Economy/progression meta: unlock unit, upgrade, reward currency.
- Daily reward/progression.
- Ads/IAP sau khi core gameplay da vui.
- Save/load progress.
- Settings: music, SFX, vibration, language.
- Tutorial/onboarding.
- Accessibility: text size, color contrast, reduce shake neu can.
- Localization pipeline: tieng Viet/tieng Anh truoc.
- Build automation: Android/iOS dev build, version bump, release notes.
- App Store/Google Play store assets.

---

## Blocked / Can kiem tra

- Unity batchmode compile co the khong chay neu project dang mo trong Unity Editor. Neu can verify compile, dong Unity hoac cho phep chay khi project khong mo.
- Sprite slicing cua `bunker_2`, `bunker_3`, `bunker_4` dang co thay doi. Neu chi dung moi anh la mot stage, nen de Sprite Mode = Single.
- Asset production chua chot: can visual brief cho board/UI/icons/VFX truoc khi tao asset hang loat.

---

## Definition of Done cho mot feature

- Code khong co Console error.
- Prefab references duoc gan dung.
- Choi Play Mode duoc it nhat 1 wave lien quan.
- Neu co animation: parameter/transition khop voi code.
- Neu co asset: sprite/meta duoc commit cung nhau.
- `TASKS.md` hoac file plan lien quan duoc cap nhat neu thay doi huong di.
