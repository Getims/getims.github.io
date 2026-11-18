# Dungeon Descent — статы врагов по кадрам видео

Источник: кадры `scan/tNNNN.jpg` (тройки секунд каждые 15 с). В видео 2+ забега: смерть на F8 (~t155), затем Continue с F6, далее до F17+; в конце «Best: Floor 18». Часть видео — 3x FAST replay. t425–t530 — статичный экран выбора карты (пауза), t290–292 и t610+ — пустые кадры.

## 1. Таблица боевых кадров

| t, с | Этаж | Foe | Враг | Max HP (текущее) | Метка способности | Урон врага по герою | Урон героя | HP героя |
|---|---|---|---|---|---|---|---|---|
| 5 | 1 | 1/3 | Tiny Mushroom Creature | 16 (0) | — (плашка «AMBUSH!») | 100→95 (≈5) | 18 | 95/100 |
| 6 | 1 | 2/3 | Tiny Mushroom Creature | 9 (9) | — | — | — | 100/100 |
| 7 | 1 | 2/3 | Tiny Mushroom Creature | 9 (0) | — | 100→96 (≈4) | 18 | 96/100 |
| 50 | 2 | 3/3 | Tiny Mushroom Creature | 11 (0) | TOUGH · +60% HP | — | — (+1 Copper Key) | 83/88 |
| 65 | 3 | 3/3 | (не читается) | 21 (0) | — | — | — (Victory, Floor 3 cleared) | 75/88 |
| 125 | 6 | 2/3 | Runt | 23 (23) | — | — | — | 62/88 |
| 126 | 6 | 2/3 | Runt | 23 (0) | — | 62→54 (8) | 26 | 54/88 |
| 127 | 6 | 3/3 | Slime | 33 (33) | — | — | — | 58/88 |
| 140 | 7 | 1/3 | Skeleton | 104 (78) | TOUGH · +60% HP | (Ambush) | 26 | 73/95 |
| 141 | 7 | 1/3 | Skeleton | 104 (78) | TOUGH · +60% HP | 73→60 (13) | — | 60/95 |
| 142 | 7 | 1/3 | Skeleton | 104 (52) | TOUGH · +60% HP | 60→49 (11; на герое «13») | 26 | 49/95 |
| 200 | 6 | 1/3 | Grunt | 56 (14) | — | — | 42 | 88/88 (3x, Strike first) |
| 215 | 7 | 1/3 | Skeleton | 104 (14) | TOUGH · +60% HP | — | 45 | 80/88 (3x) |
| 216 | 7 | 2/3 | Skeleton | 39 (39) | TOUGH · +60% HP | «11» на герое | — | 66/88 (3x) |
| 217 | 7 | 3/3 | Skirmisher | 39 (0) | TOUGH · +60% HP | — | 45 | 67/88 |
| 230 | 9 | 1/3 | Tiny Mushroom Creature | 85 (15) | TOUGH · +60% HP | — | — | 74/88 |
| 231 | 9 | 2/3 | Fire Slime | 53 (53) | TOUGH · +60% HP | — | — | 80/88 |
| 232 | 9 | 2/3 | Fire Slime | 53 (53) | TOUGH · +60% HP | 80→63 (17; «17» на герое) | — | 63/88 |
| 276 | 11 | 1/3 | Rotting Goblin | 150 (150) | — | — | — | 92/97 |
| 277 | 11 | 1/3 | Rotting Goblin | 150 (10) | — | — | 140! (крит) | 97/97 |
| 320 | 11 | 1/3 | Rotting Goblin | 150 (68) | — | (красный маркер на герое) | 82 | 104/125 (3x) |
| 335 | 12 | 3/3 | Frost Beetle | 70 (0) | — | — | 160! (крит) | 125/125 |
| 365 | 13 | 2/3 | Fierce Mushroom Creature | 98 (14) | VAMPIRIC · HEALS FROM ITS HITS | — | — | 122/125 |
| 366 | 13 | 2/3 | Fierce Mushroom Creature | 98 (0) | VAMPIRIC · HEALS FROM ITS HITS | 122→99 (23 за ~1 с) | 999! (execute) | 99/125 |
| 367 | 13 | 3/3 | Fierce Spider | 98 (98) | VAMPIRIC · HEALS FROM ITS HITS | — | — | 106/125 |
| 395 | 14 | 2/3 | Tank | 170 (86) | — | — | 84 | 93/135 |
| 396 | 14 | 2/3 | Tank | 170 (72) | — | 93→67 (26) | — | 67/135 |
| 397 | 14 | 3/3 | Frost Beetle | 95 (95) | — | — | — | 98/135 |
| 410 | 15 | 3/3 | Wraith | 132 (35) | — | «30» на герое; оранж. «13» у врага (шипы) | — | 8/149 |
| 411 | 15 | 3/3 | Wraith | 132 (0) | — | — | 84 | 37/149 (lifesteal) |
| 425 | 16 | 3/3 | Armored Slime | 216 (216) | — | «35» на герое | — | 106/149 |
| 426 | 16 | 3/3 | Armored Slime | 216 (139) | — | — | (≈77, бледно) | 132/149 |
| 427 | 16 | 3/3 | Armored Slime | 216 (0) | — | — | 154! | 149/149 |
| 605 | 16 | 3/3 | Armored Slime | 216 (216) | — | «35» на герое | — | 101/167 |
| 606 | 16 | 3/3 | Armored Slime | 216 (46) | — | «35» на герое; оранж. «22» (шипы) | — | 132/167 |
| 607 | 16 | 3/3 | Armored Slime | 216 (0) | — | — | 999! (execute) | 152/167 |

