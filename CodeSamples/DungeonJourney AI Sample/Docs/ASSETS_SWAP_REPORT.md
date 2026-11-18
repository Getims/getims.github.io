# Замена ассетов на пак заказчика — отчёт (2026-10-23)

Задача: удалить старые спрайты (вырезки со скринов), поставить новые из `Reference/New/PhaserTextures/`, подключить звуки из `Reference/New/PhaserSounds/`.

**Итог:** все экраны (меню, карты, бой, победа, смерть, инвентарь, сундук, попап предмета, настройки) на новой графике, раскладка сверена со скринами оригинала. 14 звуков подключены через шину событий, громкости Music/SFX из настроек работают «вживую». `npm run build` проходит, вес 4.31 МБ (лимит 5 МБ). Автотест `stage3`: 35/35 (см. «Замечания» про один нестабильный тест).

## 1. Бэкап

`Reference/Backup/assets_old.zip` — весь старый `Assets/` кроме `fonts/` (283 файла, 30 МБ). Архив проверен (`testzip` без ошибок, 283 записи = 283 файла) **до** удаления. Удалено 154 файла (заменённые спрайты и их `_raw/`-копии).

## 2. Замены: старый ключ → новый файл

Импорт повторяемый: `python Tools/import_new_assets.py` (из папки проекта).

### Фоны, герой, эффекты

| Старый ключ (код) | Новый файл пака | Новый ключ / путь | Примечание |
|---|---|---|---|
| bg_battle | background.png | backgrounds/bg_battle | 1672×941 → JPEG 1920 |
| bg_menu | menu_bg.png | backgrounds/bg_menu | героя на фоне больше нет → отдельный `hero_menu` |
| bg_fail | died_bg.png | backgrounds/bg_fail | |
| hero_idle | player_idle.png (2 кадра) | hero/hero_idle_1, hero_idle_2 | idle теперь анимирован (смена кадров 0.45 с) |
| hero_attack_strike | player_attack.png (3 кадра) | hero/hero_attack_1..3 | замах / удар / возврат по фазе выпада |
| hero_victory | victory_pose.png | hero/hero_victory | |
| (герой на фоне меню) | menu_hero.png | hero/hero_menu | меню и инвентарь, позиция по скринам |
| fx_slash_arc | slash.png | fx/fx_slash | масштаб в коде 1.45 → 1.05 (тот же экранный размер) |

### Враги (по визуальному совпадению со старой вырезкой)

| id в enemies.json (имя в данных) | старый спрайт | новый спрайт |
|---|---|---|
| mushroom_small (Tiny Mushroom Creature) | enemy_mushroom_small | enemy_tiny_mushroom_creature |
| beetle_tiny (Beetle) | enemy_beetle_tiny | enemy_tiny_beetle |
| goblin_knife (Runt) | enemy_goblin_knife | enemy_grunt ⚠ |
| slime_green (Slime) | enemy_slime_green | enemy_slime |
| orc_red (Grunt) | enemy_orc_red | enemy_brawler ⚠ |
| skeleton | enemy_skeleton | enemy_skeleton |
| goblin_assassin (Skirmisher) | enemy_goblin_assassin | enemy_skirmisher |
| imp_orange (Fire Imp) | enemy_imp_orange | enemy_runt ⚠ |
| ghoul (Rotting Goblin) | enemy_ghoul | enemy_rotting_goblin |
| beetle_big (Frost Beetle) | enemy_beetle_big | enemy_fierce_beetle ⚠ |
| mushroom_big (Fierce Mushroom Creature) | enemy_mushroom_big | enemy_fierce_mushroom_creature |
| spider (Fierce Spider) | enemy_spider | enemy_venomous_spider ⚠ |
| imp_blue (Frost Imp) | enemy_imp_blue | enemy_venomous_imp ⚠ |
| golem_knight (Tank) | enemy_golem_knight | enemy_tank |
| beetle_dark (Dark Beetle) | enemy_beetle_dark | enemy_frost_beetle ⚠ |
| wraith | enemy_wraith | enemy_wraith |
| slime_knight (Armored Slime) | enemy_slime_knight | enemy_armored_slime |
| lizard_purple (Lizardman) | enemy_lizard_purple | enemy_enraged_lizardman |
| troll_ice (Ice Troll) | enemy_troll_ice | enemy_frost_ogre ⚠ |
| brute (Brute, босс) | enemy_demon_purple | enemy_brute |
| ice_king (Ice Troll King, босс) | enemy_troll_ice | enemy_frost_ogre (как и раньше — тот же спрайт, что у Ice Troll) |

