# ASSET GENERATION PROMPTS - Coreline Defense

Use this file to generate art for the first production visual slice. Prompts are written in English because image generation tools usually follow English better.

## Global Rules

Use these rules for every generation:

- Pixel art only: no 3D, no vector, no photorealism.
- No text, no logos, no watermark.
- Do not copy copyrighted characters, brands, or exact layouts from references.
- Coreline Defense is sci-fi, not fantasy: no castles, knights, zombies, superheroes, or medieval props.
- Keep silhouettes readable at small mobile size.
- Cold dark palette is the base: deep navy, blue-grey, muted purple shadows.
- Warm amber/orange lights are small accents only: beacons, reactor lamps, warning lights.
- Cyan is energy/reactor/accent light.
- If the tool allows seed/style reference, use the same saved pixel-art style for all assets.

## Global Negative Prompt

Use this negative prompt where supported:

```text
photorealistic, 3D render, smooth vector art, anime, watercolor, blurry, antialiasing blur, noisy, excessive tiny details, text, logo, watermark, UI labels, copyright character, brand logo, medieval castle, knight, zombie, superhero, Marvel, Iron Man, Plants vs Zombies, human face closeup, gore, messy background, unreadable silhouette, cropped object, cut off object, tiny subject, small object in the center, too much empty padding, large blank margins
```

## Canvas Fill Rules

For backgrounds:
- Fill the entire image edge-to-edge.
- No small scene floating inside a huge border.
- No built-in UI text or logo.

For board/background:
- The board/playable area must fill 90-96% of the image width.
- The 5x9 play area must be obvious and centered.
- Do not leave large empty margins around the board.

For sprites/icons:
- Transparent background.
- Subject centered horizontally.
- Subject should fill 75-90% of canvas height or width, depending on shape.
- Keep only 5-10% safe margin around the subject.
- Do not generate a tiny object in the middle of a large transparent canvas.

For VFX:
- Transparent background.
- Effect should fill 70-90% of canvas.
- Keep edges uncropped unless the prompt says beam/full-width.

## File Naming

Suggested output names:

- `menu_hero_coreline_outpost.png`
- `board_first_contact_5x9.png`
- `ui_panel_9slice.png`
- `ui_button_primary.png`
- `ui_button_secondary.png`
- `ui_button_danger.png`
- `icon_energy_orb.png`
- `icon_arc_reactor.png`
- `icon_turret.png`
- `icon_bunker.png`
- `icon_snowgun.png`
- `icon_drone_emp.png`
- `sprite_arc_reactor.png`
- `sprite_turret.png`
- `sprite_bunker_stage_1.png`
- `sprite_bunker_stage_2.png`
- `sprite_bunker_stage_3.png`
- `sprite_snowgun.png`
- `sprite_drone_emp.png`
- `sprite_rail_cannon.png`
- `sprite_enemy_basic.png`
- `sprite_enemy_armored.png`
- `sprite_enemy_fast.png`
- `sprite_enemy_shield.png`
- `sprite_energy_orb.png`
- `sprite_bullet_turret.png`
- `sprite_bullet_frost.png`
- `vfx_muzzle_flash.png`
- `vfx_hit_spark.png`
- `vfx_enemy_death.png`
- `vfx_emp_pulse.png`
- `vfx_rail_beam.png`

## Priority Batch 1

Generate these first:

1. Main menu hero background.
2. First Contact board background.
3. Seed icons: Energy, ArcReactor, Turret, Bunker, SnowGun, DroneEMP.
4. Sprites: Basic enemy, Armored enemy, Turret, ArcReactor.
5. VFX: muzzle flash, hit spark, enemy death burst, energy orb.

---

## 1. Main Menu Hero Background

Settings:
- Output size: 1920x1080 or 3840x2160.
- Aspect ratio: 16:9.
- Background: full image, not transparent.
- Composition: full-bleed scene, fills the entire canvas edge-to-edge.
- Leave readable dark space in upper third for Unity title text.
- Leave readable dark space in lower center for Unity menu buttons.
- No title text, no menu text, no logo.

Prompt:

```text
Create a 1920x1080 full-bleed 16:9 pixel art main menu background for a dark sci-fi tower defense game. The scene must fill the entire canvas edge-to-edge, not a small illustration floating inside a border. A distant fortified sci-fi outpost sits in a cold alien night landscape, with bunker walls, antenna towers, defense rails, and a faint alien breach gate on the horizon. Add deep navy sky, blue-grey mountains, muted purple shadows, a distant moon or planet, subtle stars and orbital debris. Use small warm amber warning beacons and reactor lamps like tiny torch-like focal points, plus a few cyan reactor energy accents. Keep the upper third and lower center visually clean and dark enough for UI text that will be added later in Unity. No text, no logo, no menu labels, no characters in the foreground.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable large shapes, low noise, dark sci-fi mood, deep navy and blue-grey shadows, muted purple shadow tones, small warm amber light accents, cyan reactor energy accents.
```

Optional variant:

```text
Add a tiny armored commander silhouette on a ridge looking toward the outpost, very small, no visible face, used only as background storytelling. The silhouette must not cover the future title/menu areas.
```

---

## 2. First Contact Board Background

Settings:
- Output size: 1920x1080 or 2304x1296.
- Aspect ratio: 16:9.
- Background: full image, not transparent.
- Composition: board fills 90-96% of image width and 78-90% of image height.
- Grid: clear 5 rows x 9 columns play area.
- Board must not appear as a tiny centered object with huge margins.
- No UI text, no units, no enemies.

Prompt:

```text
Create a 1920x1080 full-bleed 16:9 pixel art gameplay board background for a sci-fi lane defense game. The board must fill 90-96% of the image width and most of the image height, not a small board floating in empty space. The central playable area is a clear 5 rows x 9 columns industrial stone-metal slab arena, centered and easy to read on mobile. Show subtle rectangular tile divisions for the 5x9 grid, but keep the center cells clean enough for units, enemies, projectiles, and energy orbs. The left side has a defense rail with five rail cannon anchor ports aligned to the five rows. The right side has an alien breach gate / enemy entry zone with red-orange warning strips and amber beacon lights. The border has low bunker walls, cables, cracked plating, small debris patches, reactor lamps, and sci-fi outpost details. Use dark navy, blue-grey, muted purple shadows, sparse warm amber lights, and cyan reactor accents. No UI, no text, no characters, no enemies, no player units.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable large shapes, low noise, dark sci-fi mood, clean gameplay readability.
```

Reject if:
- The grid is unclear.
- The board occupies less than 85% width.
- The center play cells are too busy.
- It looks medieval/fantasy instead of sci-fi.

---

## 3. UI Skin Set

General UI settings:
- Transparent PNG.
- Pixel-perfect clean edges.
- Designed for Unity 9-slice where possible.
- Center area should be plain enough for text.
- No text.

### 3.1 UI Panel 9-Slice

Settings:
- Output size: 256x256 or 512x512.
- Transparent background.
- Panel fills 92-98% of canvas.
- Border thickness: about 10-18% of canvas.
- Center should be transparent or very dark and clean.

Prompt:

```text
Create a 512x512 transparent PNG pixel art sci-fi UI panel frame for 9-slice scaling. The panel frame must fill 92-98% of the canvas, not sit tiny in the middle. Use a dark navy metal border, crisp square pixel corners, cyan thin edge lights, and tiny amber warning-light pixels in the corners. The center should be transparent or very dark and clean for text/content. No text, no icons, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean UI asset, dark sci-fi mood.
```

### 3.2 Primary Button

Settings:
- Output size: 512x128.
- Transparent background.
- Button fills 94-98% width and 80-92% height.
- No text.

Prompt:

```text
Create a 512x128 transparent PNG pixel art sci-fi primary button skin. The button plate must fill 94-98% of the canvas width and 80-92% of the canvas height, not be a small button with huge empty padding. Use cyan glowing metal plate, dark inner bevel, crisp pixel border, subtle cyan highlight, and tiny amber indicator pixels. Designed for 9-slice scaling. No text, no icons, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean UI asset, dark sci-fi mood.
```

### 3.3 Secondary Button

Settings:
- Output size: 512x128.
- Transparent background.
- Button fills 94-98% width and 80-92% height.
- No text.

Prompt:

```text
Create a 512x128 transparent PNG pixel art sci-fi secondary button skin. The button plate must fill 94-98% of the canvas width and 80-92% of the canvas height. Use a dark blue-grey metal plate, subtle cyan edge light, crisp pixel border, and clean inner area for Unity text. Designed for 9-slice scaling. No text, no icons, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean UI asset, dark sci-fi mood.
```

### 3.4 Danger Button

Settings:
- Output size: 512x128.
- Transparent background.
- Button fills 94-98% width and 80-92% height.
- No text.

Prompt:

```text
Create a 512x128 transparent PNG pixel art sci-fi danger/destructive button skin. The button plate must fill 94-98% of the canvas width and 80-92% of the canvas height. Use dark red metal plate, red-orange warning edge light, crisp pixel border, and clean inner area for Unity text. Designed for 9-slice scaling. No text, no icons, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean UI asset, dark sci-fi mood.
```

---

## 4. Seed / HUD Icons

General icon settings:
- Output size: 256x256 or 512x512.
- Transparent background.
- Icon centered.
- Icon fills 78-90% of canvas width/height.
- Keep 5-10% safe margin.
- No text, no frame unless requested.

### Energy Icon / Orb

```text
Create a 512x512 transparent PNG pixel art energy orb icon. The orb must be centered and fill 78-88% of the canvas, not a tiny orb in empty space. It is a collectible sci-fi energy orb with bright cyan-white plasma core, pixel glow, small dark outline, and clean circular power-cell silhouette. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability.
```

### ArcReactor Icon

```text
Create a 512x512 transparent PNG pixel art ArcReactor seed icon. The device must be centered and fill 78-90% of the canvas. Show a compact sci-fi energy generator with cyan glowing core, steel grey casing, small orange/amber accent lights, and a clear readable silhouette. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability, dark sci-fi mood.
```

### Turret Icon

```text
Create a 512x512 transparent PNG pixel art Turret seed icon. The turret/gunner must be centered and fill 78-90% of the canvas. It faces right, with a large minigun barrel pointing right, steel grey armor, orange accent lights, and a small red visor detail. Clear weapon silhouette. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability, dark sci-fi mood.
```

### Bunker Icon

```text
Create a 512x512 transparent PNG pixel art Bunker seed icon. The bunker must be centered and fill 80-90% of the canvas width. Show a heavy squat sci-fi armored barricade with thick steel plates, small glowing slit, orange warning lights, and sturdy defensive silhouette. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability, dark sci-fi mood.
```

### SnowGun Icon

```text
Create a 512x512 transparent PNG pixel art SnowGun seed icon. The frost cannon defender must be centered and fill 78-90% of the canvas. It faces right, with steel grey body, icy cyan tubes, frosted barrel, and a clear cannon silhouette. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability, dark sci-fi mood.
```

### DroneEMP Icon

```text
Create a 512x512 transparent PNG pixel art DroneEMP seed icon. The EMP drone must be centered and fill 78-88% of the canvas. It is a compact hovering spherical device with blue electric coils, antenna sparks, steel grey shell, and clear silhouette. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability, dark sci-fi mood.
```

---

## 5. Unit Sprites

General unit sprite settings:
- Output size: 512x512 or 1024x1024.
- Transparent background.
- Full body/object visible.
- Subject centered horizontally.
- Subject fills 75-88% of canvas height.
- Base/feet sit near lower 85-94% of canvas, with small safe margin.
- Unit weapons face right.
- No floor, no large shadow, no text.

### ArcReactor Sprite

```text
Create a 1024x1024 transparent PNG full-body game sprite of a stationary sci-fi ArcReactor energy generator. The object must be centered and fill 75-88% of the canvas height, with the base near the bottom but not cropped. Show a glowing cyan-blue reactor core mounted on a compact armored base, energy rings, steel grey casing, orange warning lights, and a silhouette that fits one tower defense grid cell. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood, cyan reactor energy accents, small warm amber lights.
```

### Turret Sprite

```text
Create a 1024x1024 transparent PNG full-body game sprite of a futuristic armored defender turret/gunner. The subject must be centered and fill 75-88% of canvas height, not tiny in the middle. It faces right, weapon pointing right, with a large multi-barrel minigun, steel grey power armor, orange accent lights, small red visor, stable stance, and a silhouette that fits one tower defense grid cell. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Bunker Stage 1

```text
Create a 1024x1024 transparent PNG full-body game sprite of an undamaged heavy sci-fi bunker barricade. The bunker must be centered and fill 78-90% of canvas width and 60-78% of canvas height, with no huge empty padding. Show squat steel armor plates, thick dark outline, small glowing slit, orange warning lights, and sturdy rectangular defensive silhouette. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Bunker Stage 2

```text
Create a 1024x1024 transparent PNG full-body game sprite of the same heavy sci-fi bunker barricade as stage 1, moderately damaged. The bunker must be centered and fill 78-90% of canvas width and 60-78% of canvas height. Add visible cracks, bent steel plates, small scorch marks, and flickering warning lights while keeping the same overall silhouette. Not exploding. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Bunker Stage 3

```text
Create a 1024x1024 transparent PNG full-body game sprite of the same heavy sci-fi bunker barricade as previous stages, badly damaged. The bunker must be centered and fill 78-90% of canvas width and 60-78% of canvas height. Add large cracks, broken armor plates, exposed cables, dim warning light, but keep it recognizable as the same bunker. Not exploding. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### SnowGun Sprite

