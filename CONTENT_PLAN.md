# CONTENT PLAN — Art, animation, VFX, audio, UI

File nay quan ly noi dung can san xuat cho game. Chu project nen dung file nay de biet can ve/rig/test gi tiep theo.

---

## 1. Art Direction chung

Xem chi tiet trong `ART_STYLE.md`.
Prompt gen asset chi tiet xem `ASSET_GENERATION_PROMPTS.md`.

Tom tat:
- Retro 16-bit pixel art.
- Bold black outline.
- Palette han che.
- Nen/mood moi: dark navy, blue-grey, muted purple shadows, diem sang amber nho kieu warning beacon/reactor lamp.
- Phe thu: steel grey + orange accents + red/cyan visor.
- Enemy: silver/grey alien robot + violet/blue energy core.
- Transparent background, square image.
- UI/HUD/seed icon dung front-facing/direct icon view de bam/doc ro.
- Gameplay unit/cover/object dung side profile hoac slight 3/4 gameplay view; phe thu quay phai, enemy quay trai.

---

## 2. Asset Pipeline de xuat

1. Generate concept/base sprite bang Recraft/image-gen.
   - Dat ten file theo `ASSET_GENERATION_PROMPTS.md` muc `Output File Naming`.
   - Luu source PNG vao `Assets/Art/_source/`; khong overwrite runtime PNG trong `Assets/Art/` khi chua duyet/import.
2. Duyet silhouette o size nho.
3. Don pixel/sua chi tiet bang Aseprite.
4. Import Unity:
   - Texture Type: Sprite (2D and UI).
   - Sprite Mode: Single neu la mot sprite stage.
   - Filter Mode: Point neu muon pixel net.
   - Compression: None trong giai doan dev.
   - PPU theo canh lon nhat texture de sprite 256/512/1024 khong tu phong world size; prefab `SpriteRenderer.size = 1x1`.
   - Sau khi Unity tao `.meta`, chay `Tools > Art > Apply Generated Sprites To Prefabs` de map sprite moi vao gameplay prefabs, UI/board scene refs, va rebuild VFX prefabs.
5. Neu la nhan vat rig:
   - Tach layer/body parts.
   - Rig trong Unity 2D Animation/Skinning Editor.
   - Tao clip: Idle, Walk, Attack, Death.
   - PNG static preview co the tam dung bang SpriteRenderer va tat `SpriteSkin`, nhung final rig phai co bone/weights that va bat lai SpriteSkin.
6. Kiem tra prefab:
   - SpriteRenderer/SpriteSkin.
   - Animator Controller.
   - CharacterAnimator neu code can trigger anim.
   - Health/DamageStages neu can.

---

## 3. Unit Content

### Turret

Status: static preview sprite da map; can rig/animation polish.

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

Status: gameplay effect wired; static preview sprite da map; can rig/visual polish.

Can co:
- Sprite/rug dung huong phai.
- Attack recoil.
- Ice projectile.
- Frost impact/snow slow VFX.
- SFX bang/laser lanh.

Current gameplay content:
- SnowGun dung `ProjectileHitEffect Slow(value=0.5, duration=3)` tu `GameBalance`.
- `FrostProjectile.prefab` da dung `sprite_projectile_frost_256`.
- Frost impact van can asset/VFX prefab sau.

### Bunker

Status: generated damage stage sprites da map; can add dynamic death VFX polish.

Can co:
- `bunker_1`: lanh.
- `bunker_2`: nut/mop nhe.
- `bunker_3`: hu nang.
- Death VFX dong: smoke, spark, metal break.

Khong nen:
- Them sprite no tung tinh vao damage stage. Bunker death dung VFX dong roi an sprite.

### ArcReactor

Status: icon UI da map; gameplay board sprite rieng chua co neu thieu `sprite_arc_reactor_256.png`.

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

Enemy family rule:
- `Basic Alien Robot` la base enemy chinh thuc.
- Tao/duyet Basic truoc, sau do dung lam image-reference/image-to-image cho Armored/Fast/Shield/Heavy.
- Cac variant phai giu cung canvas footprint, pivot, ground line, huong trai, ti le than, vi tri dau/core/khop/chan va rig proportions de reuse skeleton/animation.
- Variant chi them module len base: giap, shield emitter, day dien, speed fins, mau/core accent. Khong tao silhouette moi khac chieu cao/pose/so chi.

### Basic Alien Robot

Priority: highest.

Current status:
- Static preview sprite `sprite_enemy_basic_base_256` da map vao `Enemy.prefab`.
- `SpriteSkin` dang tam tat cho static preview; can rig lai neu muon giu walk/attack/death bone animation.

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
- Day la master rig/design reference cho toan bo enemy family.

### Armored Alien

Priority: high.

Current status:
- Static preview sprite `sprite_enemy_armored_256` da map vao `ArmorEnemy.prefab`.
- Reward drop cho armored enemy da wired trong `EnemySpawner`.

Can co:
- Cung bo clip nhu Basic neu dung chung controller.
- Visual nang hon tu Basic: them giap day, core bi che mot phan, nhung giu cung body footprint/khop/ti le.
- Toc do cham hon, HP cao hon.
- Gameplay identity hien tai:
  - resistant hon voi projectile thuong;
  - it bi slow/knockback/stun hon;
  - vulnerable hon voi EMP.