⚠ — имя врага в наших данных не совпадает с именем файла в паке (данные клона назывались по догадке). Спрайт выбран тот же, что был раньше; имена в данных не менял — **вопрос заказчику**, см. §7.

### Предметы (иконки 160×160; раньше у 12 из 20 предметов иконки не было)

| item id | старая иконка | новая |
|---|---|---|
| leather_cap | item_cap_leather | head_leather_cap |
| horned_helm | item_helm_horned | head_horned_helm |
| leather_boots | item_boots_leather | legs_boots |
| rusty_blade | item_axe_steel (временно) | wpn_rusty_blade |
| steel_axe | item_axe_steel | wpn_steel_axe |
| steel_mace | item_mace_steel | wpn_steel_mace |
| iron_ring | item_ring_iron | acc_iron_ring |
| leech_charm | item_charm_leech | acc_leech_charm |
| padded_vest | — | body_leather_vest |
| chainmail | — | body_chainmail |
| knight_plate | — | body_plate |
| leather_gloves | — | arms_gloves |
| iron_bracers | — | arms_bracers |
| wooden_shield | — | sec_wood_shield |
| spiked_buckler | — | sec_jagged_kite_shield ⚠ (баклера в паке нет) |
| tower_shield | — | sec_tower_shield |
| iron_greaves | — | legs_greaves |
| dragon_blade | — | wpn_flame_blade ⚠ |
| crown_of_kings | — | head_crown |
| ring_of_wrath | — | acc_war_band ⚠ |

Ключи: `key_{copper,silver,gold}_big/_small` → `items/key_copper|silver|gold` (одна картинка на ключ; медный и серебряный больше не тонируются из золотого).

### UI

| Старый ключ | Новый файл пака | Новый ключ | Изменение в коде |
|---|---|---|---|
| logo_emblem_skull + logo_title | menu_title.png | ui/logo_title | эмблема входит в логотип, `logo_emblem_skull` больше не рисуется |
| btn_play_blank (+ наш текст) | btn_play.png | btn_play | текст запечён, свой текст убран |
| btn_play_blank (Inventory) | btn_inventory.png | btn_inventory | то же |
| btn_continue_blank | btn_continue.png | btn_continue | то же |
| btn_settings_blank | btn_settings.png | btn_settings | то же |
| btn_leaderboard_blank | btn_leaderboard.png | btn_leaderboard | то же |
| btn_back_blank | btn_back.png | btn_back | меню инвентаря и настройки |
| btn_quit_red_settings_blank | btn_quit.png | btn_quit | QUIT в настройках |
| btn_revive_text | died_btn_revive.png | btn_revive | |
| btn_menu_skull_text | died_btn_mainmenu.png | btn_mainmenu | |
| btn_playagain_skull_text | died_btn_playagain.png | btn_playagain | |
| text_you_died | died_title.png | text_you_died | |
| orn_fail_divider | died_ornamental.png | orn_fail_divider | |
| orn_victory_top / bottom | victory_top.png / victory_bot.png | orn_victory_top / bottom | |
| orn_victory_mid_left / right | victory_side.png | orn_victory_side | правый — зеркало через CSS |
| diamond_full / empty | foe_alive.png / foe_dead.png | diamond_full / empty | |
| hpbar_empty | game_bar_hp.png | hpbar_frame | рамка с прозрачными полями — CSS-смещение, заливка прежняя |
| card_frame_common/uncommon/rare/epic | pnl_common/uncommon/rare/epic.png | card_frame_* | |
| (legendary = common + CSS-фильтр) | pnl_legendary.png | card_frame_legendary | фильтр sepia убран |
| inventory/slot_* (плейсхолдеры) | empty_*.png | inventory/slot_* | пустой слот = рамка + силуэт |
| ui/slot_frame_empty | pnl_equipment_slot.png | inventory/slot_frame | рамка пустого слота экипировки |
| кнопки OPEN (CSS) | btn_open_copper/silver/gold/disabled.png | ui/btn_open_* | |
| кнопки 3→1 (CSS) | btn_3to1_silver/gold.png | ui/btn_3to1_* | медь→серебро = серебряная, серебро→золото = золотая |

## 3. Переименования

**GUID-файлы** — открыты и просмотрены, это запечённые надписи оригинала (меню / лидерборд). Положены в `Assets/ui/text_ref/`, **в билд не идут**: у нас эти тексты живые (данные, лидерборд, рекорд).

