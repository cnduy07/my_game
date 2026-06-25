# ASSET GENERATION PROMPTS - Coreline Defense

Current generator constraint: the tool may currently create only **256x256** images for some assets.

Important: **256x256 is not valid as the final runtime size for GameScene board/background art.** Use 256x256 only for icons, gameplay object sprites, compact VFX, or temporary style references. Runtime board/background art must use the explicit wide target size in its own section.

## Global Rules

- Default output for icons, gameplay sprites, collectibles, projectiles, and compact VFX: **256x256**.
- Runtime board/background prompts override the default size. Do not use a square 256x256 board as final GameScene art.
- Pixel art only: no 3D, vector, smooth illustration, photorealism, anime, watercolor.
- No text, no logos, no watermark.
- Do not copy copyrighted characters, brands, or exact reference layouts.
- Coreline Defense is sci-fi, not fantasy: no castles, knights, zombies, superheroes, or medieval props.
- Cold dark palette is the base: deep navy, blue-grey, muted purple shadows.
- Amber/orange lights are small accents only: warning beacons, reactor lamps, signal flares.
- Cyan is reactor/energy/accent light.
- Keep silhouettes readable at small mobile size.

## View Direction Rules

- UI / HUD / seed icons are clickable or display assets. Use **front-facing / direct orthographic icon view**, centered like a readable emblem. Icons should not use gameplay side profile unless a prompt explicitly says it is a projectile direction.
- Gameplay unit sprites are board objects. Player-side combat units face **right** because enemies enter from the right. Use side profile or slight 3/4 side view, with weapon/armor readable toward the right.
- Gameplay bunker/cover sprites are static board objects. They must use right-facing side profile or slight 3/4 side view, with armor thickness visible on the enemy-facing right side. Do not generate these as flat front-facing bunker facades.
- Enemy sprites face **left**.
- Projectile and directional VFX sprites point **right**.
- Backgrounds and board concepts follow their own composition rules and do not need icon-style front view or unit-style side profile.

## Canvas Fill Rules

For every 256x256 sprite/icon/VFX image:

- The requested subject must fill the canvas intentionally.
- Do not create a tiny object in the center with huge empty padding.
- For sprites/icons/VFX on transparent background: subject/effect fills about **75-90%** of the canvas unless stated otherwise.
- For square background concepts: the scene fills the entire 256x256 image edge-to-edge.
- Keep a small safe margin so the subject is not cropped.

For runtime board/background images:

- Use the target aspect ratio and size listed in that prompt.
- Fill the full canvas edge-to-edge.
- Do not generate a square image and stretch it in Unity.
- For the GameScene board, the playable 5x9 area should fill **92-98%** of the wide canvas.

## Output File Naming

Save generated source PNGs under `Assets/Art/_source/` with these exact names. Do not overwrite current runtime files in `Assets/Art/` until the image is approved, cleaned, imported, and deliberately wired into prefab/scene references.

Naming rules:
- Use lowercase `snake_case`.
- Add `_256` for current 256x256 generated source images.
- Use `icon_` for UI/HUD/seed icons and `sprite_` for gameplay board objects.
- Enemy variants are generated from the approved Basic enemy reference, but still saved as separate output files.

Runtime board/backgrounds:
- `board_coreline_combat_grid_5x9.png` (official runtime board asset; replace only after approval)

Temporary square background/style references:
- `menu_hero_coreline_outpost_256.png`

UI skin sources:
- `ui_panel_9slice_source_256.png`
- `button_command.png` (procedural project asset; do not regenerate unless the UI spec changes)

Seed / HUD icons:
- `icon_energy_orb_256.png`
- `icon_arc_reactor_256.png`
- `icon_turret_256.png`
- `icon_bunker_256.png`
- `icon_snowgun_256.png`
- `icon_drone_emp_256.png`

Unit sprites:
- `sprite_arc_reactor_256.png`
- `sprite_turret_256.png`
- `sprite_bunker_stage_1_256.png`
- `sprite_bunker_stage_2_256.png`
- `sprite_bunker_stage_3_256.png`
- `sprite_snowgun_256.png`
- `sprite_drone_emp_256.png`
- `sprite_rail_cannon_256.png`

