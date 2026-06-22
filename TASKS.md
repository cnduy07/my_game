# TASKS — Phan cong va uu tien

File nay la bang viec hien hanh. Cap nhat thuong xuyen sau moi phien lam viec.

---

## Doing

### Chu project

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
- De xuat VFX death rieng cho bunker/object tinh va VFX beam Rail Cannon.
- Sau khi vertical slice on, danh gia co nen nang `GameBalance` len ScriptableObject/level config hay giu MonoBehaviour tren scene.

---

## Next

### Gameplay/Tech

- Them he thong DeathEffect/VFX prefab cho object tinh:
  - Bunker death: smoke puff, sparks, metal bits, sound.
  - DroneEMP: EMP pulse.
  - Enemy death: co the giu animation rig truoc, VFX sau.
- Them object pooling cho projectile, enemy, energy orb, VFX khi bat dau toi uu mobile.
- Tach `GameBalance` thanh ScriptableObject/level data khi bat dau co nhieu level.
- Them hit flash/hit sound cho enemy va bunker.
- Lam UI mobile thay cho IMGUI khi gameplay core on.

### Art/Content

- Hoan thien vertical slice:
  - 1 enemy basic rig dep.
  - 1 armored enemy bien the ro.
  - 1 turret co attack ro.
  - 1 bunker co 3 damage stages.
  - 1 ArcReactor co idle/pulse.
- Lam VFX co ban: muzzle flash, hit spark, death smoke.
- Chon SFX tam thoi cho shoot, hit, death, collect energy.

---

## Backlog

- Fast Alien.
- Shield Alien.
- Boss/mini-boss.
- Level select.
- Upgrade system.
- Daily reward/progression.
- Ads/IAP sau khi core gameplay da vui.
- Save/load progress.
- Settings: music, SFX, vibration, language.
- Tutorial/onboarding.
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