| GUID | Что на картинке | Имя |
|---|---|---|
| 1dcb0519-… | цифра «0», белая | text_digit0_white |
| a0e38720-… | цифра «0», золотая | text_digit0_gold |
| b43e379d-… | цифра «0», медная | text_digit0_copper |
| 21037e37-… | «Rank #6708 of 7941 — deeper than 16%» | lb_rank_line |
| 48738698-… | «Your best: Floor 7» | lb_your_best |
| 5c2a8224-… | «Season 1 is live» | lb_season_live |
| 9b7e7e9e-… | «Season 1» | lb_season |
| c1f95aab-… | «#1 FiveXeroSeven — 317» | lb_entry_1 |
| fb548a9b-… | «#2 IronGrandma.xZsq — 158» | lb_entry_2 |
| e3bdf25f-… | «#3 Yoyoman228 — 132» | lb_entry_3 |
| c4db4391-… | «Floor 6 — your loadout» | lb_loadout_title |
| d5a96385-… | «Your rank earns a Veteran's Reward» | lb_reward_note |
| dd216745-… | «One dungeon — everyone races the same floors» | lb_tagline |
| f65d00c3-… | «The Eternal Descent» | lb_title |

**Случайные суффиксы** пака (`_74y`, `_wf6`, `_rs2`…) отброшены: `enemy_armored_ogre_74y` → `enemy_armored_ogre`, `wpn_steel_mace_rs2` → `wpn_steel_mace`, `acce_*` → `acc_*`. Совпадения разведены по виду: `enemy_rotting_zombie_jof` → `enemy_rotting_zombie`, `_qux` (бледный) → `enemy_rotting_zombie_pale`; `head_storm_circlet_nb1` → `head_storm_circlet`, `_jjs` (с синим камнем) → `head_storm_circlet_blue`. Прочие файлы: `key.png` → `ui/icon_key`, `fight.png` → `ui/icon_fight`, `flame/glow/spark.png` → `fx/fx_*`, `pnl_inventory_slot` → `inventory/grid_cell`, `pnl_inventory_keyholder` → `inventory/panel_keys`, `btn_inventory_tab` → `ui/btn_tab`.

## 4. Звуки → события

WAV лежат в `Assets/Audio/`; сборка конвертирует в MP3 64k моно (`Config/build.config.mjs`), WAV в билд не попадает. Музыка и SFX подключены в `Game/sound.ts` через шину `Game/events.ts`. Громкость: SFX = слайдер SFX × микс звука; музыка = слайдер Music × 1.2 (было × 0.6, увеличено вдвое по просьбе заказчика) (меняется сразу при движении слайдера); `mute` из сейва глушит всё. В Fast Replay SFX тише вдвое.

| Звук | Событие | Где |
|---|---|---|
| click | нажатие любой кнопки на сцене, выбор карты | `pointerdown` по `button` / `.pcard` |
| slot_spin | начало прокрутки карт | новое событие `card_spin_start` |
| slot_stop | карта остановилась | новое событие `card_stop` (на каждую незалоченную карту) |
| hit | удар героя по врагу; thorns героя | `hit` (from=hero), `thorns` |
| hurt | удар по герою; thorns врага | `hit` (from=foe), `thorns` |
| crit | крит, execute | `crit`, `execute` |
| death | смерть врага (тише), босса, героя | `foe_death`, `boss_death`, `hero_death` |
| victory | этаж пройден | `floor_clear` |
| key | выпал ключ | `key_drop` |
| reward | сундук / мердж (не legendary), revive | `chest_open`, `merge`, `revive` |
| legendary | legendary из сундука / мерджа / выбрана legendary-карта | `chest_open`, `merge`, `card_pick` |
| equip | экипировка по кнопке Equip, апгрейд | новое событие `equip`, `upgrade` |
| menu (музыка, луп) | экраны menu, inventory, died | `screen` |
| combat (музыка, луп) | экраны path, choose, battle, descending | `screen`, переход с кроссфейдом 0.4 с |

Модуль звука: `../../Modules/audio.ts` не умеет переключать треки и менять громкость музыки на лету, поэтому сделана копия в проекте `Game/core/audio.ts` (по правилу «особая версия модуля — копия внутри проекта»): отдельный gain музыки, `playMusic(key)` с кроссфейдом, `setMusicVolume()`. Общий модуль не тронут — находка для «Находок по воркфлоу» (переключение музыки может пригодиться и другим).

