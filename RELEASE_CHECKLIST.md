# RELEASE CHECKLIST — App Store va Google Play

File nay la checklist dai han de dua game len iOS App Store va Google Play.

---

## 1. Milestones

### Milestone 1: Playable Prototype

- Core gameplay chay.
- Dat unit, enemy di, ban, damage, wave, win/lose.
- Khong co Console error chinh.
- Co docs va version control.

### Milestone 2: Vertical Slice

- 1 level playable dep.
- 1 enemy basic polished.
- 1 armored enemy.
- Turret, bunker, ArcReactor polished.
- VFX/SFX co ban.
- UI mobile co ban.
- Choi tren dien thoai that.
- Tutorial toi thieu cho energy/dat unit.

### Milestone 3: Content Expansion

- Nhieu unit/enemy/level.
- Balance pass.
- Tutorial.
- Save/load progress.
- Settings.
- Level select/unlock flow.
- Projectile effects va enemy counters co vai tro ro.

### Milestone 4: Mobile Optimization

- Object pooling.
- FPS on dinh tren device yeu.
- Build size hop ly.
- Texture/audio compression.
- Crash-free internal testing.
- Safe area/touch target pass.
- Battery/thermal sanity check.

### Milestone 5: Store Release

- Store listing.
- Screenshots/trailer.
- Privacy policy.
- App icon.
- Release build signed.
- Internal/closed testing.
- Submit review.

### Milestone 6: Post-launch Readiness

- Crash/feedback monitoring.
- Balance updates nho.
- Content update plan.
- Store page A/B notes neu co.
- Monetization chi them khi core retention on.

---

## 2. iOS App Store

Can co:
- Apple Developer account.
- Bundle Identifier.
- App icon day du size.
- Launch screen.
- Privacy policy URL.
- App Privacy labels.
- Age rating.
- Screenshots cho cac device size Apple yeu cau.
- TestFlight build.
- Release notes.

Unity/Xcode:
- iOS Build Support installed.
- Signing team configured.
- Minimum iOS version quyet dinh.
- Orientation locked/handled.
- Safe area handled.
- No unnecessary permissions.

---

## 3. Google Play

Can co:
- Google Play Console account.
- Package name.
- App signing.
- Privacy policy URL.
- Data safety form.
- Content rating questionnaire.
- Store listing.
- Feature graphic.
- Screenshots.
- Internal testing track.
- Release notes.

Unity/Android:
- Android Build Support installed.
- Keystore backed up safely.
- Target SDK phu hop yeu cau Google Play hien hanh.
- ARM64 build.
- No unnecessary permissions.

---

## 4. Store Assets

Can san xuat:
- App icon.
- Game title/logo.
- Screenshots portrait/landscape tuy orientation.
- Short trailer/gameplay clip.
- Feature graphic cho Google Play.
- App description ngan/dai.
- Keywords.

Nguyen tac screenshot:
- Hien gameplay that, khong chi menu.
- UI ro tren mobile.
- 3-5 selling points: dat unit, wave, boss/enemy, VFX, victory/progression.

---

## 5. Compliance va Privacy

Can quyet dinh som:
- Game co ads khong.
- Co IAP khong.
- Co analytics khong.
- Co thu thap data khong.
- Co account/login khong.

Neu co SDK ben thu ba:
- Doc privacy/data safety yeu cau.
- Cap nhat privacy policy.
- Kiem tra permission trong Android manifest/iOS plist.

---

## 6. QA Checklist

Gameplay:
- New game chay duoc.
- Win/lose/restart.
- Pause/resume.
- Khong softlock.
- Khong crash khi spam touch.

Mobile:
- Safe area.
- Text/UI khong bi cat.
- Touch target du lon.
- FPS on dinh.
- Audio volume hop ly.
- App background/foreground khong loi.

Build:
- Version number tang dung.
- Build release khong co debug UI/log qua nhieu.
- Build cai duoc tren device.
- Khong co missing reference.

Automation nen co:
- Prefab/reference validator.
- Animator parameter validator.
- GameBalance sanity check.
- Audio clip/null/volume validator.
- Basic PlayMode smoke test.

---

## 7. Launch Strategy

De xuat:
- Test noi bo truoc.
- Soft launch nho neu co the.
- Thu thap feedback ve kho/de, UI, FPS.
- Chi them monetization nang khi core retention on.

Can theo doi:
- Crash rate.
- Session length.
- Level fail rate.
- Ad/IAP impact neu co.
- User feedback ve do kho va ro rang UI.
