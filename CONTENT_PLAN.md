# CONTENT PLAN — Art, animation, VFX, audio, UI

File nay quan ly noi dung can san xuat cho game. Chu project nen dung file nay de biet can ve/rig/test gi tiep theo.

---

## 1. Art Direction chung

Xem chi tiet trong `ART_STYLE.md`.

Tom tat:
- Retro 16-bit pixel art.
- Bold black outline.
- Palette han che.
- Phe thu: steel grey + orange accents + red/cyan visor.
- Enemy: silver/grey alien robot + violet/blue energy core.
- Transparent background, square image.
- Unit quay phai; enemy quay trai.

---

## 2. Asset Pipeline de xuat

1. Generate concept/base sprite bang Recraft/image-gen.
2. Duyet silhouette o size nho.
3. Don pixel/sua chi tiet bang Aseprite.
4. Import Unity:
   - Texture Type: Sprite (2D and UI).
   - Sprite Mode: Single neu la mot sprite stage.
   - Filter Mode: Point neu muon pixel net.
   - Compression: None trong giai doan dev.
5. Neu la nhan vat rig:
   - Tach layer/body parts.
   - Rig trong Unity 2D Animation/Skinning Editor.
   - Tao clip: Idle, Walk, Attack, Death.
6. Kiem tra prefab:
   - SpriteRenderer/SpriteSkin.
   - Animator Controller.
   - CharacterAnimator neu code can trigger anim.
   - Health/DamageStages neu can.

---

## 3. Unit Content

### Turret

Status: can polish.

Can co:
- Static/base sprite hoac rigged sprite.
- Idle.
- Attack recoil.
- Death/hit reaction neu rigged.
- MuzzlePoint dat dung dau nong.
- Muzzle flash VFX.
- Bullet sprite/trail.
- SFX shoot.

Khuyen nghi visual:
- Than giap xam, diem cam.
- Sung nhin ro sang phai.
- Recoil nho nhung doc duoc.

### SnowGun

Status: gameplay effect wired; can rig/visual polish.

Can co:
- Sprite/rug dung huong phai.
- Attack recoil.
- Ice projectile.
- Frost impact/snow slow VFX.
- SFX bang/laser lanh.

Current gameplay content:
- SnowGun dung `ProjectileHitEffect Slow(value=0.5, duration=3)` tu `GameBalance`.
- Visual frost projectile/impact van can asset/VFX prefab sau.

### Bunker

Status: dang lam damage stages.

Can co:
- `bunker_1`: lanh.
- `bunker_2`: nut/mop nhe.
- `bunker_3`: hu nang.
- Death VFX dong: smoke, spark, metal break.

Khong nen:
- Dung `bunker_4` la no tung tinh.

### ArcReactor

Status: can polish.

Can co:
- Idle pulse.
- Energy orb spawn effect.
- SFX energy collect/spawn.

### DroneEMP

Status: can VFX.

Can co:
- Idle/charge.
- Fuse warning.
- EMP pulse.
- Electric spark.
- SFX charge + burst.

### Rail Cannon

Status: thay the last-defense kieu lawnmower.

Can co:
- Rail cannon device/marker ben trai moi row.
- Warning glow ngan khi enemy breach.
- Beam xuyen row.
- Spark/explosion nho tren enemy bi hit.
- SFX rail shot.

---

## 4. Enemy Content

### Basic Alien Robot

Priority: highest.

Can co:
- Idle.
- Walk.
- Attack.
- Death.
- Hit flash/VFX.
- SFX hit/death.

Khuyen nghi:
- Silhouette ro: dau/core/chan khac biet.
- Khong qua nhieu chi tiet nho.
- Core tim/xanh de doc la enemy.

### Armored Alien

Priority: high.

Can co:
- Cung bo clip nhu Basic neu dung chung controller.
- Visual nang hon: giap day, core bi che mot phan.
- Toc do cham hon, HP cao hon.
- Gameplay identity hien tai:
  - resistant hon voi projectile thuong;
  - it bi slow/knockback/stun hon;
  - vulnerable hon voi EMP.
- Art nen doc ro la enemy boc giap/duoc che chan, de nguoi choi hieu vi sao can EMP/SnowGun/OC.

### Future Enemy

- Fast Alien: chan dai, nho, toc do cao; counter bang SnowGun/Bunker.
- Shield Alien: khien phia truoc; counter bang EMP/pierce/splash.
- Heavy Alien: cuc ben, cham; counter bang OC/EMP.
- Mini-boss: to, doc dao, xuat hien cuoi level.

---

## 5. VFX List

Must-have vertical slice:
- Turret muzzle flash.
- Bullet hit spark.
- Enemy death spark/smoke.
- Bunker break smoke/spark.
- Energy orb collect pop.
- EMP pulse.

Implementation note:
- `CombatVfxSettings` tren `GameSystems` da co slot prefab cho muzzle/hit/death/static break/bunker break/EMP/Rail Cannon.
- Khi co VFX prefab that, keo vao day; fallback code-generated se duoc thay the tung phan.

Nice-to-have:
- Screen shake nhe khi EMP/lawnmower.
- Slow frost overlay/particles.
- Lane warning khi wave lon.
- Hit flash material/shader.

---

## 6. Audio List

Must-have:
- Shoot.
- Hit metal.
- Enemy attack/metal bite loop ngan theo interval.
- Enemy death.
- Bunker break.
- Energy collect.
- Seed select / unit place UI click.
- EMP burst.
- UI click.
- Game over/win sting.

Mobile note:
- SFX phai ngan, ro, khong qua day.
- Co settings rieng music/SFX.