**Проверено** в Chrome (Playwright): все 14 файлов декодируются, проигрываются реальные буферы (музыка 45 с в лупе, SFX — с их длиной), на проходе меню → карты → бой → смерть → инвентарь сработали: click, slot_spin, slot_stop, hit, hurt, crit, death, victory, key, reward, equip, музыка menu/combat. Лог — `BuildReport/Screenshots/assets_swap/sounds.json`; для тестов добавлено `window.__game.sounds` (последние 200 ключей).

## 5. Новые ассеты без пары в коде (лежат в `Assets/`, в билд не идут)

- **Враги (14):** armored_ogre, armored_zombie, bat, enraged_demon, enraged_gargoyle, fierce_spider, fire_slime, rotting_cultist, rotting_zombie, rotting_zombie_pale, spectral_mushroom_creature, spectral_ogre, spectral_skeleton, venomous_slime; `enemy_fallback` (красный квадрат 40×40) — в билде, кодом не используется.
- **Предметы (35):** acc_crit_eye, acc_flaming_amulet, acc_jagged_charm, acc_obsidian_charm, arms_gauntlets, arms_golden_vambraces, arms_ornate_gloves, body_dragon_mail, body_robe, body_steel_breastplate, head_crystal_circlet, head_flaming_crown, head_iron_helm, head_mage_hood, head_storm_circlet, head_storm_circlet_blue, legs_bloodforged_leg_plates, legs_crystal_boots, legs_obsidian_greaves, legs_plate_legs, sec_crystal_spell_tome, sec_frost_parrying_dagger, sec_iron_parrying_dagger, sec_kite_shield, sec_parry_dagger, sec_storm_kite_shield, sec_tome, wpn_glaive, wpn_iron_sword, wpn_void_scythe, wpn_warhammer (+ уже занятые выше). В `items.json` всего 20 предметов — остальные иконки ждут этапа контента.
- **UI / FX:** `inventory/grid_cell` (ячейка склада — склад пока на CSS с цветной рамкой редкости), `inventory/panel_keys` (полоса ключей 2508×423 — по пропорциям не ложится в нашу нижнюю строку высотой ~70 px), `ui/btn_tab` (одна вкладка без активного состояния), `ui/icon_key`, `ui/icon_fight`, `fx/fx_flame`, `fx/fx_glow`, `fx/fx_spark` (частиц пока нет — этап 8), `ui/text_ref/*` (14).

## 6. Старые ассеты без замены в паке (не удалены)

Используются в игре (это та же графика оригинала, по стилю совпадает): `btn_gear`, `btn_stats`, `icon_checkmark`, `orn_card_divider`, `orn_header_card`, `orn_small_divider`, `orn_settings_divider`, `orn_status_swords_left/right`, `slider_thumb`, `text_victory`, `text_strike_first`, `text_ambush_icon`.

Лежат, не используются: `hero_stance_1..3`, `hero_attack_windup`, `hero_recover`, `hero_hurt` (+ их `_raw/`), `bg_cards`, `bg_inventory`, ui: `btn_cancel_violet`, `btn_equip_green`, `btn_equipped_green`, `btn_keep_gold`, `btn_merge_blue`, `btn_salvage_red`, `btn_shards_yellow`, `btn_tab_active/inactive`, `btn_toggle_on/off`, `hpbar_foe_blue`, `hpbar_player_red`, `icon_shard`, `icon_skull_boss`, `icon_video_ad`, `icon_warning`, `logo_emblem_skull`, `panel_inventory`, `panel_item_modal`, `panel_settings`, `panel_texture_strip`, `slider_track_empty/fill`, `text_boss_big`, `text_boss_defeated`. Шрифты `fonts/` — без изменений. `Audio/_clone/` (37 mp3 старого клона) — не используются, оставлены.

## 7. Чего не хватает / вопросы заказчику