Примечания: HP героя растёт между ударами за счёт Lifesteal (8% → 34%), поэтому разница HP занижает урон врага. Красные числа на герое — показанный урон после блока DEF.

## 2. По типам врагов

| Враг | Этаж: Max HP | Урон по герою |
|---|---|---|
| Tiny Mushroom Creature | F1: 9 и 16; F2: 11 (Tough); F9: 85 (Tough) | F1 ≈4–5 |
| Fierce Mushroom Creature | F13: 98 (Vampiric) | F13 ≈23 за ~1 с |
| Runt | F6: 23 | F6: 8 |
| Slime | F6: 33 | — |
| Fire Slime | F9: 53 (Tough) | F9: 17 |
| Armored Slime | F16: 216 (оба забега) | F16: 35 (стабильно) |
| Grunt | F6: 56 | — |
| Skeleton | F7: 104 (Tough, дважды); F7: 39 (Tough, FOE 2/3) | F7: 11–13 |
| Skirmisher | F7: 39 (Tough) | — |
| Rotting Goblin | F11: 150 (оба забега) | — |
| Frost Beetle | F12: 70; F14: 95 | — |
| Fierce Spider | F13: 98 (Vampiric) | — |
| Tank | F14: 170 | F14: 26 |
| Wraith | F15: 132 | F15: 30 |

Детерминизм: одинаковые этаж+враг дают одинаковый Max HP в разных забегах (Skeleton F7 = 104, Rotting Goblin F11 = 150, Armored Slime F16 = 216) — этажи фиксированы (в меню: «one dungeon, everyone races the same floors»).

### Подгонка HP(floor)
- Tiny Mushroom (база без Tough = HP/1.6): F1 ≈9, F2 ≈7, F9 ≈53 → экспонента k ≈ (53/9)^(1/8) ≈ **1.25/этаж**. Линейная (≈+5.5/этаж) хуже объясняет F1–F2. Точек 3 — качество низкое.
- Frost Beetle: F12 70 → F14 95 → k ≈ 1.165/этаж (2 точки).
- Сводно (Tough приведён к базе): F1 ≈9, F6 23–56, F7 24–65, F9 33–53, F11 150, F12 70, F13 98, F14 95–170, F15 132, F16 216. Грубая модель **HP ≈ 9·1.22^(F−1) × множитель типа** (Runt ≈0.8, Slime/Beetle/Mushroom ≈1, Grunt/Goblin/Tank ≈1.7–2.3). Разброс типов на одном этаже до ×2.5 — без множителя типа подгонка плохая.
- Tough = ×1.6 HP (подтверждено текстом метки).
- Странность: Skeleton F7 FOE 2/3 = 39 при FOE 1/3 = 104 на том же этаже — возможно, позиция в тройке влияет на HP, либо другой вариант врага. Данных мало.