Enemy sprites:
- `sprite_enemy_basic_base_256.png`
- `sprite_enemy_armored_256.png`
- `sprite_enemy_fast_256.png`
- `sprite_enemy_shield_256.png`
- `sprite_enemy_heavy_256.png` reserved for future Heavy variant.

Projectiles / collectibles:
- `sprite_energy_orb_256.png`
- `sprite_bullet_turret_256.png`
- `sprite_projectile_frost_256.png`

VFX sources:
- `vfx_muzzle_flash_256.png`
- `vfx_hit_spark_256.png`
- `vfx_enemy_death_burst_256.png`
- `vfx_emp_pulse_256.png`
- `vfx_rail_beam_source_256.png`

## Global Negative Prompt

```text
photorealistic, 3D render, smooth vector art, anime, watercolor, blurry, antialiasing blur, noisy, excessive tiny details, text, logo, watermark, UI labels, copyright character, brand logo, medieval castle, knight, zombie, superhero, Marvel, Iron Man, Plants vs Zombies, human face closeup, gore, messy background, unreadable silhouette, cropped object, cut off object, tiny subject, small object in the center, too much empty padding, large blank margins, front-facing view when the prompt asks for gameplay side-view sprite, straight-on symmetrical facade when the prompt asks for gameplay bunker side-view sprite, flat front-facing gameplay bunker sprite
```

## Priority Batch 1

Generate these first:

1. `menu_hero_coreline_outpost_256.png`
2. `board_coreline_combat_grid_5x9.png` for runtime GameScene board art. Generate/repaint it at a wide 9:5 runtime size and replace the official file only after approval. If the generator only supports `256x256`, do not treat that square output as final board art.
3. Seed icons: `icon_energy_orb_256.png`, `icon_arc_reactor_256.png`, `icon_turret_256.png`, `icon_bunker_256.png`, `icon_snowgun_256.png`, `icon_drone_emp_256.png`.
4. Sprites: `sprite_enemy_basic_base_256.png`, `sprite_enemy_armored_256.png`, `sprite_turret_256.png`, `sprite_arc_reactor_256.png`.
5. VFX/collectible: `vfx_muzzle_flash_256.png`, `vfx_hit_spark_256.png`, `vfx_enemy_death_burst_256.png`, `sprite_energy_orb_256.png`.

---

## 1. Main Menu Hero Background Concept

Current settings:
- Output: `256x256`.
- Background: full image, not transparent.
- Composition: square menu mood concept, full-bleed edge-to-edge.
- Leave the upper center and lower center relatively calm/dark so UI can be overlaid later.
- No title, no logo, no menu text.

Future target:
- Regenerate or extend to `1920x1080` / `3840x2160`, 16:9.
- Future 16:9 version should preserve the same mood and leave space for title/menu.

Prompt:

```text
Create a 256x256 full-bleed square pixel art main menu background concept for a dark sci-fi tower defense game. The scene must fill the entire canvas edge-to-edge, not a small illustration inside a border. Show a distant fortified sci-fi outpost in a cold alien night landscape, with bunker walls, antenna towers, defense rails, and a faint alien breach gate on the horizon. Add deep navy sky, blue-grey mountains, muted purple shadows, a distant moon or planet, subtle stars and orbital debris. Use small warm amber warning beacons and reactor lamps as tiny focal lights, plus a few cyan reactor energy accents. Keep the upper center and lower center visually calm and dark enough for Unity title and menu text later. No text, no logo, no menu labels, no foreground character.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable large shapes, low noise, dark sci-fi mood.
```

---

## 2. First Contact Board Background Concept

Current settings:
- Output: `2304x1280`.
- Aspect ratio: `9:5`, matching the 9 columns x 5 rows gameplay board.
- Background: full image, not transparent.
- Composition: wide gameplay board **base texture**, not a finished scene illustration.
- Board/play area fills **92-98%** of canvas width and **82-94%** of canvas height.
- Must show clear `5 rows x 9 columns` readable structure with calm playable cells.
- No units, enemies, UI, text, rail cannon barrels, turrets, player devices, pickups, projectiles, or VFX.
- Unity will draw the final grid, rail cannon/last-defense devices, enemy entry markers, click targets, HUD, and tactical overlays. The image must not bake those gameplay objects into the background.