1. **Имена врагов.** В паке имена другие, чем в наших данных (⚠ в §2): например, гоблин с ножом в паке `grunt`, а у нас «Runt»; красный орк — `brawler`, у нас «Grunt»; оранжевый бес — `runt`, у нас «Fire Imp». Переименовать врагов в `enemies.json` по паку (Runt / Grunt / Brawler / Venomous Imp / Fierce Beetle / Frost Beetle / Venomous Spider / Frost Ogre)?
2. **Ice Troll King** (босс) использует тот же спрайт, что Ice Troll. В паке есть свободные `enemy_spectral_ogre`, `enemy_armored_ogre`, `enemy_enraged_demon` — дать боссу один из них?
3. **Нет кадров героя:** урон и смерть. Сейчас: урон — красная вспышка поверх idle, смерть — idle с красной заливкой (раньше был кадр `hero_hurt` из старой нарезки, по стилю не совпадает с новыми кадрами). Нужны `player_hurt` / `player_death`?
4. **Предметы без точной пары:** Spiked Buckler → `sec_jagged_kite_shield`, Dragon Blade → `wpn_flame_blade`, Ring of Wrath → `acc_war_band`. Ок?
5. **Нет в паке:** шестерёнка и кнопка статов, галочка «надето», бегунок слайдера, тексты VICTORY / STRIKE FIRST / AMBUSH, декоративные разделители карт и настроек, иконки статов — используются старые вырезки (см. §6). Рамка ячейки склада и полоса ключей в паке есть, но не подошли (см. §5).

## 8. Замечания

- **Автотест:** `npm test -- stage3` первый прогон 34/35 — упал «equipped item changes hero stats» (124 → 124). Причина не в ассетах: тест суммирует atk+maxHp+def, а медный сундук может дать предмет только с critом и т. п. Повторный прогон — 35/35. Тест нестабилен, стоит поправить на этапе тестов (не трогал: тесты без просьбы не меняю).
- Пустые папки `Assets/_raw/enemies`, `_raw/fx`, `_raw/items` удалить не удалось (защита от удаления по маске) — можно удалить вручную, они пустые.
- legendary-звук в проходе не выпал естественно (нужен legendary-дроп), проверен по коду: та же ветка, что у reward.
- Добавлен `.claude/launch.json` (dev-сервер для превью).

## 9. Вес билда

| | Байт | Примечание |
|---|---|---|
| До | 3 172 367 (3.10 МБ) | старые вырезки, без звука |
| После | 4 516 586 (4.31 МБ) | лимит 5 000 000 |

Прирост +1.34 МБ: музыка 2 × 470 КБ (base64; MP3 64k моно, 45 с), SFX 12 шт. ≈ 150 КБ, рамки карт 5 × ~135 КБ (раньше 4 маленькие вырезки), иконки 20 предметов вместо 8. Если нужен запас — первыми кандидатами будут музыка (48k или укоротить луп) и рамки карт (JPEG невозможен из-за прозрачности; можно уменьшить до 360 px).

## 10. Скрины

`BuildReport/Screenshots/assets_swap/`: `01b_menu_best`, `03_cards`, `04_battle`, `06_victory`, `07_died`, `09_inventory`, `10_chest_reward`, `10b_item_popup`, `12_settings` (+ `sounds.json`). Проблем раскладки не найдено; места, где на скринах видна старая графика, перечислены в §6.

## 11. Изменённые файлы

- `Tools/import_new_assets.py` — новый (импорт пака).
- `Config/build.config.mjs` — явный список ассетов, враги/иконки по данным, звуки.
- `Game/data/enemies.json`, `Game/data/items.json` — спрайты и иконки.
- `Game/battle/presenter.ts` — кадры героя, слэш.
- `Game/screens/menus.ts`, `flow.ts`, `battle.ts`, `cards.ts`, `Game/overlays.ts`, `Game/inventory.ts` — новые ключи, размеры, кнопки с запечённым текстом.
- `Game/events.ts` — события `card_spin_start`, `card_stop`, `equip`.
- `Game/core/audio.ts` (копия модуля с доработками), `Game/sound.ts` — новые; `Game/main.ts` — подключение звука, `__game.sounds`.
- `Assets/ASSETS.md` — переписан под новый пак.

## 12. Решения заказчика (2026-10-23)

Ответы на §7: «1 переименуй по паку, 2 оставь, 3 кадров смерти вроде нет и в оригинале, там просто персонаж заливается красным светом и вспышка урона происходит, 4 оставь 5, да, пока старые используем. может позже принесу ещё спрайты».

1. **Враги переименованы по паку** (`Game/data/enemies.json`, поле `name`):

   | id | было | стало |
   |---|---|---|
   | beetle_tiny | Beetle | Tiny Beetle |
   | goblin_knife | Runt | Grunt |
   | orc_red | Grunt | Brawler |
   | imp_orange | Fire Imp | Runt |
   | beetle_big | Frost Beetle | Fierce Beetle |
   | spider | Fierce Spider | Venomous Spider |
   | imp_blue | Frost Imp | Venomous Imp |
   | beetle_dark | Dark Beetle | Frost Beetle |
   | troll_ice | Ice Troll | Frost Ogre |

   **Id не менял.** Сейвы id врагов не хранят (забег = этаж, перки, HP, сид), но на id ссылаются `floors.json` (боссы) и генератор этажей; смена id — лишняя правка без пользы для игрока. Id остаются внутренними ключами (часть теперь не совпадает с именем, например `imp_orange` = Runt). Остальные имена уже совпадали с паком. Lizardman оставлен без префикса Enraged (в паке это модификатор, как Fierce/Venomous у других). Другие места с именами врагов: в коде и тестах нет; в ГДД §3 типы перечислены общими словами — добавлено уточнение про Frost Ogre / Brute и ссылка на `enemies.json`.
