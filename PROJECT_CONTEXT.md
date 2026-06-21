# PROJECT CONTEXT — Vai tro, workflow, source of truth

File nay la ban tom tat ngan de Codex va chu project doc lai truoc moi phien lam viec dai. Neu context chat bi phinh to, hay bat dau bang viec doc file nay truoc, sau do doc `TASKS.md`, `PROJECT_PROGRESS.md`, `ART_STYLE.md`.

---

## 1. Project la gi

- Game Unity 2D tower defense phong cach sci-fi tuong lai.
- Cam hung co che tu lane/grid tower defense, nhung asset/ten goi phai la ban goc, khong dung IP/brand co ban quyen.
- Core loop hien tai: dat unit tren luoi 5x9, sinh energy, chon seed packet, enemy di tu phai sang trai, unit ban sang phai, wave tang dan, co thang/thua.
- Art direction: retro 16-bit pixel art, bold black outline, armor xam/cam cho phe thu, alien robot bac/tim/xanh cho dich.

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
- `GAME_DESIGN.md`: game design hien hanh.
- `TASKS.md`: viec dang lam, viec tiep theo, viec chia cho chu project/Codex.
- `CONTENT_PLAN.md`: danh sach sprite, rig, animation, VFX, SFX, UI asset can lam.
- `TECH_PLAN.md`: systems, architecture, performance, testing.
- `RELEASE_CHECKLIST.md`: App Store/Google Play checklist.

---

## 5. Workflow moi phien

1. Doc `PROJECT_CONTEXT.md`, `TASKS.md`, va file lien quan den task.
2. Kiem tra `git status --short`; khong revert thay doi khong phai cua Codex.
3. Neu task lien quan animation/prefab, kiem tra ca code C# va Unity YAML.
4. Sua nho, verify bang `git diff --check`; neu co the thi verify trong Unity.
5. Cap nhat `TASKS.md`/file planning khi co thay doi huong di ro rang.
6. Bao lai ngan gon: da sua gi, can chu project lam gi trong Unity.

---

## 6. Nguyen tac chat luong

- Lam vertical slice nho nhung polished truoc khi mo rong content.
- Gameplay chay dung bang ban xam/trang thai co ban truoc, polish sau.
- Damage state tinh theo gameplay state; effect dong nhu no, khoi, spark nen tach thanh VFX prefab.
- Mot prefab co Animator/SpriteSkin nen dung Animator; object tinh co SpriteRenderer nen dung component nhu `DamageStages`.
- Mobile first: doc duoc tren man hinh nho, touch de bam, FPS on dinh, build size hop ly.

