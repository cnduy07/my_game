# UI_SPEC.md - Coreline Defense

Codex must read this before implementing any UI scene.

## Visual Language

Style: pixel art, dark sci-fi

Palette:
- Background: `#0D1117`
- Panel: `#111827`
- Border: `#1E90A8` for frames, not button recolors
- Accent: `#FF6B35` for highlight, danger, exit button label only
- Text: `#E8EAF0`
- Text muted: `#6B7280`

## Buttons - Required Rules

- Do not use the default engine button look.
- `Assets/UI/button_command.png` is the only button background allowed for clickable actions.
- Native size: `288 x 104 px`.
- Render mode: Point/Nearest filter. Do not use bilinear or trilinear filtering.
- Button states use color modulate on the same sprite, not separate sprites.
- Do not draw extra button notches, stripes, glow, or frame rectangles in code.
- Do not stretch a button to fill a panel, table cell, or mission detail column. Use the fixed display size for that context and center/align it inside the available space.

Display sizes:
- Main menu buttons: `220 x 72 px`.
- Small popup buttons: `172 x 58 px`.
- Secondary scene buttons such as `BACK` and `DEPLOY`: `188 x 58 px`.
- Sidebar wave labels are not buttons; use ColorRect/panel styling there.

Button labels:
- Use the project pixel/TMP font.
- Menu label size: `16 px`.
- Popup label size: `14 px`.
- Color: `#E8EAF0`, except EXIT label uses `#FF6B35`.
- Align center-middle.
- No text shadows.
- No text outlines.

Button states:
- Normal: `Color(1, 1, 1, 1)`.
- Hover: `Color(0.85, 0.95, 1.0, 1)`.
- Pressed: `Color(0.6, 0.75, 0.9, 1)` plus a down offset of 2px.
- Disabled: `Color(0.4, 0.4, 0.4, 0.6)`.
- Hover/touch motion: button scale may rise subtly to about `1.04x`; pressed may compress to about `0.97x`.
- Mission map nodes may use stronger hover/selected pulse, but must stay readable and must not move layout.

## Scene: Main Menu

Layout:
- Left 60%: title, subtitle, and status text only.
- Right 40%: `COMMAND` panel with 4 vertical buttons.

COMMAND panel:
- Background: `#111827` panel or nine-patch panel.
- Border: `#1E90A8`, 1px.
- Padding: 24px on all sides.
- Button vertical gap: 12px.
- Header: `COMMAND`, pixel font, 14px, `#6B7280`, left aligned.

Button order:
- START GAME
- SETTING
- HOW TO PLAY
- EXIT

EXIT:
- Label color `#FF6B35`.
- Background modulate stays normal.

## Scene: Defeat / End Game Popup

Popup panel:
- Approximately `820 x 520 px`, centered.
- Background: `#0D1117`.
- Border: 2px solid `#1E90A8`.

Content order:
1. `DEFEAT` or `VICTORY`: pixel font 32px. Defeat uses `#FF6B35`.
2. Mission name.
3. Stats line: Cleared / Highest.
4. Empty spacing.
5. Three horizontal buttons: `RESTART | MISSION | MAIN MENU`.

## Scene Motion

- Non-gameplay frontend scenes should not be static. Use subtle dark pixel debris or diagonal bullet-rain motion behind the UI.
- Main menu title may flicker with dim attack-light modulation. Do not use text shadow, outline, or glow.
- Mission `DEPLOY` should play a short tactical transition before loading the gameplay scene.

## Do Not Do

- Do not draw rounded corners with generic flat engine style.
- Do not use the default engine theme for menu elements.
- Do not add drop shadow or glow to text.
- Do not scale `button_command.png` with linear filtering.
- Do not invent new UI colors outside the palette above.