If only 256x256 generation is available:
- Do not use the square output as final runtime board art.
- Generate it only as a style/color reference, then extend/repaint/regenerate to `2304x1280`.

Prompt:

```text
Create a 2304x1280 full-bleed 9:5 pixel art gameplay board base texture for a sci-fi lane defense game. This is only the clean playable board surface that sits under Unity grid overlays, not a cinematic background, not a decorative frame, and not a finished gameplay screenshot. The image must be a wide rectangular board source, not a square composition. The board must fill 92-98% of the canvas width and 82-94% of the canvas height, with no large empty margins. Use a clear orthographic / top-down 2.5D board view. The playable area must be designed for exactly 5 horizontal lanes x 9 columns, but do not draw thick cell borders or strong baked grid lines. Use only very subtle tile seams or faint panel breaks, low contrast, so Unity can draw the actual gameplay grid on top. Keep the 45 center cells mostly flat, low-noise, dark blue-grey metal / stone plating so player units, enemies, projectiles, energy orbs, and VFX remain readable. Put heavier detail only on the outer rim: left side can have five small recessed anchor sockets or energy ports aligned to the lanes, but no cannon barrels, no turret devices, no rail cannon objects, and no protruding weapons. Right side can have a clean alien breach/entry strip with restrained red-orange warning marks, but no large bright arrows, no enemy markers, no spawn icons, and no oversized glowing gate objects. Add low bunker wall edges, cables, cracks, tiny amber reactor lamps, and a few cyan energy accents only around the rim. The board should feel like a clean tactical arena from a sci-fi outpost, not a big illustrated room. No UI, no text, no characters, no enemies, no player units, no pickups, no projectiles, no VFX, no giant decorative border covering cells, no thick black frame, no strong perspective, no diagonal walls crossing the playable area, no busy texture in the central cells.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable large shapes, low noise, clean gameplay readability.
```

Reject if:
- Output is square or will require horizontal stretching in Unity.
- Grid is unclear.
- Cell borders/grid lines are thick, high contrast, or already look like the final Unity grid.
- Board occupies less than 85% of image width.
- Center cells are too busy.
- It contains cannon barrels, turrets, rail cannon devices, projectile launchers, enemy markers, arrows, pickups, units, or VFX.
- Border/rim details compete with units or cover playable cells.
- The board looks like a finished illustration instead of a readable gameplay base.
- It looks medieval/fantasy instead of sci-fi.

---

## 3. UI Skin Concepts

All current UI outputs:
- Output: `256x256`.
- Transparent background.
- No text.
- These are style sources. Future UI skin can be regenerated as 9-slice-specific dimensions.

### 3.1 UI Panel 9-Slice Source

Current settings:
- Output: `256x256`.
- Transparent background.
- Panel frame fills **92-98%** of canvas.
- Border thickness about **10-18%** of canvas.
- Center is transparent or very dark and clean.

Future target:
- `512x512` or larger for clean 9-slice slicing.

Prompt:

```text
Create a 256x256 transparent PNG pixel art sci-fi UI panel frame source for later 9-slice scaling. The panel frame must fill 92-98% of the canvas, not sit tiny in the middle. Use a dark navy metal border, crisp square pixel corners, cyan thin edge lights, and tiny amber warning-light pixels in the corners. The center should be transparent or very dark and clean for text/content. No text, no icons, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean UI asset, dark sci-fi mood.
```

### 3.2 Button Source

Current settings:
- Output: `256x256`.
- Transparent background.
- Horizontal button plate centered.
- Button fills **94-98%** width and **34-46%** height.
- Leave transparent space above/below inside the square canvas.
- Single shared button background for all menu/HUD/modal buttons.
- Use Unity tint/color states for primary, secondary, danger, hover, pressed, and disabled variants instead of generating separate button PNGs.

Future target:
- `512x128` or similar horizontal button skin if the generator supports wider output later.

Prompt:

