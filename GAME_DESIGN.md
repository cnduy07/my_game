# GAME DESIGN — Sci-fi Lane Defense

File nay mo ta thiet ke game hien hanh. Khi co quyet dinh moi ve gameplay, enemy, unit, economy, hay wave, cap nhat vao day.

---

## 1. Vision

Game tower defense 2D tren mobile, nhan manh vao:
- Luoi 5 hang x 9 cot, de doc tren man hinh dien thoai.
- Dat phong thu nhanh, quan ly energy, song sot qua cac wave alien robot.
- Cam giac chien dau hien dai: sung, beam, EMP, armor, spark, smoke, impact.
- Pixel art 16-bit doc ro, animation gon, VFX co luc.

Muc tieu chat luong: mot vertical slice nho nhung choi da tay, nhin ro, khong loi prefab/animation, co UI mobile gon.

---

## 2. Core Loop

1. Nguoi choi nhan energy orb hoac dat ArcReactor de sinh energy.
2. Chon seed packet/unit.
3. Dat unit vao o trong tren luoi.
4. Enemy spawn tu ben phai va di sang trai.
5. Unit ban/chan/no lam cham de bao ve lane.
6. Qua wave cuoi thi win; enemy vuot qua het tuyen cuu thi game over.

---

## 3. Grid va huong nhin

- Luoi: 5 rows x 9 cols.
- 1 cell = 1 Unity unit.
- Enemy di tu phai sang trai, art quay mat sang trai.
- Unit phong thu ban sang phai, art/vu khi quay sang phai.
- Khong lat art trong code neu sprite da ve dung huong.

---

## 4. Economy

Energy la tai nguyen chinh.

Nguon energy:
- Orb roi tu troi theo interval.
- ArcReactor sinh orb tai cho.

Nguyen tac:
- Orb phai duoc click/nhat moi cong energy.
- Unit chi dat duoc khi du energy va cooldown seed da xong.
- Gia/cooldown phai tao lua chon: spam unit yeu hay dau tu ArcReactor/bunker.

---

## 5. Unit hien tai

### ArcReactor
- Vai tro: tao energy.
- Khong tan cong.
- Can animation idle/pulse, orb spawn effect.

### Turret
- Vai tro: damage co ban.
- Ban dan sang phai khi co enemy cung hang.
- Can idle, attack, death hoac damage VFX.

### Bunker
- Vai tro: chan duong, HP cao.
- Khong ban.
- Dung `DamageStages` theo phan tram mau.
- Khuyen nghi stage:
  - `bunker_1`: lanh.
  - `bunker_2`: nut/mop nhe.
  - `bunker_3`: hu nang, gan sap.
  - Death: bien mat + VFX dong rieng, khong dung sprite no tinh.

### SnowGun
- Vai tro: damage thap/trung binh + slow.
- Can dan bang/impact bang.

### DroneEMP
- Vai tro: bomb no dien rong.
- Dat xuong, doi fuse, gay damage trong radius.
- Can VFX charge + EMP burst.

### Rail Cannon
- Vai tro: tuyen cuu cuoi moi hang, dung mot lan.
- Kich hoat khi enemy vuot tuyen.
- Ban beam xuyen row thay vi xe chay ngang, de giam cam giac clone PvZ.
- Can warning glow, beam VFX, SFX rail shot.

### Overcharge
- Vai tro: mechanic chu dong cua nguoi choi.
- Ton energy de buff mot row trong thoi gian ngan.
- Shooter trong row do ban nhanh hon va gay damage cao hon.
- Hop theme energy grid/sci-fi, tang quyet dinh trong combat.

---

## 6. Enemy hien tai va de xuat

### Basic Alien Robot
- HP/toc do trung binh.
- Silhouette ro o size nho.
- Can idle/walk/attack/death.

### Armored Alien
- HP cao, toc do cham hon.
- Giap day, core phat sang sau armor.
- Dung chung controller duoc neu rig path tuong thich.

### De xuat sau nay
- Fast Alien: HP thap, chay nhanh, than gay/chan dai.
- Shield Alien: co khien phia truoc, khang dan thuong.
- EMP-resistant Alien: it bi slow/stun.
- Mini-boss: to, cham, co animation rieng, xuat hien cuoi level.

---

## 7. Wave Design

Muc tieu wave:
- Wave dau day nguoi choi cach dat unit.
- Wave giua bat dau tron enemy armored/fast.
- Wave cuoi la huge wave, can dung du economy va bunker.

Quy tac can tuning:
- Thoi gian delay dau tran du de dat ArcReactor/unit dau.
- Khoang cach spawn khong lam man hinh qua dong.
- Armor enemy tang ti le tu tu, khong dot ngot.

---

## 8. Combat Feel

Can uu tien:
- Hit feedback: flash, spark, sound nho.
- Death feedback: enemy death animation ro; object tinh co smoke/spark/bien mat.
- Attack feedback: turret giat nhe, muzzle flash, dan co trail.
- UI feedback: seed cooldown ro, energy thay doi ro.

Khong nen:
- Dung sprite no tung tinh lam death effect.
- Lam qua nhieu chi tiet nho khong doc duoc tren mobile.
- Them content moi khi enemy/turret/bunker core chua polished.
