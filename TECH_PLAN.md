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
- `GameBalance`: central runtime config cho unit/enemy/energy/wave/Overcharge.
- `CombatVfx`: code-generated prototype VFX cho muzzle/hit/death/pulse.
- `DamageFeedback`: auto flash/shake khi `Health` nhan damage.

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
- `DeathEffect` co the override death VFX theo prefab; neu khong co, non-enemy se fallback sang static break/default burst.

Can lam sau:
- Them hit flash/hit effect event.
- Them damage numbers neu can.

---

## 4. Weapon Socket / MuzzlePoint

Shooter khong spawn bullet tu root transform nua.

Contract:
- `Shooter.muzzlePoint` la empty child dat o dung dau nong.
- Neu `muzzlePoint` trong, code fallback ve `transform.position`.
- Root/pivot dung cho grid placement.
- MuzzlePoint dung cho projectile spawn.
- Visual sprite/rig khong can bi dich chuyen chi de dan ra dung vi tri.

`Unit.prefab` va `SnowGun.prefab` da co child `MuzzlePoint`; can tinh chinh bang mat trong Unity.

---

## 5. Rail Cannon Last Defense

Script/class van ten `Lawnmower` de giu prefab reference, nhung hanh vi gameplay la Rail Cannon.

Contract:
- Moi row co mot rail cannon dung mot lan.
- Khi enemy breach, `EnemyMover` goi `GameManager.TryLawnmower(row)` nhu cu.
- `Lawnmower.Activate()` hien beam va gay damage lon cho tat ca enemy trong row.
- Sau `beamDuration`, row defense bi clear va object destroy.

Sau nay co the doi ten script/prefab sang `RailCannon` khi da san sang migrate reference.

---

## 6. Audio

`AudioManager` tren `GameSystems` la singleton null-safe.

Hooks hien co:
- Shoot
- Hit
- EnemyDeath
- UnitBreak
- EnergyCollect
- EmpBurst
- RailCannon
- UiClick
- Win
- GameOver

Chua co clip thi game van chay im lang. Khi co SFX, them vao `AudioManager.clips`.

---

## 7. Overcharge

`OverchargeSystem` la mechanic chu dong dau tien de game khac PvZ hon.

Contract:
- Gan tren `GameSystems`.
- Ton `energyCost` de buff mot row trong `duration` giay.
- Shooter cung row doc multiplier qua `OverchargeSystem.FireRateMultiplierForRow(row)` va `DamageMultiplierForRow(row)`.
- UI tam thoi la IMGUI nut `OC` ben phai va phim so 1-5.

Can tuning sau Play Mode:
- energyCost
- duration
- fireRateMultiplier
- damageMultiplier

---

## 8. Combat Feedback

Hien tai VFX la code-generated de prototype nhanh, chua phai final art.

Hooks:
- `Shooter.Fire()` -> muzzle flash tai `muzzlePoint`.
- `Projectile` hit -> hit spark.
- `BombUnit` -> EMP pulse + hit spark.
- `Lawnmower` Rail Cannon -> hit spark tren enemy bi quet.
- `Health.TakeDamage()` -> `Damaged` event.
- `DamageFeedback` tu duoc gan runtime cho object co `Health`, flash/rung nhe khi damage.
- `Health.Die()` -> non-enemy death burst co ban.
- Enemy chi flash khi bi damage, khong shake root transform vi enemy dang di chuyen.
- `PlacementController` khong cho dat unit vao cell dang co enemy.
- Knockback/freeze/stun nen la effect rieng theo loai dan/vu khi sau nay, khong nam trong damage feedback mac dinh.
- `DeathEffect` + `CombatVfx.PlayStaticBreak/PlayBunkerBreak` la VFX death tam cho object tinh.

Sau nay:
- Doi `CombatVfx` sang spawn prefab VFX.
- Them hit flash rieng cho enemy rig neu flash root lam animation bi rung qua nhieu.
- Them screen shake nhe cho EMP/Rail Cannon.

---

## 9. Data Architecture

Hien tai stats gameplay chinh da duoc gom ve `GameBalance` tren object `GameSystems` trong `SampleScene`.

Contract hien tai:
- Prefab giu default saner values.
- Khi Play Mode, `GameBalance` apply cac stats chinh vao `SeedBar`, placed unit, spawned enemy, `EnergySystem`, `EnemySpawner`, va `OverchargeSystem`.
- Khi edit `GameBalance` trong Play Mode, `OnValidate` goi runtime tuning de day thay doi sang `OverchargeSystem` va object dang song co `BalanceIdentity`.
- Live tuning khong refill current HP; neu tang/giam maxHealth giua Play Mode thi object dang song chi clamp current HP.
- Neu chi tune gameplay, uu tien sua `GameSystems > GameBalance` thay vi sua tung prefab.
- Neu sua visual/reference/rig/MuzzlePoint thi van sua prefab.

