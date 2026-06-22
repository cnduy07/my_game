# OVERNIGHT SPRINT — Coreline Defense Tactical Vertical Slice

Date: 2026-06-23  
Owner: Codex  
Mode: autonomous overnight implementation

## Objective

Upgrade Coreline Defense from a functional lane-defense prototype into a tactical campaign vertical slice without adding throwaway systems or unapproved asset production.

## Non-Negotiables

- Runtime UI/copy remains English-first.
- Do not add packages or new frameworks.
- Do not generate mass production assets before art direction is approved.
- Prefer shared data/analysis helpers over hardcoded UI-only logic.
- Keep commits scoped and documented.
- Run static checks and Unity batchmode checks when possible.

## Primary Work Blocks

1. Build shared `CampaignIntel` for enemy composition, dominant threats, recommended tools, pressure scoring, and mission node type.
2. Replace mission list with a tactical campaign map: route, nodes, states, selected mission detail, deploy action.
3. Add wave intel to gameplay HUD using the same `CampaignIntel` wave summary.
4. Improve seed/OC/placement clarity without increasing gameplay complexity.
5. Expand AI QA and balance report to campaign-wide checks.
6. Clean and update project docs so context stays accurate after the sprint.

## Desired Morning State

- `MISSIONS` opens a campaign map, not a scrolling list.
- Level 1-10 are presented as campaign nodes with route and tactical details.
- Gameplay HUD previews incoming wave composition.
- QA verifies campaign data, recommended tools, SnowGun effect, Fast/Shield identity, and release-readiness warnings.
- Docs describe the current architecture accurately.

## Progress Log

- Done: added `CampaignIntel` shared helper.
- Done: replaced mission list implementation with campaign map nodes, route lines, mission detail panel, and explicit deploy action.
- Done: added gameplay wave intel HUD fed by `EnemySpawner.WaveIntelText`.
- Done: added gameplay command feedback strip for placement and Overcharge.
- Done: added OC lane pressure indicators from live enemy rows.
- Done: expanded balance report with campaign-wide pressure/mix/tool summaries.
- Done: expanded AI QA with campaign map/catalog checks and campaign intel validations.
- Done: updated `PROJECT_PROGRESS.md`, `TASKS.md`, `TECH_PLAN.md`, `PROJECT_CONTEXT.md`, and `CONTENT_PLAN.md`.
- Done: static checks passed (`git diff --check`, obsolete/symbol cleanup search).
- Done: Unity batchmode AI QA passed with `0` fail, `1` expected release warning for `unlockAllLevelsForTesting`.
- Pending: commit checkpoint.