```text
Create a 1024x1024 transparent PNG full-body game sprite of a futuristic frost cannon defender. The subject must be centered and fill 75-88% of canvas height. It faces right, weapon pointing right, with steel grey armor, icy cyan coolant tubes, frosted barrel, small cold mist pixels, and a silhouette that fits one tower defense grid cell. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### DroneEMP Sprite

```text
Create a 1024x1024 transparent PNG game sprite of a small hovering EMP attack drone. The drone must be centered and fill 70-84% of canvas height/width, not tiny in the middle. It has a compact spherical sci-fi body, blue electric coils, antenna sparks, steel grey shell, charged and ready to burst, no legs. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Rail Cannon Sprite

```text
Create a 1024x1024 transparent PNG game sprite of a compact rail cannon defense device. The cannon must be centered and fill 78-90% of canvas width. It faces right, mounted on a reinforced platform, with a long rectangular barrel, red-orange charge emitter, steel grey armor plating, cyan power cable detail, and a silhouette suitable for one lane's last-defense cannon. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

---

## 6. Enemy Sprites

General enemy sprite settings:
- Output size: 512x512 or 1024x1024.
- Transparent background.
- Enemy faces left.
- Full body visible.
- Subject centered horizontally.
- Subject fills 75-88% of canvas height.
- Feet/base near lower 85-94% of canvas.
- No floor, no text.

### Basic Alien Robot

```text
Create a 1024x1024 transparent PNG full-body game sprite of a basic alien robot invader. The enemy must be centered and fill 75-88% of canvas height. It is side profile facing left, biomechanical silver-grey metal body, glowing violet energy core in chest, thin multi-jointed legs, alien sensor head, unsettling non-human shape, readable silhouette for a tower defense enemy. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Armored Alien Robot

```text
Create a 1024x1024 transparent PNG full-body game sprite of a bulky armored alien robot invader. The enemy must be centered and fill 75-88% of canvas height. It is side profile facing left, with thick silver-grey armor plates, heavy slow silhouette, partially covered red-violet core behind armor, reinforced legs, and clear tanky enemy identity. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Fast Alien Robot

```text
Create a 1024x1024 transparent PNG full-body game sprite of a fast alien robot invader. The enemy must be centered and fill 75-88% of canvas height. It is side profile facing left, slim spindly legs, lightweight silver-grey shell, exposed blue-violet wires, sharp forward-leaning silhouette, looks quick and fragile. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Shield Alien Robot