- Art nen doc ro la enemy boc giap/duoc che chan, de nguoi choi hieu vi sao can EMP/SnowGun/OC.

### Future Enemy

- Fast Alien: static preview sprite da map vao `FastEnemy.prefab`; bien the tu Basic, bot giap/them wire/speed fins, toc do cao; khong doi chieu cao/rig footprint; counter bang SnowGun/Bunker.
- Shield Alien: static preview sprite da map vao `ShieldEnemy.prefab`; bien the tu Basic, them shield emitter/khien phia truoc ben trai; counter bang EMP/pierce/splash.
- Heavy Alien: bien the tu Basic, them armor module nang hon nhung giu rig proportions; cham, cuc ben; counter bang OC/EMP.
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
- `Tools > VFX > Rebuild Core VFX Prefabs` tao sprite VFX prefabs tu `vfx_*_256.png` neu co, fallback ParticleSystem neu sprite thieu, va gan vao `GameScene`/`SampleScene`.
- `GeneratedArtApplier` cung goi rebuild VFX, nen flow chuan sau khi them art la chay `Tools > Art > Apply Generated Sprites To Prefabs`.
- Current tuning pass da clean alpha cho VFX PNG de tranh dark square artifact, giam scale muzzle/EMP/energy collect/death/bunker break, va spawn muzzle flash tai dau nong. Van can Play Mode verify tren nhieu resolution vi VFX phu thuoc sprite moi va camera framing.

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
- Generated/procedural `button_command` (canonical path: `Assets/UI/button_command.png`), `ui_panel_9slice_source_256`, `menu_hero_coreline_outpost_256`, va seed icons da duoc wire vao runtime UI factories/scenes.
- Board/background da co `BoardVisualController` voi generated board sprite lam base va procedural overlay giu grid/rail/click readability.
- Procedural visual polish pass da them tactical grid/chrome/corner ticks cho frontend/GameScene UI va mission map; generated panel/menu hero/board art hien thay the cac rectangle chinh, con chrome/grid la overlay phu.
- Strict UI spec: `button_command` la shared button background duy nhat cho clickable action. Khong ve notch/stripe/line/frame rieng tren button; state normal/hover/pressed/disabled dung Color modulate theo `UI_SPEC.md`, pressed co 2px down offset, text khong shadow/outline.
- Victory/Defeat modal dung dark panel trung tinh + typography/result accent nhe, khong dung terminal backdrop toan man hinh, khong phu xanh/do transparent hay hop mau rong.
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
- Main menu production art sau vertical slice:
  - full-screen command background sprite/scene;
  - logo/title treatment for `CORELINE DEFENSE`;
  - 9-slice command panel + single shared `button` skin matching current runtime layout;
  - subtle scanline/noise overlay and cyan/red tactical rails.
- Campaign map node/route art cho map scroll ngang.
- Icon/skin cho Overcharge row buttons.
- Pause/settings modal background; button states dung tint/overlay tren single shared `button` skin.

## 8. Board / Background Art

Runtime hien tai:
- Camera background toi.
- Board runtime official hien tai la `Assets/Art/board_coreline_combat_grid_5x9.png`, wire vao board/frontend/game mission views.
- Board van giu lane/grid/defense rail/enemy entry overlay bang code de cell va click target doc ro.
- `Tile.prefab` la overlay trong suot de giu cell click target va doc grid.
- Board opacity runtime hien tai la `1.0`; overlay grid/rail/spawn markers cua gameplay dam nhiem readability.
- Prompt board trong `ASSET_GENERATION_PROMPTS.md` hien yeu cau board base texture 5x9, trung tam phang/it noise, chi nhieu detail o rim trai/phai.
- Neu thay board production moi, generate/clean/approve source truoc roi thay co chu dich vao `board_coreline_combat_grid_5x9.png` de giu reference on dinh.

Can co cho visual polish that:
- 1 background/board sprite moi theo phong cach sci-fi industrial trong `ART_STYLE.md`: dark slab arena, wall/gate border, amber beacon lights, nhung phai la gameplay base texture chu khong phai cinematic room illustration.
- Board can doc ro 5 lane x 9 cell o man hinh dien thoai.
- Ben trai co defense rail/rail cannon anchors.
- Ben phai co enemy entry/gate/warning strip.
- Vung center khong qua nhieu chi tiet de unit/enemy/projectile van doc ro.
- Co the dung 1 anh background lon + marker/grid overlay rieng de de tune trong Unity.

App Store quality minimum:
- Bo UI skin rieng: panel 9-slice, single shared `button` background, slider handle, toggle. Button normal/pressed/disabled/danger dung tint hoac overlay trong Unity, khong tao PNG rieng.
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
- `PlayMuzzleFlash`, `PlayHitSpark`, `PlayPulse`, `PlayDeathBurst`, `PlayStaticBreak`, `PlayBunkerBreak`, `PlayRailCannonBeam` uu tien prefab VFX generated-sprite neu duoc gan trong `CombatVfxSettings`.
- Fallback code-generated van chay neu slot prefab thieu hoac bi xoa.
- `PlayEnergyCollect` pop/pulse khi nhat energy orb.

Khi co prefab VFX dep, thay ruot cac ham nay hoac cho `DeathEffect` spawn prefab.