### Подгонка DMG(floor) (показанный урон)
F1 ≈4–5, F6 8, F7 11–13, F9 17, F13 ~23, F14 26, F15 30, F16 35.
- Линейно: DMG ≈ 2.1·F + 1 — хорошо на F9–F16, завышает F6–F7.
- Экспонента: 4.5·1.15^(F−1) → F7 10.4, F9 13.8, F16 36.6 — приемлемо (±20%).
- Показанный урон уже уменьшен блоком DEF героя (F1: 31%, F9: 38%, F14: 45%, F17: 51%). Восстановленный «сырой» урон: F1 ≈6–7, F7 ≈17, F9 ≈27, F14 ≈47, F16 ≈60–70 → ≈ 6.5·1.16^(F−1).

## 3. Метки способностей (точный текст)
- `TOUGH · +60% HP` (золотой)
- `VAMPIRIC · HEALS FROM ITS HITS` (фиолетовый)

У большинства врагов метки нет.

## 4. Прочее новое
- HUD: три ромба под «FLOOR N» — прогресс FOE; справа «Best: Floor N», «Keys c/s/g».
- Плашки начала боя: «⚠ AMBUSH!» (красная, враг бьёт первым) и «YOU STRIKE FIRST!» (зелёная).
- В реплее под заголовком этажа значок «▶▶ 3x FAST»; в настройках тумблер «FAST REPLAY (3×)», а также «SCREEN SHAKE», громкости Music/SFX.
- Числа урона: белые — обычные; золотые с «!» — крит (140!, 154!, 160!); красные «999!» — execute; красные на герое — входящий урон; оранжевые с иконкой у врага — урон шипов (Thorns): 13, 22.
- При получении удара спрайт героя краснеет; белый силуэт — вспышка удара врага (Wraith/Slime).
- Карты «Choose One»: редкости Common/Uncommon/Rare/Epic, превью «стат A→B · (STACKS)», «REROLL – WATCH AD». Виденные: Executioner, Gambeson, Warlord, Bastion, Padding, Snack, Whetstone, Immortal King, Splinters, Deadly, Honing Oil, Spark, Serrated, Brutal, Duelist, Shield Bash, Reinforce, Bloodbound, Momentum, Cull, Retribution (Epic), Assassinate, Bloodlust, Surge, Reckless, Catch Breath, Barbs, Leech, Stoneblood.
- Окно «Your Build»: Max HP, Attack (+эффективная от DEF), Defense («blocks X% of every hit»), Crit (шанс, x2.0), Lifesteal, Thorns, Execute («instantly kill below 26% HP (cards replace, never stack)»), «Def → Atk: 30% of defense adds attack»; список апгрейдов с тултипами, дубликаты как «Barbs ×2».
- Смерть: «You Died / Reached Floor 8 / Keys — 1 copper · 1 silver · 1 gold / Your rank #6287 of 7757», Revive (watch ad), Main Menu, Play Again. Меню: Continue (Floor 6 · your loadout), Play, Inventory, лидерборд Season 1.
- Инвентарь: слоты Head/Body/Arms/Legs/Weapon/Secondary/Accessory; ключи открывают сундуки; предметы Leather Cap (+11 MaxHP), Steel Mace (+12 ATK +7 DEF), Rusty Blade (+7 ATK), Leech Charm (Rare, +16 Lifesteal), Horned Helm (Rare), Boots; Salvage → осколки (10 → copper key), Merge 1/3 копий.
