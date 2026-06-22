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

Status: can rig/polish.

Can co:
- Sprite/rug dung huong phai.
- Attack recoil.
- Ice projectile.
- Frost impact/snow slow VFX.
- SFX bang/laser lanh.

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

### Future Enemy

- Fast Alien: chan dai, nho, toc do cao.
- Shield Alien: khien phia truoc.
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

Can co:
- Seed packet icons.
- Cooldown overlay.
- Energy counter.
- Wave indicator.
- Pause button.
- Win/Game Over panels.
- Main menu sau vertical slice.
