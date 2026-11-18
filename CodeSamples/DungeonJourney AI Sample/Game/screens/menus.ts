// Main menu and Inventory; registers all screens. Layout measured on the original's screenshots.
import { images } from '../core/assets';
import type { Screen } from '../core/screens';
import { H, W } from '../core/stage';
import { t } from '../core/strings';
import { KEY_TYPES } from '../data/types';
import { game } from '../game';
import { InventoryView } from '../inventory';
import { openSettings } from '../overlays';
import { continueRun } from '../run';
import { save } from '../save';
import { battleScreen } from './battle';
import { chooseScreen, pathScreen } from './cards';
import { descendingScreen, diedScreen, startNewRun } from './flow';
import { css, drawCover, el, img, imgButton, line, place } from '../ui';

css(`
.season{position:absolute;left:1372px;top:428px;width:496px;height:350px;box-sizing:border-box;border:1px solid #6a5e48;background:#0006}
.season div{position:absolute;left:0;right:0;text-align:center;font-weight:700}
`);

const bg = (key: string): Pick<Screen, 'render'> => ({ render: (ctx) => drawCover(ctx, images[key], W, H) });
const KEY_COLOR = { copper: '#d9824a', silver: '#c8d0d8', gold: '#f0c040' };

/** Fake season board (the original's leaderboard; ours is not live). */
function season(root: HTMLElement) {
  const b = el('div', 'season', '', root);
  const row = (html: string, y: number, size: number, color: string) => {
    const d = el('div', '', html, b);
    Object.assign(d.style, { top: `${y - size * 0.62}px`, fontSize: `${size}px`, color });
  };
  row(t('menu.season'), 72, 28, '#e8b84a');
  row(t('menu.lb1'), 123, 26, '#ece4d4');
  row(t('menu.lb2'), 167, 26, '#ece4d4');
  row(t('menu.lb3'), 211, 26, '#ece4d4');
  row(t('menu.rank', { r: 6287, n: 7757, p: 19 }), 294, 24, '#7fd48a');
  line(root, t('menu.live'), 812, 28, '#8ec8ff').style.cssText += 'left:1372px;right:52px;font-weight:700';
  line(root, t('menu.reward'), 848, 22, '#9aa0b0').style.cssText += 'left:1372px;right:52px';
}

const menu: Screen = {
  ...bg('bg_menu'),
  enter(root) {
    img(root, 'hero_menu', 17, 522, 643, 543);
    img(root, 'logo_title', 449, 22, 1021, 340); // emblem is part of the new logo
    line(root, t('menu.tagline'), 336, 30, '#e8d4a8').style.fontWeight = '700';
    line(root, t('menu.subtitle'), 376, 22, '#9aa0b0');
    if (game.meta.best) line(root, t('menu.best', { n: game.meta.best }), 419, 30, '#e8b84a').style.fontWeight = '700';
    const run = game.run;
    if (run) {
      const from = run.dead ? Math.max(1, run.floor - 2) : run.floor;
      imgButton(root, 'btn_continue', '', 960, 536, 476, 101, 0, () => {
        if (run.dead) { continueRun(run); game.screens.go('descending'); } else game.screens.go('battle');
      }, 'continue');
      line(root, t('menu.continueSub', { n: from }), 604, 22, '#8fd4a0').style.fontWeight = '700';
    }
    imgButton(root, 'btn_play', '', 960, 686, 497, 122, 0, startNewRun, 'play');
    imgButton(root, 'btn_inventory', '', 960, 810, 495, 122, 0, () => game.screens.go('inventory'), 'inventory');
    imgButton(root, 'btn_settings', '', 240, 1012, 301, 78, 0, openSettings, 'settings');
    imgButton(root, 'btn_leaderboard', '', 1680, 1012, 304, 75, 0, () => {}, 'leaderboard');
    KEY_TYPES.forEach((k, i) => {
      img(root, `key_${k}`, 714 + i * 193, 978, 41, 66);
      const c = place(el('div', 'abs', String(game.meta.keys[k]), root), 774 + i * 193, 988, 60);
      Object.assign(c.style, { fontSize: '36px', fontWeight: '700', color: KEY_COLOR[k] });
    });
    season(root);
  },
};

const inventory: Screen = {
  ...bg('bg_menu'),
  enter(root) {
    imgButton(root, 'btn_back', '', 180, 79, 316, 78, 0, () => game.screens.go('menu'), 'back');
    img(root, 'logo_title', 632, 4, 656, 219);
    img(root, 'hero_menu', 241, 341, 678, 572); // new menu background has no hero: it is a separate sprite
    new InventoryView(root, game.meta, { mode: 'menu', onChange: save });
  },
};

export function registerScreens() {
  const s = game.screens;
  s.add('menu', menu);
  s.add('path', pathScreen);
  s.add('choose', chooseScreen);
  s.add('battle', battleScreen);
  s.add('died', diedScreen);
  s.add('descending', descendingScreen);
  s.add('inventory', inventory);
}