```text
Create a 256x256 transparent PNG pixel art sci-fi button background source named button. Inside the square canvas, make one horizontal dark blue-grey metal button plate centered vertically. The button plate must fill 94-98% of the canvas width and 34-46% of the canvas height, not be tiny with huge padding. Use a dark inner bevel, crisp pixel border, subtle cyan edge highlight, and tiny amber indicator pixels. Keep the center clean for Unity text and tinting. This is the single shared button background for normal, primary, secondary, danger, pressed, disabled, menu, HUD, and modal buttons. No text, no icons, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean UI asset, dark sci-fi mood.
```

---

## 4. Seed / HUD Icons

All current icon settings:
- Output: `256x256`.
- Transparent background.
- Icon centered.
- Icon fills **78-90%** of canvas.
- Keep only 5-10% safe margin.
- No text, logo, or frame.
- Icons are UI/display assets: use front-facing / direct orthographic icon view. Do not use gameplay side-view composition for seed icons.

Future target:
- `512x512` if higher-resolution UI icons are needed.

### Energy Icon / Orb

```text
Create a 256x256 transparent PNG pixel art energy orb UI icon. Use front-facing / direct orthographic icon view. The orb must be centered and fill 78-88% of the canvas, not a tiny orb in empty space. It is a collectible sci-fi energy orb with bright cyan-white plasma core, pixel glow, small dark outline, and clean circular power-cell silhouette. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability.
```

### ArcReactor Icon

```text
Create a 256x256 transparent PNG pixel art ArcReactor seed UI icon. Use front-facing / direct orthographic icon view, centered like a clickable emblem, not a gameplay side-view sprite. The device must fill 78-90% of the canvas. Show a compact sci-fi energy generator with cyan glowing core, steel grey casing, small amber accent lights, and a clear readable silhouette. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability, dark sci-fi mood.
```

### Turret Icon

```text
Create a 256x256 transparent PNG pixel art Turret seed UI icon. Use front-facing / direct orthographic icon view, centered like a clickable emblem, not a gameplay side-view sprite. The turret/gunner must fill 78-90% of the canvas. Show a compact armored turret/gunner with readable minigun identity, steel grey armor, orange accent lights, and a small red visor detail. Clear weapon silhouette, but do not make it a right-facing gameplay sprite. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability, dark sci-fi mood.
```

### Bunker Icon

```text
Create a 256x256 transparent PNG pixel art Bunker seed UI icon. Use front-facing / direct orthographic icon view, centered like a clickable emblem, not the gameplay bunker sprite. The bunker icon must fill 80-90% of the canvas width. A symmetrical armored front facade is allowed here because this is a UI icon. Show a heavy squat sci-fi armored barricade with thick steel plates, central glowing slit, amber warning lights, and a sturdy defensive silhouette. Do not use side-profile composition for this icon. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability, dark sci-fi mood.
```

### SnowGun Icon

```text
Create a 256x256 transparent PNG pixel art SnowGun seed UI icon. Use front-facing / direct orthographic icon view, centered like a clickable emblem, not a gameplay side-view sprite. The frost cannon defender must fill 78-90% of the canvas. Show steel grey body, icy cyan tubes, frosted barrel identity, and a clear cannon silhouette. Do not make it a right-facing gameplay sprite. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability, dark sci-fi mood.
```

### DroneEMP Icon

```text
Create a 256x256 transparent PNG pixel art DroneEMP seed UI icon. Use front-facing / direct orthographic icon view, centered like a clickable emblem, not a gameplay side-view sprite. The EMP drone must fill 78-88% of the canvas. It is a compact hovering spherical device with blue electric coils, antenna sparks, steel grey shell, and clear silhouette. No text, no logo, no frame.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game icon readability, dark sci-fi mood.
```

---

## 5. Unit Sprites

All current unit sprite settings:
- Output: `256x256`.
- Transparent background.
- Full body/object visible.
- Subject centered horizontally.
- Subject fills **75-88%** of canvas height unless noted.
- Base/feet sit near lower **85-94%** of canvas with small safe margin.
- Unit weapons face right.
- Static defensive covers face right too: use side profile or slight 3/4 side view, with armor thickness visible toward the enemy side. Avoid straight-on front view.
- Non-weapon gameplay devices can use slight 3/4 gameplay view, but should not look like flat UI icons.
- No floor, no large shadow, no text.

