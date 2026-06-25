# PROJECT CONTEXT — Vai tro, workflow, source of truth

File nay la ban tom tat ngan de Codex va chu project doc lai truoc moi phien lam viec dai. Neu context chat bi phinh to, hay bat dau bang viec doc file nay truoc, sau do doc `TASKS.md`, `PROJECT_PROGRESS.md`, `ART_STYLE.md`.

---

## 1. Project la gi

- Game Unity 2D tower defense phong cach sci-fi tuong lai.
- Cam hung co che tu lane/grid tower defense, nhung asset/ten goi phai la ban goc, khong dung IP/brand co ban quyen.
- Core loop hien tai: dat unit tren luoi 5x9, sinh energy, chon seed packet, enemy di tu phai sang trai, unit ban sang phai, wave tang dan, co thang/thua.
- Art direction: retro 16-bit pixel art, bold black outline, armor xam/cam cho phe thu, alien robot bac/tim/xanh cho dich.
- Asset view rule: UI/HUD/seed icon dung front-facing/direct icon view; gameplay unit/cover/object dung side profile hoac slight 3/4 view, phe thu quay phai va enemy quay trai.
- Enemy art rule: `Basic Alien Robot` la base enemy chinh; cac enemy variant phai derive tu base nay va giu cung footprint/pivot/pose/rig proportions de reuse skeleton/animation.

---

## 2. Vai tro cua chu project

Chu project la nguoi quyet dinh cam giac va chat luong cuoi cung.

Lam tot nhat:
- Choi thu trong Unity va tren dien thoai that.
- Danh gia cam giac: toc do, damage, kho/de, hieu ung co "da" khong.
- Tao/chon sprite, rig, animation clip, VFX va audio bang mat/nghe that.
- Quyet dinh visual style, fantasy, monetization, store presentation.
- Cap nhat nhan xet sau moi lan test: cai gi thich, cai gi kho chiu, cai gi can sua.

Nen bao cho Codex bang input cu the:
- "Enemy di cham/nhanh qua."
- "Death animation khong chay."
- "Prefab nay toi vua rig xong, kiem tra reference giup."
- "Can goi y sprite/unit/enemy tiep theo."

---

## 3. Vai tro cua Codex

Codex la engineering/game-dev copilot cho project nay.

Lam tot nhat:
- Doc codebase, prefab, scene, controller, `.md` truoc khi sua.
- Sua C# gameplay systems, prefab YAML, Animator Controller parameter/transition khi ro rang.
- Debug mismatch giua code va Unity data: missing reference, sai parameter, sai GUID/fileID, component thieu.
- De xuat architecture: ScriptableObject configs, pooling, VFX/SFX system, save/load, mobile UI.
- Tao va cap nhat tai lieu planning.
- Khuyen nghi art/gameplay pipeline, nhung khong thay the viec test bang mat va cam giac cua chu project.

Can than trong cac viec:
- Khong revert thay doi nguoi dung da lam.
- Khong sua prefab phuc tap neu can Unity Editor sinh data/rig/weights/preview bang mat.
- Khi sua Unity YAML, chi sua khi GUID/fileID va cau truc ro rang.
- Khi nghi ngo, noi ro phan nao can kiem tra lai trong Unity Inspector.

---

## 4. Source of truth

- `PROJECT_CONTEXT.md`: vai tro, workflow, doc gi truoc.
- `PROJECT_PROGRESS.md`: lich su tien do va quyet dinh da lam.
- `ART_STYLE.md`: style art, prompt, import convention, rig pipeline.
- `ART_STYLE.md`: approved visual direction va cach translate mood/palette sang sci-fi Coreline Defense.
- `ASSET_GENERATION_PROMPTS.md`: prompt pack chi tiet de gen menu background, board, UI skin, seed icons, unit/enemy sprites va VFX.
- `UI_SPEC.md`: UI source of truth. Must be read before UI work; defines exact palette, button asset path, button sizes/states, menu layout, and end modal layout.
- `GAME_DESIGN.md`: game design hien hanh.
- `TASKS.md`: viec dang lam, viec tiep theo, viec chia cho chu project/Codex.
- `NEWS_TASK.md`: current UI/UX upgrade rules va task order cho runtime-generated UI; neu lech path thi map sang file that trong repo.
- `CONTENT_PLAN.md`: danh sach sprite, rig, animation, VFX, SFX, UI asset can lam.
- `TECH_PLAN.md`: systems, architecture, performance, testing.
- `RELEASE_CHECKLIST.md`: App Store/Google Play checklist.
- `OVERNIGHT_SPRINT.md`: snapshot objective/constraint/work blocks cho phien nang cap tu chu 2026-06-23.
- `GameSystems > GameBalance` trong `GameScene`: source of truth tam thoi cho stats gameplay chinh.
- `GameUiController` tren `GameSystems`: source of truth tam thoi cho runtime HUD uGUI/TextMeshPro. Cac IMGUI cu chi la debug khi bat `showDebugImGui`.
- `CampaignIntel`: source of truth code-side cho campaign map node positions/types, enemy mix, threat label, recommended tools va pressure score.
- Frontend UI (`MainMenuScene`, `SettingsScene`, `HowToPlayScene`, `MissionMapScene`) hien da duoc bake thanh scene-authored `FrontendCanvas/SafeAreaRoot` de co the chinh tay trong Unity Editor. `FrontendUiController` chi bind button/slider/toggle va co fallback runtime-build neu scene chua co authored UI.
- Rebuild authored frontend UI bang `Tools > Coreline > Rebuild Current Frontend Authored UI` hoac `Tools > Coreline > Rebuild All Frontend Authored UI`. Sau khi rebuild, kiem tra lai scene bang mat vi generator se ghi de `FrontendCanvas`.
- GameScene runtime HUD van sinh bang C# uGUI/TMP tu `GameUiController`; chua chuyen het sang scene-authored/prefab-authored UI.
- Procedural UI/board/VFX polish hien duoc ket hop voi generated art: menu hero, board art, button/panel skin, seed icons, gameplay sprites, va sprite VFX prefab. Asset final/rig/audio van theo `CONTENT_PLAN.md`/`ART_STYLE.md`.
- Generated PNG runtime mapping hien dung `Assets/Scripts/Editor/GeneratedArtApplier.cs`: import PNG, map gameplay prefabs, assign UI/board scene refs, rebuild VFX prefabs, va gan `CombatVfxSettings`. Static PNG preview co the tam tat `SpriteSkin`; rig final phai bat lai SpriteSkin/Animator sau khi co bone/weights dung.
- Sprite sizing/import la phan viec engineering: PPU theo canh lon nhat texture de giu world size on dinh, prefab `SpriteRenderer.size = 1x1`, tune kich thuoc gameplay bang prefab Transform scale khi can.