Audio sourcing note:
- Uu tien CC0/royalty-free commercial-safe.
- Tot nhat cho placeholder: Kenney audio packs.
- Tot cho tim tung am rieng: Freesound, nhung chi dung CC0 hoac CC BY va luu attribution neu can.
- Tot cho thu vien lon/pro hon: Sonniss GDC bundles.
- Tranh CC BY-NC/NonCommercial neu game co kha nang len store/monetize.

---

## 7. UI Art

Runtime hien tai:
- HUD da chuyen sang uGUI/TextMeshPro code-generated trong `GameUiController`.
- Day la UI runtime that de gameplay dung duoc, nhung visual art van can polish bang icon/sprite rieng.
- Board/background da co `BoardVisualController` tao procedural sci-fi lane layout tam thoi. Day la guide layout, khong phai final environment art.
- Runtime UI copy hien dung English. Vietnamese/Chinese/French se them sau bang localization pipeline.

Can co:
- Seed packet icons.
- Cooldown overlay.
- Energy counter.
- Wave indicator.
- Campaign map node icons:
  - outpost/armor gate/signal relay/raider track/shield column/EMP corridor/velocity net/iron rain/breach point/coreline stand;
  - route line/sector connector art;
  - selected/current/cleared/locked visual states.
- Pause button.
- Win/Game Over panels.
- Main menu sau vertical slice.
- Icon/skin cho Overcharge row buttons.
- Pause/settings modal background + button states.

## 8. Board / Background Art

Runtime hien tai:
- Camera background toi.
- Board co lane bands, grid lines, defense rail, enemy entry zone bang code.
- `Tile.prefab` la overlay trong suot de giu cell click target va doc grid.

Can co cho visual polish that:
- 1 background/board sprite cho level "First Contact" theo phong cach sci-fi industrial.
- Board can doc ro 5 lane x 9 cell o man hinh dien thoai.
- Ben trai co defense rail/rail cannon anchors.
- Ben phai co enemy entry/gate/warning strip.
- Vung center khong qua nhieu chi tiet de unit/enemy/projectile van doc ro.
- Co the dung 1 anh background lon + marker/grid overlay rieng de de tune trong Unity.

App Store quality minimum:
- Bo UI skin rieng: panel 9-slice, button normal/pressed/disabled, slider handle, toggle.
- Icon seed doc duoc o kich thuoc nho.
- Energy/OC/pause dung icon thay vi chu thuan.
- Board/background co art direction, khong dung nen xanh + tile xam trong ban showcase.

Can them cho mobile release:
- Touch-friendly seed bar.
- Row Overcharge buttons hoac gesture/UI thay the.
- Settings panel: music/SFX/vibration.
- Tutorial callout/icon.
- Level select/map tam thoi.
- Store screenshots/title logo/app icon.

Localization content policy:
- English copy la ban goc trong giai doan nay.
- Khong viet Vietnamese khong dau trong UI runtime.
- Khi gameplay/menu flow on dinh, tao localization table cho English, Vietnamese, Chinese, French.

---

## 9. Level va Progression Content

Hien tai:
- Level 1: First Contact.
- Level 2: Armor Probe.
- Level 3: Signal Siege.
- Level 4: Velocity Breach.
- Level 5: Shield Column.
- Level 6: EMP Corridor.
- Level 7: Velocity Net.
- Level 8: Iron Rain.
- Level 9: Breach Protocol.
- Level 10: Coreline Stand.
- 10 level nay dung chung scene/board, khac authored waves va balance.
- Unlock rule: win level truoc de mo level tiep theo.
- Unit/OC unlock:
  - Level 1: ArcReactor + Turret.
  - Level 2: mo Bunker.
  - Level 3: mo SnowGun + DroneEMP + Overcharge.
  - Level 4: gioi thieu Fast enemy.
  - Level 5: gioi thieu Shield enemy.
  - Level 6-10: baseline mixed formations de test pacing truoc khi them enemy/art moi.

Can lam tiep:
- Level 1-3 baseline da duoc tune va playtest OK; level 4-10 can playtest/tune pacing/fairness.
- Sau playtest, quyet dinh level nao can enemy/unit/upgrade moi thay vi chi lap lai combination hien co.
- Moi level co theme/tint nho hoac background variation de tranh lap.
- Wave notes: enemy type, count, timing, huge wave.
- Reward/unlock copy baseline da co cho level 1-10.
- Level select can icon/preview/reward state de nhin nhu game release hon.

Phase content target:
- Phase 1: level 1-10 co data baseline va day du unit/enemy unlock — con tune sau playtest.
- Phase 2: First Contact co board/UI/VFX/audio du dep de lam vertical slice showcase.
- Phase 3: level 4-10 co enemy/unit combinations ro va them identity rieng neu can.
- Phase 4: store-facing content gom icon, screenshots, title/logo, short description.

---

## 10. Final VFX Direction

`CombatVfx` hien tai la prototype. Ban final nen co:
- Muzzle flash prefab rieng cho Turret/SnowGun.
- Hit spark metal nho, doc ro tren mobile.
- Bunker death: smoke + spark + metal debris nhe.
- Enemy death: animation rig + small spark/smoke.
- EMP: electric ring + screen shake nhe + crackle SFX.
- Rail Cannon: warning line, charge glow, beam, impact sparks.

Nguyen tac:
- VFX phai ngan va ro.
- Khong che mat gameplay cell.
- Co option giam shake neu can.

Hien tai da co VFX prototype:
- `PlayMuzzleFlash`.
- `PlayHitSpark`.
- `PlayPulse`.
- `PlayDeathBurst`.
- `PlayStaticBreak`.
- `PlayBunkerBreak`.

Khi co prefab VFX dep, thay ruot cac ham nay hoac cho `DeathEffect` spawn prefab.