Future target:
- `512x512` or `1024x1024` if rigging/cutting/animation needs more detail.

### ArcReactor Sprite

```text
Create a 256x256 transparent PNG full-body game sprite of a stationary sci-fi ArcReactor energy generator. Use slight 3/4 gameplay side view so it reads as a board object, not a flat UI icon. The object must be centered and fill 75-88% of the canvas height, with the base near the bottom but not cropped. Show a glowing cyan-blue reactor core mounted on a compact armored base, energy rings, steel grey casing, amber warning lights, and a silhouette that fits one tower defense grid cell. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Turret Sprite

```text
Create a 256x256 transparent PNG full-body game sprite of a futuristic armored defender turret/gunner. The subject must be centered and fill 75-88% of canvas height, not tiny in the middle. It faces right, weapon pointing right, with a large multi-barrel minigun, steel grey power armor, orange accent lights, small red visor, stable stance, and a silhouette that fits one tower defense grid cell. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Bunker Stage 1

Current setting note:
- Bunker is wide, so it should fill **80-92%** width and **55-75%** height.

```text
Create a 256x256 transparent PNG full-body game sprite of an undamaged heavy sci-fi bunker barricade for a side-view lane defense game. The bunker must be centered and fill 80-92% of canvas width and 55-75% of canvas height, with no huge empty padding. Use right-facing side profile or slight 3/4 side view, not a straight-on front view. The player should see the bunker from one side, with its reinforced armor thickness and heavy plating facing enemies on the right. Show squat steel armor plates, thick dark outline, a small horizontal glowing side slit, amber warning lights, and sturdy defensive silhouette. Avoid symmetrical front facade. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Bunker Stage 2

```text
Create a 256x256 transparent PNG full-body game sprite of the same heavy sci-fi bunker barricade as stage 1, moderately damaged. Keep the exact same right-facing side profile / slight 3/4 side view, not a straight-on front view. The bunker must be centered and fill 80-92% of canvas width and 55-75% of canvas height. Add visible cracks, bent steel plates, small scorch marks, and flickering warning lights while keeping the same overall silhouette and armor thickness facing enemies on the right. Avoid symmetrical front facade. Not exploding. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Bunker Stage 3

```text
Create a 256x256 transparent PNG full-body game sprite of the same heavy sci-fi bunker barricade as previous stages, badly damaged. Keep the exact same right-facing side profile / slight 3/4 side view, not a straight-on front view. The bunker must be centered and fill 80-92% of canvas width and 55-75% of canvas height. Add large cracks, broken armor plates, exposed cables, dim warning light, but keep it recognizable as the same bunker with armor thickness facing enemies on the right. Avoid symmetrical front facade. Not exploding. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### SnowGun Sprite

```text
Create a 256x256 transparent PNG full-body game sprite of a futuristic frost cannon defender. The subject must be centered and fill 75-88% of canvas height. It faces right, weapon pointing right, with steel grey armor, icy cyan coolant tubes, frosted barrel, small cold mist pixels, and a silhouette that fits one tower defense grid cell. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### DroneEMP Sprite

```text
Create a 256x256 transparent PNG game sprite of a small hovering EMP attack drone. Use right-facing side profile or slight 3/4 right-facing gameplay view, not a front-facing UI icon. The drone must be centered and fill 70-84% of canvas height/width, not tiny in the middle. It has a compact spherical sci-fi body, blue electric coils, antenna sparks, steel grey shell, charged and ready to burst, no legs. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Rail Cannon Sprite

Current setting note:
- Rail cannon is wide, so it should fill **82-94%** width and **45-68%** height.

```text
Create a 256x256 transparent PNG game sprite of a compact rail cannon defense device. The cannon must be centered and fill 82-94% of canvas width, not tiny in the middle. It faces right, mounted on a reinforced platform, with a long rectangular barrel, red-orange charge emitter, steel grey armor plating, cyan power cable detail, and a silhouette suitable for one lane's last-defense cannon. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

---

## 6. Enemy Sprites

