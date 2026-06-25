# ART STYLE — Hướng dẫn & Prompt sinh sprite

File này gom toàn bộ phong cách art + quy ước + prompt để gen lại nhất quán bất cứ lúc nào (Recraft.ai hoặc công cụ khác). Prompt giữ tiếng Anh vì image-gen ăn tiếng Anh hơn.

---

## 1. Phong cách chung (chốt)

- **Pixel art** kiểu retro **16-bit**, **viền đen đậm (bold black outline)**, palette giới hạn, shading nhẹ.
- Tông chủ đạo: **giáp thép xám** + **điểm nhấn cam** cho phe phòng thủ; **bạc + lõi tím/xanh** cho địch alien.
- Chủ đề: thủ thành sci-fi tương lai. **KHÔNG dùng tên thương hiệu có bản quyền** (Iron Man, Plants vs Zombies, Marvel…) trong prompt/asset — chỉ làm cảm hứng, mô tả bằng từ chung chung.

### Palette gợi ý
- Giáp phe thủ: xám thép (#6b7079, #9aa0ab, #3a3d44), nhấn cam (#e0742c), mắt visor đỏ (#d23b3b).
- Địch alien: bạc xám, lõi năng lượng tím (#9b51e0) hoặc xanh.
- Snow gun / băng: xanh băng cyan (#5bd6e0, #aef2ff).
- Năng lượng / orb: cyan-trắng phát sáng (#aef0ff, #ffffff core).

---

## 2. Style block dùng chung (dán vào MỌI prompt)

Hai khối này giữ cả bộ đồng tông + đúng khung sprite — chỉ đổi phần mô tả thân, **giữ nguyên 2 khối này**:

**Khối STYLE:**
```
Chunky retro 16-bit pixel art style, bold black outline, limited palette,
crisp pixels.
```

**Khối COMPOSITION (nhân vật/máy đứng trên đất):**
```
Centered, full body inside frame, feet/base at the bottom, empty margin around it.
Transparent background. Square 1:1.
```

**Khối COMPOSITION (vật thể nổi: orb, bullet, drone):**
```
Centered single object, empty margin. Transparent background. Square 1:1.
```

---

## 3. Quy ước hướng nhìn (QUAN TRỌNG)

- **Địch (alien)** đi từ phải sang trái → **art quay mặt sang TRÁI**.
- **Phe phòng thủ** (turret, snow gun, lawnmower) bắn/đi sang phải → **art quay mặt + vũ khí sang PHẢI** (về phía địch).
- **Nguyên tắc:** vẽ sẵn đúng hướng hoạt động → KHÔNG lật trong code. (Đã bỏ `FaceLeft` trong `EnemyMover`.)
- Vật tĩnh đối xứng (bunker, arc reactor, orb) không cần hướng; bunker để giáp dày hướng phải.

---

## 4. Quy ước import vào Unity

- Texture Type: **Sprite (2D and UI)**, Sprite Mode: **Single**.
- **Pivot: Center** (lưới đặt object theo tâm ô).
- **Pixels Per Unit = cạnh ảnh** (vd 1024) rồi tinh chỉnh Transform Scale trên prefab cho vừa 1 ô.
- **Alpha Is Transparency: ON**, **Compression: None**, Mesh Type: Full Rect, Max Size ≥ cạnh ảnh.
- Filter Mode: Point (no filter) cho pixel art nét; hoặc Bilinear nếu muốn mềm.
- Ảnh **master để rig**: yêu cầu A-pose, tay/chân tách rời (xem mục 6).

---

## 5. Pose để rig (ảnh master)

Khi gen ảnh đem đi rig xương, thêm:
```
Neutral A-pose, standing straight, arms slightly away from body, legs slightly
apart, every limb clearly separated and not overlapping, all body parts visible.
```
→ Tay/chân tách rời = cắt bộ phận + gán xương dễ hơn nhiều.
(Vật tĩnh / máy / orb thì không cần A-pose.)

---

## 6. Negative prompt (nếu dùng Stable Diffusion / công cụ có ô negative)

```
zombie, human face, brand logo, superhero, Marvel, Iron Man,
crouching, curled pose, overlapping limbs, multiple characters,
text, watermark, drop shadow, floor, ground, 3/4 perspective,
cropped, cut off, blurry, busy background
```
(Recraft sinh nền trong sẵn nên thường không cần ô này.)

---

## 7. PROMPT TỪNG OBJECT

> Mỗi prompt đã nhúng sẵn style + composition. Khi gen, nhớ **chọn Style đã lưu** + bật **Transparent background** trong Recraft.

### 7.1 Địch — Alien robot (cơ bản)
```
Enemy production rule:
- Basic Alien Robot is the canonical enemy base.
- Generate and approve the Basic enemy first, then use it as image-reference/image-to-image input for Armored/Fast/Shield/Heavy variants.
- Enemy variants must preserve the same body size, canvas footprint, pivot, ground line, left-facing side-profile pose, head/core/hip/limb joint positions, and rig proportions as Basic.
- Variants only add modules on top of Basic: armor plates, shield emitter, exposed wires, speed fins, color/core accents. Do not create a taller, wider, differently posed, or different-limb-count enemy.

Pixel art game character sprite, full body, side profile view facing left,
an alien invader robot enemy — biomechanical otherworldly design, brushed
silver-grey metal carapace, glowing violet energy core in the chest, thin
multi-jointed legs, antenna-like sensors, unsettling non-human shape.
Chunky retro 16-bit pixel art style, bold black outline, limited palette, crisp pixels.
Centered, full body inside frame, feet at the bottom, empty margin around it.
Transparent background. Square 1:1.
```
**Biến thể (đổi dòng mô tả thân):**
- Địch giáp (trâu máu, chậm): `Add thick silver-grey armor plates over the same base body, partially cover the violet core behind armor, reinforce the same legs with plating, and make the tanky identity clear while preserving the Basic enemy proportions.`
- Địch nhanh: `Remove some armor plating, add exposed blue-violet wires, small speed fins, lighter leg plating, and sharper forward energy accents so it reads as quick and fragile while preserving the Basic enemy proportions.`
- Địch khiên: `Add an energy shield plate or shield emitter on the enemy's leading left side, with blue-violet shield glow and defensive identity, while preserving the Basic enemy proportions.`

### 7.2 Turret (lính minigun cơ bản — phe thủ)
```
Pixel art game sprite, full body, facing right, weapon pointing right.
A futuristic armored soldier defender, chunky retro 16-bit pixel style, bold black
outline, grey steel power-armor with orange accent lines, glowing red visor eyes,
spiky helmet. Holding a large multi-barrel minigun cannon with ammo belt and
orange energy details. Neutral standing pose, full body inside frame, feet at the
bottom, empty margin. Transparent background. Square 1:1.
```

### 7.3 Snow gun (súng băng — làm chậm)
```
Pixel art game sprite, full body, facing right, weapon pointing right.
A futuristic armored soldier defender, chunky retro 16-bit pixel style, bold black
outline, grey steel power-armor with icy cyan accent lines, glowing ice-blue visor
eyes, spiky helmet. Holding a large frost/ice cannon: frosted metal barrels, glowing
blue coolant tubes, icicle details, faint cold mist. Neutral standing pose, full body
inside frame, feet at the bottom, empty margin. Transparent background. Square 1:1.
```

### 7.4 Bunker (lô cốt — chịu đòn, không bắn)
```
Pixel art game sprite, full body, a heavy defensive bunker fortification: a bulky
armored sci-fi barricade of thick grey steel plates, riveted metal, a small glowing
slit/visor, sturdy and squat, heavier reinforced armor plating facing right. Chunky
retro 16-bit pixel style, bold black outline, grey steel with orange accent lines.
A defensive structure, NOT a character, no weapon. Standing on the ground, base at
the bottom, centered, empty margin. Transparent background. Square 1:1.
```
Tuỳ chọn thêm: `also generate damaged/cracked variants of the same bunker` (để đổi sprite theo % máu).

### 7.5 Drone EMP (nổ diện rộng — vật bay)
```
Pixel art game sprite, a small hovering EMP attack drone, compact spherical sci-fi
body with glowing electric-blue energy coils, antenna sparks, charged and about to
burst, grey steel + electric blue, chunky retro 16-bit pixel style, bold black
outline. A floating device, NOT a character, no legs. Centered single object,
empty margin. Transparent background. Square 1:1.
```

### 7.6 Lawnmower (tuyến cứu cuối — máy quét laser chạy phải)
```
Pixel art game sprite, full body, facing right. A compact armored last-defense laser
sweeper rover on small tank treads, grey steel chassis with a forward laser emitter
glowing bright red-orange, orange accent lines, ready to charge right. Chunky retro
16-bit pixel style, bold black outline. A machine, NOT a character. On the ground,
base at the bottom, centered, empty margin. Transparent background. Square 1:1.
```

### 7.7 Arc reactor (máy sản năng lượng — thiết bị tĩnh)
```
Pixel art game sprite, full body, a stationary sci-fi energy generator device: a
glowing arc-reactor core mounted on a small armored metal base/tripod, pulsing
cyan-blue energy rings. Matching grey steel, orange accents, bold black outline,
retro 16-bit pixel style. A device, NOT a character. Standing on the ground, base
at the bottom, centered, empty margin. Transparent background. Square 1:1.
```

### 7.8 Energy orb (mặt trời — nhặt được)
```
Pixel art game icon, a single glowing energy orb / power cell collectible, bright
cyan-white plasma sphere with a radiant core and soft glow, sci-fi energy. Chunky
retro 16-bit pixel style, bold black outline. No character. Centered single object,
empty margin. Transparent background. Square 1:1.
```

### 7.9 Bullet (đạn turret thường)
```
Pixel art game projectile sprite, a small glowing bullet/energy round, side view
pointing right, orange-yellow tracer streak, chunky retro 16-bit pixel style, bold
black outline. Tiny single object, centered. Transparent background. Square 1:1.
```
**Đạn snow gun (mảnh băng):** đổi `orange-yellow tracer streak` → `icy blue frost shard with cold mist trail`.

---

## 8. Quy trình Recraft & lưu trữ

1. **Lưu Style:** Recraft → mục Styles → *Create your own style* → upload ảnh tham chiếu đã ưng (turret, alien…) → đặt tên (vd `MyGame Pixel`). Mỗi lần gen chọn style này để đồng tông qua nhiều ngày.
2. **Bật Transparent background** khi gen (khỏi tách nền).
3. **Giữ nguyên nhân vật, đổi style:** dùng **image-to-image**, đưa file PNG cũ làm input + chọn Style khác.
4. **Sao lưu của riêng mình** (đừng chỉ dựa vào tài khoản Recraft):
   - Ảnh tham chiếu + mọi PNG đã gen → `Assets/Art/_source/`.
   - File này (`ART_STYLE.md`) là log prompt + quy ước, version-control trong repo.

---

## 9. Pipeline sau khi có ảnh

`Recraft (sinh base)` → `Aseprite (dọn pixel/sửa nhỏ nếu cần)` → `Unity: import (mục 4) → rig xương (Skinning Editor) → clip Idle/Walk/Attack/Death → Animator (params: Walking bool, Attack/Die trigger) → gán CharacterAnimator`.

Chi tiết bước rig & các lỗi thường gặp: xem `PROJECT_PROGRESS.md` mục Bước #7.
