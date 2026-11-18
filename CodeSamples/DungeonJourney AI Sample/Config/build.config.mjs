// Config for Tools/build/build.mjs (project copy of Modules/build/build.mjs) (run from the project folder: npm run build / npm run dev).
// Keys of `assets` become window.__A[key] (data URIs); fonts become @font-face.
import fs from 'node:fs';

const png = (dir, prefix, width, skip = /^$/) => Object.fromEntries(
  fs.readdirSync(`Assets/${dir}`).filter((f) => f.endsWith('.png') && f.startsWith(prefix) && !skip.test(f))
    .map((f) => [f.replace('.png', ''), { kind: 'image', file: `${dir}/${f}`, width }]),
);
const img = (file, width) => ({ kind: 'image', file, width });
const json = (f) => JSON.parse(fs.readFileSync(f, 'utf8'));

// only what the data uses goes into the build (the pack has more enemies / items than the clone's data)
const enemies = Object.fromEntries([...new Set(json('Game/data/enemies.json').map((e) => e.sprite)), 'enemy_fallback']
  .map((k) => [k, img(`enemies/${k}.png`)]));
const items = Object.fromEntries(json('Game/data/items.json').filter((i) => i.icon).map((i) => [i.icon, img(`items/${i.icon}.png`, 112)]));
// customer's sounds: WAV in Assets/Audio, MP3 (mono) in the build
const sfx = ['click', 'crit', 'death', 'equip', 'hit', 'hurt', 'key', 'legendary', 'reward', 'slot_spin', 'slot_stop', 'victory'];
const audio = {
  ...Object.fromEntries(sfx.map((k) => [k, { kind: 'audio', file: `Audio/${k}.wav`, bitrate: '64k' }])),
  menu: { kind: 'audio', file: 'Audio/menu.wav', bitrate: '64k', trim: false },
  combat: { kind: 'audio', file: 'Audio/combat.wav', bitrate: '64k', trim: false },
};

const GLYPHS = Array.from({ length: 95 }, (_, i) => String.fromCharCode(32 + i)).join('') + '→×◆▲▼·…—–⚠▶';

export default {
  src: 'Assets',
  entry: 'Game/main.ts',
  title: 'Dungeon Journey',
  assets: {
    bg_battle: { kind: 'image', file: 'backgrounds/bg_battle.png', jpeg: 72, width: 1920 },
    bg_menu: { kind: 'image', file: 'backgrounds/bg_menu.png', jpeg: 72, width: 1920 },
    bg_fail: { kind: 'image', file: 'backgrounds/bg_fail.png', jpeg: 72, width: 1920 },
    ...enemies,
    ...png('hero', 'hero_attack_', undefined, /windup/), // hero_attack_windup: old cut, not in the new pack
    ...png('hero', 'hero_idle_'),
    hero_victory: img('hero/hero_victory.png'),
    hero_menu: img('hero/hero_menu.png', 680),
    fx_slash: img('fx/fx_slash.png'),
    ...items,
    key_copper: img('items/key_copper.png', 80),
    key_silver: img('items/key_silver.png', 80),
    key_gold: img('items/key_gold.png', 80),
    ...png('inventory', 'slot_', 144),
    logo_title: img('ui/logo_title.png', 1021),
    btn_play: img('ui/btn_play.png', 500),
    btn_inventory: img('ui/btn_inventory.png', 500),
    btn_continue: img('ui/btn_continue.png', 480),
    btn_settings: img('ui/btn_settings.png', 405),
    btn_leaderboard: img('ui/btn_leaderboard.png', 305),
    btn_back: img('ui/btn_back.png', 405),
    btn_quit: img('ui/btn_quit.png', 405),
    btn_revive: img('ui/btn_revive.png', 592),
    btn_mainmenu: img('ui/btn_mainmenu.png', 436),
    btn_playagain: img('ui/btn_playagain.png', 436),
    btn_open_copper: img('ui/btn_open_copper.png', 120),
    btn_open_silver: img('ui/btn_open_silver.png', 120),
    btn_open_gold: img('ui/btn_open_gold.png', 120),
    btn_open_disabled: img('ui/btn_open_disabled.png', 120),
    btn_3to1_gold: img('ui/btn_3to1_gold.png', 72),
    btn_3to1_silver: img('ui/btn_3to1_silver.png', 72),
    text_you_died: img('ui/text_you_died.png', 566),
    orn_fail_divider: img('ui/orn_fail_divider.png', 690),
    orn_victory_top: img('ui/orn_victory_top.png', 340),
    orn_victory_bottom: img('ui/orn_victory_bottom.png', 284),
    orn_victory_side: img('ui/orn_victory_side.png', 152),
    diamond_full: img('ui/diamond_full.png', 54),
    diamond_empty: img('ui/diamond_empty.png', 54),
    hpbar_frame: img('ui/hpbar_frame.png', 524),
    ...png('ui', 'card_frame_', 400),
    // old cuts without a replacement in the new pack (see Docs/ASSETS_SWAP_REPORT.md)
    ...Object.fromEntries(['btn_gear', 'btn_stats', 'icon_checkmark', 'orn_card_divider', 'orn_header_card', 'orn_small_divider', 'orn_settings_divider',
      'orn_status_swords_left', 'orn_status_swords_right', 'slider_thumb', 'text_victory', 'text_strike_first', 'text_ambush_icon',
      'text_boss_big', 'text_boss_defeated']
      .map((k) => [k, img(`ui/${k}.png`)])),
    ...audio,
    font_title: { kind: 'font', file: 'fonts/Cinzel.ttf', glyphs: GLYPHS, family: 'Cinzel' },
    font_mono: { kind: 'font', file: 'fonts/CourierPrime-Regular.ttf', glyphs: GLYPHS, family: 'Mono' },
    font_mono_b: { kind: 'font', file: 'fonts/CourierPrime-Bold.ttf', glyphs: GLYPHS, family: 'MonoBold' },
    font_goth: { kind: 'font', file: 'fonts/UnifrakturMaguntia.ttf', glyphs: GLYPHS, family: 'Gothic' },
  },
  body: '<div id="stage"><canvas id="game"></canvas><div id="ui"></div></div><div id="rotate">Rotate your device</div>',
  css: `
  #stage{position:absolute;left:0;top:0;width:1920px;height:1080px;transform-origin:0 0;overflow:hidden;background:#000}
  #stage canvas{position:absolute;left:0;top:0;width:1920px;height:1080px}
  #ui{position:absolute;inset:0;font-family:Mono,'Courier New',monospace;color:#e8dcc0}
  #rotate{display:none;position:fixed;inset:0;z-index:100;background:#000;color:#e8dcc0;font:28px Cinzel,serif;align-items:center;justify-content:center;text-align:center}
  `,
};
