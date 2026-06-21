# TECH PLAN — Systems, architecture, quality

File nay quan ly huong ky thuat cua project Unity 2D.

---

## 1. Current Architecture

Scripts chinh:
- `GridManager`: luoi 5x9, world/cell conversion, unit occupancy.
- `PlacementController`: click/placement/collect orb.
- `EnergySystem`: energy total, sky orb spawn.
- `SeedBar`: unit selection/cost/cooldown bang IMGUI.
- `EnergyProducer`: ArcReactor spawn energy.
- `EnemySpawner`: wave state machine.
- `EnemyMover`: di chuyen/attack/lawnmower trigger.
- `Shooter`: scan enemy cung hang va ban.
- `Projectile`: bay/trung enemy/gay damage/slow.
- `Health`: mau, death flow, event.
- `CharacterAnimator`: bridge giua gameplay code va Animator parameter.
- `DamageStages`: doi sprite theo % mau cho object tinh.
- `GameManager`: lawnmower, win/lose.
- `BombUnit`: EMP bomb.
- `Lawnmower`: tuyen cuu cuoi.

---

## 2. Animator Contract

Neu gameplay code goi `CharacterAnimator`, Animator Controller phai co:

Enemy:
- Bool `Walking`
- Trigger `Attack`
- Trigger `Die`

Unit co attack/death:
- Trigger `Attack`
- Trigger `Die`

Chu y:
- Parameter phan biet hoa/thường.
- Enemy controller da duoc chuan hoa thanh `Walking`, `Attack`, `Die`.
- Object co Animator va can gameplay trigger nen co `CharacterAnimator`.
- Object tinh chi doi damage state khong nen dung Animator de doi cung SpriteRenderer voi `DamageStages`.

---

## 3. Damage va Death

`Health` la source of truth cho mau.

Recommended:
- Rigged character: `Health` -> `CharacterAnimator.TriggerDie()` -> destroy sau `deathAnimTime`.
- Static object: `Health` + `DamageStages` -> sprite stage theo mau -> death VFX rieng -> destroy.
- Projectile/EMP chi goi `TakeDamage`, khong tu dieu khien sprite/animation cua target.

Can lam sau:
- Them `DeathEffectSpawner` hoac field `deathEffectPrefab` vao `Health`.
- Them hit flash/hit effect event.
- Them damage numbers neu can.

---

## 4. Data Architecture de xuat

Hien tai nhieu stats nam trong prefab Inspector. Khi gameplay on dinh, nen tach ra:

### UnitDefinition ScriptableObject
- label
- prefab
- cost
- cooldown
- hp
- damage
- fireInterval
- special effect type

### EnemyDefinition ScriptableObject
- prefab
- hp
- speed
- attackDamage
- armor/resistance
- reward neu co

### WaveDefinition ScriptableObject
- list spawn group
- enemy type
- count
- delay
- row policy
- final wave flag

Loi ich:
- Tuning khong can sua code.
- De tao level.
- De so sanh balance.

---

## 5. Performance Plan

Mobile priority:
- Object pooling cho projectile, enemy, orb, VFX.
- Giam `Instantiate/Destroy` lien tuc.
- Tranh allocation trong Update khi co nhieu enemy/projectile.
- Sprite atlas cho art.
- Texture compression rieng cho iOS/Android khi gan release.
- Target FPS: 60 neu co the, toi thieu on dinh 30 tren may yeu.

Can audit sau:
- `EnemyMover.All` scan trong `Shooter`/`Projectile` co the ok luc nho, nhung can toi uu neu nhieu object.
- IMGUI nen thay bang UI Toolkit/uGUI cho mobile polish.
- VFX particle count can gioi han.

---

## 6. Testing Plan

Manual Play Mode:
- Spawn wave 1-3.
- Dat tung unit.
- Enemy idle/walk/attack/death.
- Bunker damage stages.
- Energy collect.
- Game over/win.
- Pause/restart sau nay.

Device Testing:
- iPhone/Android real device.
- Touch target size.
- Safe area/notch.
- FPS/memory.
- Build install/run.

Automated/Editor checks sau nay:
- Prefab missing references.
- Animator parameter mismatch.
- Required components per prefab type.
- Basic gameplay logic tests neu tach pure C# duoc.

---

## 7. Tooling de xuat

Co the tao Editor tools:
- ValidatePrefabReferences.
- ValidateAnimatorParameters.
- Balance table exporter.
- Wave preview.
- Sprite import preset checker.

Codex co the viet cac tool nay khi project bat dau co nhieu prefab/asset.

---

## 8. Git/Version Control

Nen commit:
- `Assets/`
- `Packages/`
- `ProjectSettings/`
- `*.md`
- `.gitignore`
- `.gitattributes`

Khong commit:
- `Library/`
- `Temp/`
- `Logs/`
- `UserSettings/`
- build output.

Quan trong:
- Luon commit asset va `.meta` cung nhau.
- Dung Git LFS cho PNG/PSD/PSB/Aseprite/audio/video/FBX.