All current enemy sprite settings:
- Output: `256x256`.
- Transparent background.
- Enemy faces left.
- Full body visible.
- Subject centered horizontally.
- Subject fills **75-88%** of canvas height.
- Feet/base near lower **85-94%** of canvas.
- `Basic Alien Robot` is the canonical base enemy for the whole enemy family.
- Generate and approve the Basic enemy first. Use that approved image as image-reference/image-to-image input for Armored, Fast, Shield, Heavy, and future variants whenever the tool supports references.
- All enemy variants must preserve the same canvas footprint, pivot, ground line, facing direction, side-profile pose, body proportions, head/chest-core/hip/limb joint positions, and rig-friendly separation as the Basic enemy.
- Variants should add modules on top of the Basic enemy: armor plates, shield device, exposed wires, speed fins, color/core changes. Do not redesign a different creature with a different height, limb count, pose, or silhouette footprint.
- No floor, no text.

Future target:
- `512x512` or `1024x1024` if rigging/cutting/animation needs more detail.

### Basic Alien Robot

```text
Create a 256x256 transparent PNG full-body game sprite of the canonical base alien robot invader for all future enemy variants. The enemy must be centered and fill 75-88% of canvas height. It is side profile facing left, biomechanical silver-grey metal body, glowing violet energy core in chest, thin multi-jointed legs, alien sensor head, unsettling non-human shape, readable silhouette for a tower defense enemy. Keep a rig-friendly structure: separated head, chest/core, hips, limbs, and visible joints. This exact body size, pose, pivot, ground line, joint layout, and silhouette footprint will be reused by Armored, Fast, Shield, and future variants. No floor, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, readable silhouette, low noise, dark sci-fi mood.
```

### Armored Alien Robot

Use with the approved `Basic Alien Robot` image reference. Paste this as the variant modifier, not as a standalone redesign prompt.

```text
Add thick silver-grey armor plates over the same base body, partially cover the violet core behind armor, reinforce the same legs with plating, and make the tanky identity clear while preserving the Basic enemy proportions.
```

### Fast Alien Robot

Use with the approved `Basic Alien Robot` image reference. Paste this as the variant modifier, not as a standalone redesign prompt.

```text
Remove some armor plating, add exposed blue-violet wires, small speed fins, lighter leg plating, and sharper forward energy accents so it reads as quick and fragile while preserving the Basic enemy proportions.
```

### Shield Alien Robot

Use with the approved `Basic Alien Robot` image reference. Paste this as the variant modifier, not as a standalone redesign prompt.

```text
Add an energy shield plate or shield emitter on the enemy's leading left side, with blue-violet shield glow and defensive identity, while preserving the Basic enemy proportions.
```

---

## 7. Projectiles And Collectibles

All current settings:
- Output: `256x256`.
- Transparent background.
- No text.

### Energy Orb Sprite

```text
Create a 256x256 transparent PNG collectible sci-fi energy orb sprite. The orb must be centered and fill 78-88% of the canvas, not tiny in empty space. It is a bright cyan-white plasma sphere with radiant pixel core, small dark outline, and clean circular silhouette. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game asset readability.
```

### Turret Bullet

Current setting note:
- Projectile should fill **70-86%** width and point right.

```text
Create a 256x256 transparent PNG small sci-fi bullet / energy projectile sprite. The projectile must be centered, point right, and fill 70-86% of canvas width. It has an orange-yellow tracer core, tiny bright tip, and dark outline. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game asset readability.
```

### Frost Projectile

Current setting note:
- Projectile should fill **70-86%** width and point right.

```text
Create a 256x256 transparent PNG small icy projectile sprite. The projectile must be centered, point right, and fill 70-86% of canvas width. It is a cyan frost shard with cold mist trail pixels, bright ice-blue core, and dark outline. No text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, bold dark outline, limited palette, clean game asset readability.
```

---

## 8. VFX Sprites

All current settings:
- Output: `256x256`.
- Transparent background.
- No text.
- Effect fills **70-90%** of canvas unless stated otherwise.
- Use true alpha transparency. The PNG must not contain a dark square, dark rectangle, solid-color backdrop, colored canvas, checkerboard, or hidden background pixels around the effect.
- Keep sparks/debris compact. Avoid large random square color chunks that read as broken pixels.