Dang gom trong `GameBalance`:
- Unit: label, prefab, cost, cooldown, maxHealth, deathAnimTime.
- Shooter: fireInterval, bulletSpeed, bulletDamage, bulletSlowFactor, bulletSlowDuration.
- ArcReactor: energyAmount, energyInterval.
- DroneEMP: empFuse, empRadius, empDamage.
- Enemy: maxHealth, deathAnimTime, speed, attackDamage, attackSfxInterval.
- Global: startEnergy, sky orb, wave setup, Overcharge cost/duration/multipliers.

Buoc nang cap sau, khi co nhieu level, nen tach tiep thanh:

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

## 10. Performance Plan

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

## 11. Testing Plan

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

## 12. Tooling de xuat

Co the tao Editor tools:
- ValidatePrefabReferences.
- ValidateAnimatorParameters.
- Balance table exporter.
- Wave preview.
- Sprite import preset checker.
- AI QA report writer: markdown/json + screenshots.
- Build validator: missing refs, null clips, large textures, Android/iOS settings.

Codex co the viet cac tool nay khi project bat dau co nhieu prefab/asset.

AI QA v1 hien co:
- Menu: `Tools > AI QA > Run Full Check`.
- Batchmode method: `AiQaReportRunner.RunFullCheck`.
- Output: `AIReports/latest_ai_qa_report.md` va `AIReports/latest_balance_metrics.json`.
- Check hien tai: `GameBalance`, audio SFX entries, unit/enemy prefab components, animator parameter contract.

---

## 13. Mobile UI va Input

IMGUI hien tai tot cho prototype, khong nen giu cho release.

Hien tai v1:
- `GameUiController` ve pause/settings/progression panel bang IMGUI.
- `GameSettings` luu SFX volume/reduce shake/vibration bang PlayerPrefs.
- `PlacementController` hoi `GameUiController.PointerOverPanel` de chan click xuyen UI.
- Day la UI tam thoi, khong phai final mobile UI.

Can lam:
- uGUI hoac UI Toolkit cho seed bar, energy, wave, pause, win/lose.
- Safe area cho iPhone notch va Android cutout.
- Touch target toi thieu ~44px logical.
- Pointer/touch handling tach ro UI va world placement.
- Haptic/vibration optional khi collect energy, EMP, Rail Cannon.

---

## 14. Level/Progression Architecture

Khi co hon 1 level, can tach data:
- LevelDefinition: grid/theme, startEnergy, available units, wave list.
- WaveDefinition: enemy groups, timing, row policy.
- UnlockDefinition: unit/enemy/tutorial gates.
- SaveData: completed levels, unlocked units, settings.

Hien tai v1:
- `LevelDefinition` ScriptableObject gom level id/name/number va balance global.
- `LevelManager` tren `GameSystems` apply `currentLevel` vao `GameBalance` luc Awake.
- `PlayerProgress` dung PlayerPrefs de mark level complete khi `GameManager.Win()`.
- `Assets/Levels/Level_01.asset` la level data dau tien.
- `LevelDefinition` da co optional authored waves: `useAuthoredWaves`, `LevelWaveDefinition`, `LevelSpawnGroup`.
- `EnemySpawner` van fallback ve formula wave cu neu authored waves tat hoac rong.
- `Level_01.useAuthoredWaves` hien bat mac dinh sau playtest OK.

Uu tien tiep:
1. Level select UI tam thoi.
2. SaveData co version khi progression phuc tap hon.
3. Tach UI prototype sang uGUI/UI Toolkit.

---

## 15. Projectile Effect Architecture

Slow cu van duoc giu de tuong thich trong `Projectile`, nhung da co effect list moi:
- `ProjectileHitEffect[] hitEffects`.
- `ProjectileEffectType.Slow`.
- `ProjectileEffectType.Knockback`.
- `ProjectileEffectType.Stun`.

Sau nay co the mo rong them:
- Damage.
- Pierce/splash.
- Armor modifier.

Nguyen tac:
- Hit feedback mac dinh khong di chuyen target.
- Knockback/freeze chi xay ra khi projectile/effect type yeu cau.
- Enemy resistance co the nam trong EnemyDefinition.

---

## 16. Build va Release Pipeline

Can co truoc khi len store:
- Android/iOS dev build reproducible.
- Version number/build number workflow.
- Release vs debug config.
- Texture/audio compression settings.
- Symbol/log/crash reporting neu them SDK.
- Checklist khong co missing references va Console error.

---

## 17. Git/Version Control

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