2. **Ice Troll King** — без изменений (спрайт `enemy_frost_ogre`).
3. **Кадры урона/смерти не нужны** — текущее поведение (красная заливка героя + вспышка) соответствует оригиналу, оставлено.
4. **Пары предметов** (Spiked Buckler, Dragon Blade, Ring of Wrath) — оставлены.
5. **Недостающие элементы** — пока старые вырезки (§6); заказчик, возможно, принесёт ещё спрайты.

Проверка после правки: `npm run build` — 4 516 598 байт; `npm test -- stage3` — 34/35, затем 35/35 (тот же нестабильный тест про статы предмета из сундука, §8).

## 13. Музыка меню с первого клика (2026-10-23)

Браузеры не дают играть звук до первого клика, а первым кликом обычно был PLAY — музыка сразу переключалась на боевую, и музыку меню не было слышно. Теперь при первом входе в меню поверх него затемнение с надписью «CLICK TO START» (`menu.tapToStart`, `Game/screens/menus.ts`): клик по нему разблокирует звук, и сразу играет музыка меню; кнопки под ним этим кликом не нажимаются. После разблокировки звука экран больше не показывается. Под автотестом (`navigator.webdriver`) экран не показывается — тесты кликают кнопки меню напрямую. Проверено: экран → клик → остаёмся в меню, запущен луп `menu` (45 с); `npm test -- stage3` 35/35; билд 4 517 168 байт.

## 14. Правки звука и новые файлы пака (2026-10-23, второй заход)

- **Экран CLICK TO START убран** (вместе со строкой `menu.tapToStart`).
- **Музыка меню (`menu.wav`):** экраны карт (Choose Your Path / Choose One) больше не переключают трек — после PLAY музыка меню играет до начала боя, боевая включается на экране боя. Поэтому даже если первый клик — сразу PLAY (браузер не даёт звук до первого клика), `menu` запускается и слышна. Проверено в Chrome: первый клик по PLAY → стартует луп `menu` (45 с), экран карт — музыка та же.
- **Громкость в настройках работает вживую.** Причина бага: окно настроек ставит игровой цикл на паузу, а громкость музыки применялась только в цикле — ползунок двигался, звук не менялся до закрытия окна. Теперь ползунки сразу применяют громкость (`syncSound()` из `Game/overlays.ts`); при отпускании ползунка SFX звучит click с новой громкостью. Проверено: при открытых настройках Music 0% → gain 0, 50% → 0.3; SFX 0% — звуки не играют.
- **Новые файлы в паке:** 60 новых PNG, все с GUID-именами — это запечённые надписи оригинала (названия и описания перков: Ironhide, Vampirism, Juggernaut…; статы Your Build; «Choose One», «Reroll · Watch Ad», «Close», редкости COMMON/UNCOMMON). У нас эти тексты живые (данные + шрифт), поэтому в игру не ставились. Нужных элементов из §7.5 (шестерёнка, кнопка статов, галочка, бегунок, VICTORY / STRIKE FIRST / AMBUSH, разделители) среди новых нет. Звуки — те же 14.
- Проверка: `npm run build` — 4 516 623 байт; `npm test -- stage3` — 35/35.

## 15. Громкость музыки ×2, тихий клик (2026-10-23)

- Музыка: множитель к ползунку Music 0.6 → 1.2 (`Game/sound.ts`).
- Клик не был слышен: в паке `click.wav` записан почти беззвучно (пик ~1% шкалы, −39 дБ; у остальных звуков 80–100%). Также тихие `reward.wav` (14%) и `equip.wav` (27%). Эти три нормализованы до пика 0.9 при импорте (`NORMALIZE` в `Tools/import_new_assets.py`); оригиналы в `Reference/New/` не тронуты. В билде (MP3) пик теперь 0 / 0 / −0.9 дБ. Заказчику: если громкость у исходников так и задумана — лучше перевыгрузить эти три файла нормальной громкости.