Future target:
- Regenerate at higher resolution only if Unity VFX sprites need more detail.

### Muzzle Flash

```text
Create a 256x256 transparent PNG pixel art muzzle flash VFX sprite. The flash must be centered, point right, and fill 45-62% of canvas width, compact enough for a turret muzzle. Bright yellow-white core with orange sparks, horizontal burst, readable but not huge. Use true alpha transparency only around the flash. No dark rectangle background, no square backdrop, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, limited palette, clean readable VFX.
```

### Hit Spark

```text
Create a 256x256 transparent PNG pixel art metal hit spark VFX sprite. The impact must be centered and fill 55-72% of the canvas. Cyan-white impact core with orange-yellow sparks radiating outward, compact and readable. Use true alpha transparency only around the spark. No dark rectangle background, no square backdrop, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, limited palette, clean readable VFX.
```

### Enemy Death Burst

```text
Create a 256x256 transparent PNG pixel art enemy death burst VFX sprite. The burst must be centered and fill 58-76% of the canvas. Small smoke puff with orange sparks and violet energy fragments, compact explosion for a robot enemy, readable but not huge. Use true alpha transparency only around the burst. No dark rectangle background, no square backdrop, no oversized colored square chunks, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, limited palette, clean readable VFX.
```

### EMP Pulse

```text
Create a 256x256 transparent PNG pixel art EMP pulse VFX sprite. The electric ring must be centered and fill 62-78% of the canvas without being cropped. Cyan electric ring expanding outward, small lightning arcs, clean circular silhouette. Use true alpha transparency inside and outside the ring. No dark square background, no filled dark center, no square backdrop, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, limited palette, clean readable VFX.
```

### Rail Beam Source

Current settings:
- Output: `256x256`.
- Transparent background.
- Horizontal beam fills **96-100%** width and **15-28%** height.

Future target:
- `1024x256` or `2048x256` horizontal beam strip.

```text
Create a 256x256 transparent PNG pixel art rail cannon beam source. Inside the square canvas, make one horizontal rail beam centered vertically. The beam must fill 96-100% of the canvas width edge-to-edge and 10-20% of the canvas height, not be a short beam floating in the center. Bright white-orange core with red-orange glow, long thin beam pointing right, clean and readable. Use true alpha transparency around the beam. No dark rectangle background, no square backdrop, no text, no logo.

Chunky retro 16-bit pixel art, crisp pixels, limited palette, clean readable VFX.
```

---

## 9. Optional Animation Sources

Current 256x256 constraint:
- Do **not** generate full spritesheets yet if the tool only supports 256x256.
- Generate single static sprites first.
- Animation can be done in Unity using rig/Animator or generated later when larger/correct aspect sizes are available.

Future target for spritesheets:
- 4-frame strips should be `1024x256` if each frame is 256x256.
- Higher quality future target: `2048x512` if each frame is 512x512.

Future prompt example:

```text
Create a four-frame transparent PNG spritesheet of the same character, evenly spaced frames, consistent design in every frame, no text, no frame numbers, no logo. Each frame should keep the subject filling 75-88% of frame height.
```

---

## 10. Acceptance Checklist

Reject and regenerate if:

- The image contains text, logos, watermark, or copied IP.
- The sprite/icon/VFX is not transparent when it should be.
- A background is not full-bleed.
- A board does not fill most of the canvas.
- A sprite/icon/VFX is tiny with huge empty padding.
- Enemy does not face left.
- Unit weapon does not face right.
- A seed/HUD icon is generated as a gameplay side-view sprite instead of a front-facing/direct icon.
- A gameplay unit/bunker sprite is generated as a front-facing UI icon or flat facade instead of a side-view board object.
- An enemy variant changes the Basic enemy footprint, pivot, height, pose, limb layout, or rig proportions instead of deriving from the approved Basic enemy.
- The board has busy detail inside playable cells.
- The style becomes smooth/vector/3D instead of pixel art.
- Warm amber lights dominate the whole palette instead of acting as small focus points.
- The asset silhouette is unclear at small size.
