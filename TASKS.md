# TASKS — Phan cong va uu tien

File nay la bang viec hien hanh. Cap nhat thuong xuyen sau moi phien lam viec.

---

## Doing

### Chu project

- Tai/chon bo SFX tam thoi cho vertical slice.
- Import audio vao `Assets/Audio/` va gan vao `AudioManager.clips`.
- Test SnowGun sau khi rig + animator dung chung Unit controller.
- Test lai Overcharge sau khi Codex tuning multiplier: toc do ban phai thay doi ro hon.
- Ghi cam giac: Rail Cannon co du thoa man khong, audio nao can thay/giu.

### Codex

- Kiem tra/sua mismatch giua code C# va Unity data khi chu project bao loi.
- Ho tro setup `DamageStages`, `CharacterAnimator`, Animator Controller, prefab references.
- Ho tro tinh chinh Rail Cannon/Overcharge/audio sau khi co feedback Play Mode.
- De xuat VFX death rieng cho bunker/object tinh va VFX beam Rail Cannon.
- Tuning Overcharge de buff toc do ban ro hon.
- Sau khi animation core on, de xuat buoc chuyen stats sang ScriptableObject config.

---

## Next

### Gameplay/Tech

- Them he thong DeathEffect/VFX prefab cho object tinh:
  - Bunker death: smoke puff, sparks, metal bits, sound.
  - DroneEMP: EMP pulse.
  - Enemy death: co the giu animation rig truoc, VFX sau.
- Them object pooling cho projectile, enemy, energy orb, VFX khi bat dau toi uu mobile.
- Tach stats cua unit/enemy/wave thanh ScriptableObject de de tuning.
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
