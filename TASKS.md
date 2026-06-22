# TASKS — Phan cong va uu tien

File nay la bang viec hien hanh. Cap nhat thuong xuyen sau moi phien lam viec.

---

## Doing

### Chu project

- Play test combat feedback pass:
  - Turret/SnowGun ban co muzzle flash.
  - Dan trung enemy co hit spark.
  - Bunker/unit bi danh co flash/rung nhe.
  - DroneEMP co pulse.
  - Rail Cannon hit enemy co spark.
  - Console khong co error.
- Test SnowGun sau khi rig + animator dung chung Unit controller.
- Test lai gameplay sau khi them `GameBalance`:
  - seed cost/cooldown hien dung;
  - Turret/SnowGun ban dung;
  - sua `overchargeFireRateMultiplier` tren `GameBalance` trong Play Mode phai doi toc do ban ngay;
  - Enemy/ArmorEnemy danh bunker/unit voi am thanh attack hop ly;
  - Overcharge buff toc do ban/damage dung cam giac.
- Tune cac so tren `GameSystems > GameBalance` thay vi sua tung prefab neu chi la gameplay stat.
- Ghi cam giac: Rail Cannon co du thoa man khong, audio nao can thay/giu.

### Codex

- Kiem tra/sua mismatch giua code C# va Unity data khi chu project bao loi.
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

---

## Next

### Roadmap uu tien sau combat feedback

1. Runtime HUD polish pass: safe area, final spacing, icons, seed card art, mobile touch targets.
2. Bunker/object death VFX pass.
3. Projectile effect architecture: slow/knockback/stun/EMP theo tung loai dan.
4. Balance pass bang `GameBalance`.
5. AI QA harness v1.
6. Mobile UI/input pass: tutorial overlays, level select, settings polish.
7. Tutorial/onboarding pass: day nguoi choi energy, dat unit, Overcharge, Rail Cannon.
8. Level/progression pass: level data, unlock unit/enemy, save/load.
9. Performance/device pass: pooling, texture/audio compression, test tren dien thoai.

### Gameplay/Tech

- Them he thong DeathEffect/VFX prefab cho object tinh:
  - Bunker death: smoke puff, sparks, metal bits, sound.
  - DroneEMP: EMP pulse.
  - Enemy death: co the giu animation rig truoc, VFX sau.
- Thay code-generated VFX bang prefab VFX dep hon khi visual direction on dinh.
- Test `ProjectileHitEffect` cho knockback/stun tren mot bullet rieng khi can them weapon moi.
- Chay `Tools > AI QA > Run Full Check` va doc `AIReports/latest_ai_qa_report.md`.
- Them object pooling cho projectile, enemy, energy orb, VFX khi bat dau toi uu mobile.
- Tach `GameBalance` thanh ScriptableObject/level data khi bat dau co nhieu level.
- Them hit flash/hit sound cho enemy va bunker.
- Polish UI runtime moi: icon seed card, energy icon, OC row feedback, pause modal visual.
- Them pause/resume, restart, speed control neu can.
- Them save/load progress va settings volume/vibration.
- Them level definition va wave authoring workflow.
- Mo rong `LevelDefinition` de chua wave list that thay vi formula wave.
- Lam level select UI tam thoi doc tu danh sach `LevelDefinition`.
- Test thu `Level_01.useAuthoredWaves = true` sau khi gameplay hien tai on dinh de so sanh nhịp wave authored voi formula cu.
- Sau runtime HUD on dinh, tach thanh prefab UI/skinning pipeline neu can art UI rieng.

### Art/Content

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

- Fast Alien.
- Shield Alien.
- Projectile effects: knockback, freeze/slow variants, stun/EMP variants.
- Boss/mini-boss.
- Level select.
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
- Nhieu thay doi asset/prefab hien dang trong working tree; khong revert neu khong co yeu cau ro.

---

## Definition of Done cho mot feature

- Code khong co Console error.
- Prefab references duoc gan dung.
- Choi Play Mode duoc it nhat 1 wave lien quan.
- Neu co animation: parameter/transition khop voi code.
- Neu co asset: sprite/meta duoc commit cung nhau.
- `TASKS.md` hoac file plan lien quan duoc cap nhat neu thay doi huong di.