```text
Create a 1024x1024 transparent PNG full-body game sprite of a shield-bearing alien robot invader. The enemy must be centered and fill 75-88% of canvas height. It is side profile facing left, silver-grey robotic body protected by a front energy shield plate, blue-violet shield glow, defensive heavy silhouette, and clear shield identity. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

---

## 7. Projectiles And Collectibles

### Energy Orb Sprite

Settings:
- Output size: 512x512.
- Transparent background.
- Orb fills 78-88% of canvas.

```text
Create a 512x512 transparent PNG collectible sci-fi energy orb sprite. The orb must be centered and fill 78-88% of the canvas, not tiny in empty space. It is a bright cyan-white plasma sphere with radiant pixel core, small dark outline, and clean circular silhouette. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game asset readability.
```

### Turret Bullet

Settings:
- Output size: 256x256 or 512x512.
- Transparent background.
- Projectile fills 70-86% of canvas width.
- Points right.

```text
Create a 512x512 transparent PNG small sci-fi bullet / energy projectile sprite. The projectile must be centered, point right, and fill 70-86% of canvas width. It has an orange-yellow tracer core, tiny bright tip, and dark outline. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game asset readability.
```

### Frost Projectile

Settings:
- Output size: 256x256 or 512x512.
- Transparent background.
- Projectile fills 70-86% of canvas width.
- Points right.

```text
Create a 512x512 transparent PNG small icy projectile sprite. The projectile must be centered, point right, and fill 70-86% of canvas width. It is a cyan frost shard with cold mist trail pixels, bright ice-blue core, and dark outline. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game asset readability.
```

---

## 8. VFX Sprites

General VFX settings:
- Transparent PNG.
- No text.
- Effect fills 70-90% of canvas.
- Keep readable at mobile scale.

### Muzzle Flash

Settings:
- Output size: 256x256 or 512x512.
- Transparent background.
- Flash points right.
- Effect fills 75-90% width.

```text
Create a 512x512 transparent PNG pixel art muzzle flash VFX sprite. The flash must be centered, point right, and fill 75-90% of canvas width. Bright yellow-white core with orange sparks, horizontal burst, compact and readable. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, limited palette, clean readable VFX.
```

### Hit Spark

Settings:
- Output size: 512x512.
- Transparent background.
- Effect fills 70-86% of canvas.

```text
Create a 512x512 transparent PNG pixel art metal hit spark VFX sprite. The impact must be centered and fill 70-86% of the canvas. Cyan-white impact core with orange-yellow sparks radiating outward, compact and readable. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, limited palette, clean readable VFX.
```

### Enemy Death Burst

Settings:
- Output size: 512x512.
- Transparent background.
- Effect fills 75-90% of canvas.

```text
Create a 512x512 transparent PNG pixel art enemy death burst VFX sprite. The burst must be centered and fill 75-90% of the canvas. Small smoke puff with orange sparks and violet energy fragments, compact explosion for a robot enemy, readable but not huge. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, limited palette, clean readable VFX.
```

### EMP Pulse

Settings:
- Output size: 512x512.
- Transparent background.
- Ring fills 80-94% of canvas.

```text
Create a 512x512 transparent PNG pixel art EMP pulse VFX sprite. The electric ring must be centered and fill 80-94% of the canvas without being cropped. Cyan electric ring expanding outward, small lightning arcs, clean circular silhouette. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, limited palette, clean readable VFX.
```

### Rail Beam

Settings:
- Output size: 1024x256 or 2048x256.
- Transparent background.
- Beam fills 96-100% width.
- Beam points right.

```text
Create a 2048x256 transparent PNG horizontal rail cannon beam VFX sprite. The beam must fill 96-100% of the canvas width edge-to-edge, not be a short beam floating in the center. Bright white-orange core with red-orange glow, long thin beam pointing right, pixel art, clean and readable. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, limited palette, clean readable VFX.
```

---

## 9. Optional Spritesheets

Only use these if the generation tool can keep character consistency. If consistency is poor, generate static sprites first and animate in Unity.

### Basic Enemy Walk Spritesheet

Settings:
- Output size: 2048x512.
- 4 frames, 512x512 each.
- Transparent background.
- Same enemy in every frame.
- Enemy faces left.
- Enemy fills 75-88% height in each frame.

```text
Create a 2048x512 transparent PNG four-frame spritesheet of the same basic alien robot invader walking left. Each frame is 512x512, evenly spaced, same character design in every frame, side profile facing left, full body visible, enemy fills 75-88% of each frame height. No text, no frame numbers, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette.
```

### Turret Attack Spritesheet

Settings:
- Output size: 2048x512.
- 4 frames, 512x512 each.
- Transparent background.
- Same turret in every frame.
- Weapon faces right.
- Subject fills 75-88% height in each frame.

```text
Create a 2048x512 transparent PNG four-frame spritesheet of the same futuristic turret/gunner firing to the right. Each frame is 512x512, evenly spaced, same character design in every frame, subtle recoil, muzzle flash on final frame, full body visible, subject fills 75-88% of each frame height. No text, no frame numbers, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette.
```

### ArcReactor Idle Spritesheet

Settings:
- Output size: 2048x512.
- 4 frames, 512x512 each.
- Transparent background.
- Same generator in every frame.
- Device fills 75-88% height in each frame.

```text
Create a 2048x512 transparent PNG four-frame spritesheet of the same arc reactor energy generator pulsing cyan light. Each frame is 512x512, evenly spaced, same device design in every frame, subtle glow change, full object visible, device fills 75-88% of each frame height. No text, no frame numbers, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette.
```

## 10. Acceptance Checklist

Reject and regenerate if:

- The image contains text, logos, watermark, or copied IP.
- The sprite/icon/VFX is not transparent when it should be.
- A background is not full-bleed.
- A board does not fill most of the canvas.
- A sprite/icon is tiny with huge empty padding.
- Enemy does not face left.
- Unit weapon does not face right.
- The board has busy detail inside playable cells.
- The style becomes smooth/vector/3D instead of pixel art.
- Warm amber lights dominate the whole palette instead of acting as small focus points.
- The asset silhouette is unclear at small size.