---

## 5. Workflow moi phien

1. Doc `PROJECT_CONTEXT.md`, `TASKS.md`, va file lien quan den task.
2. Kiem tra `git status --short`; khong revert thay doi khong phai cua Codex.
3. Hieu ro yeu cau, context, code path, scene/prefab/data lien quan truoc khi code. Neu yeu cau mo ho hoac co rui ro sai huong, hoi lai chu project truoc.
4. Dua ra plan ngan: muc tieu, phuong an kha thi, tradeoff, va phuong an Codex tu chon la toi uu theo codebase hien tai.
5. Neu task lien quan animation/prefab, kiem tra ca code C# va Unity YAML.
6. Sau khi code, verify bang build/static check/QA san co neu kha thi; toi thieu chay `git diff --check`.
7. Cap nhat `TASKS.md`/`PROJECT_PROGRESS.md`/file planning phu hop khi co feature, quyet dinh, task moi, hoac thay doi trang thai.
8. Bao lai ngan gon: da sua gi, da verify gi, viec con ton dong, va can chu project test gi trong Unity.

Workflow bat buoc cho feature lon:
- Khong nhay vao code khi chua doc docs/context va file lien quan.
- Khong lan man sang refactor ngoai pham vi neu khong can de dat muc tieu.
- Uu tien cong nghe/pattern da co trong project: Unity uGUI/TMP runtime UI, ScriptableObject/scene config hien hanh, QA/editor tooling co san.
- Neu co nhieu huong, Codex phai tu danh gia va chon huong tot nhat, sau do moi implement.

---

## 6. Nguyen tac chat luong

- Lam vertical slice nho nhung polished truoc khi mo rong content.
- Gameplay chay dung bang ban xam/trang thai co ban truoc, polish sau.
- Damage state tinh theo gameplay state; effect dong nhu no, khoi, spark nen tach thanh VFX prefab.
- Mot prefab co Animator/SpriteSkin nen dung Animator; object tinh co SpriteRenderer nen dung component nhu `DamageStages`.
- Tune gameplay stat tren `GameBalance`; tune visual/reference/rig/MuzzlePoint tren prefab.
- Mobile first: doc duoc tren man hinh nho, touch de bam, FPS on dinh, build size hop ly.
- UI color semantics: `accent`/cyan cho primary action, selected state, slider/toggle; `hot`/red chi cho destructive/danger nhu EXIT; success cho cleared/completed; warning cho pressure/canh bao. Khong dung `hot` cho slider handle, toggle, START GAME, hay normal selected state.
- Strict UI button rule: `Assets/UI/button_command.png` is the only clickable-action button background. Import/use with Point filtering, never bilinear. Button visual states use color modulate only: normal white, hover light blue tint, pressed darker blue tint plus 2px down offset, disabled grey alpha. Do not draw extra frame/notch/stripe/line overlays on buttons. Standard display sizes: menu `220x72`, popup `172x58`, secondary/deploy/back `188x58`.
- Button layout rule: button sprites must use fixed display sizes for their context and be centered/aligned inside available panels; do not stretch buttons to fill mission detail columns, layout cells, or large panels.
- Text rule for runtime UI: use Thaleah Pixel Font via TMP asset `Assets/Resources/Fonts/ThaleahFat SDF.asset`, no text shadow, no text outline, `Truncate` overflow instead of `Ellipsis`, and fixed spec sizes for menu/popup buttons. Runtime code must call `UiFont.Apply(...)`/`UiFont.ApplyToChildren(...)` for generated or scene-authored TMP labels.
- Font workflow: after importing/updating `Assets/Thaleah_PixelFont/Materials/ThaleahFat_TTF.ttf`, run `Tools > Coreline > Apply Thaleah Pixel Font` once in Unity. The tool creates or repairs the TMP font asset if missing/invalid, sets the TMP default font, and applies it to authored frontend scene labels.
- End-state modal rule: Victory/Defeat can dark-neutral, clean, centered, asset-led. Khong dung transparent green/red full-screen overlay, terminal map backdrop, hoac hop mau rong de trang tri.
- Khi lam dep toan game, uu tien runtime polish khong pha gameplay truoc; sau do moi thay bang sprite/UI skin/VFX prefab final co license va source ro rang.
