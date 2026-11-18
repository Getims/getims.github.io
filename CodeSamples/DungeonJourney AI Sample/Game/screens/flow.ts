// You Died (revive / menu / play again) and DESCENDING (continue). Layout measured on the original's screenshots.
import { images } from '../core/assets';
import type { Screen } from '../core/screens';
import { H, W } from '../core/stage';
import { t } from '../core/strings';
import { DATA } from '../data';
import { bus } from '../events';
import { game } from '../game';
import { platform } from '../platform';
import { newRun, revive } from '../run';
import { css, drawCover, el, img, imgButton, line } from '../ui';

css(`
.flyperk{position:absolute;left:960px;top:540px;font-size:30px;font-weight:700;white-space:nowrap;transform:translate(-50%,-50%);transition:transform 1s ease-out,opacity 1s ease-in;text-shadow:0 2px 6px #000}
`);

/** Countdown driven by the screen's update (respects pause/speed). */
function timer() {
  let left = -1;
  let fn: (() => void) | null = null;
  return {
    set(sec: number, f: () => void) { left = sec; fn = f; },
    clear() { fn = null; },
    tick(dt: number) { if (fn && (left -= dt) <= 0) { const f = fn; fn = null; f(); } },
  };
}

export function startNewRun() {
  newRun();
  game.screens.go('path');
}

const diedTimer = timer();
let onSpace: ((e: KeyboardEvent) => void) | null = null;
export const diedScreen: Screen = {
  render: (ctx) => drawCover(ctx, images.bg_fail, W, H),
  enter(root) {
    const run = game.run!;
    const g = run.keysGained;
    img(root, 'text_you_died', 676, 108, 566, 142);
    line(root, t('died.reached', { n: run.floor }), 337, 56, '#f3e2c2');
    line(root, t('died.best', { n: game.meta.best }), 404, 32, '#a8a8d0');
    img(root, 'orn_fail_divider', 614, 423, 690, 59);
    line(root, t('died.keys', { c: g.copper, s: g.silver, g: g.gold }), 503, 30, '#e8b84a');
    const rank = line(root, t('died.ranking'), 563, 32, '#e8c060');
    diedTimer.set(DATA.timing.rankDelay, () => {
      const total = 7757;
      const r = Math.max(1, Math.round(total * Math.max(0.02, 1 - run.floor / 30)) + Math.floor(Math.random() * 50));
      rank.textContent = t('died.rank', { r, n: total });
    });
    imgButton(root, 'btn_revive', '', 960, 686, 592, 102, 0, async () => {
      if (!(await platform.rewarded('revive'))) return;
      revive(run);
      bus.emit('revive', { floor: run.floor });
      game.screens.go('battle');
    }, 'revive');
    imgButton(root, 'btn_mainmenu', '', 724, 826, 432, 143, 0, () => game.screens.go('menu'), 'menu');
    imgButton(root, 'btn_playagain', '', 1197, 826, 436, 144, 0, startNewRun, 'again');
    line(root, t('died.space'), 928, 22, '#8a8a8a');
    onSpace = (e) => { if (e.code === 'Space') { e.preventDefault(); startNewRun(); } };
    addEventListener('keydown', onSpace);
  },
  exit() {
    diedTimer.clear();
    if (onSpace) removeEventListener('keydown', onSpace);
    onSpace = null;
  },
  update: (dt) => diedTimer.tick(dt),
};

const descTimer = timer();
export const descendingScreen: Screen = {
  render(ctx) { ctx.fillStyle = '#05040a'; ctx.fillRect(0, 0, W, H); },
  enter(root) {
    const run = game.run!;
    line(root, t('descending.title', { n: run.floor }), 540, 64, '', 'gold');
    const names = run.perks.map((id) => DATA.perks.get(id)!).filter(Boolean);
    names.forEach((p, i) => {
      const e = el('div', 'flyperk', p.name.toUpperCase(), root);
      e.style.color = p.rarity === 'common' ? '#e8e0d0' : DATA.balance.rarities[p.rarity].color;
      const a = (i / Math.max(1, names.length)) * Math.PI * 2;
      requestAnimationFrame(() => requestAnimationFrame(() => {
        e.style.transform = `translate(calc(-50% + ${Math.cos(a) * 700}px), calc(-50% + ${Math.sin(a) * 380}px))`;
        e.style.opacity = '0';
      }));
    });
    descTimer.set(DATA.timing.descend, () => game.screens.go('battle'));
  },
  exit() { descTimer.clear(); },
  update: (dt) => descTimer.tick(dt),
};
