// Battle screen: simulates the floor once (core), plays it (presenter), HUD in DOM, key drop, VICTORY, then cards / death.
// HUD layout measured on the original's screenshots (1920×1080).
import { simulateFloor, type FloorResult } from '../battle/core';
import { FOE_X, GROUND, Presenter } from '../battle/presenter';
import { assetUrl } from '../core/assets';
import { snapshot } from '../core/backdrop';
import { Rng } from '../core/rng';
import type { Screen } from '../core/screens';
import { t } from '../core/strings';
import { DATA } from '../data';
import { KEY_TYPES, type FloorNode, type KeyType } from '../data/types';
import { bus } from '../events';
import { game } from '../game';
import { openSettings } from '../overlays';
import { platform } from '../platform';
import { heroEffects, heroStats, isReplay, rollKeys, type RunState } from '../run';
import { css, el, img, imgButton, line, place } from '../ui';

css(`
.hp-frame{position:absolute;width:517px;height:40px;background:-4px -6px/524px 51px no-repeat}
.vside{position:static}
.vside.flip{transform:scaleX(-1)}
.hp-fill{position:absolute;left:16px;top:7px;bottom:7px;transition:width var(--hpt,.18s) ease-out}
.hp-ghost{position:absolute;left:16px;top:7px;bottom:7px;background:#fff8e8;opacity:.75}
.hp-fill.hero{background:linear-gradient(#ff6a5a,#d8302a 55%,#a81e1a)}
.hp-fill.foe{background:linear-gradient(#5aa8ff,#2f6fd8 55%,#1e4aa8)}
.hp-val{position:absolute;inset:0;display:flex;align-items:center;justify-content:center;font-size:28px;font-weight:700;color:#fff;text-shadow:0 0 4px #000,0 2px 2px #000}
.hud-name{position:absolute;width:517px;text-align:center}
.banner-img{position:absolute;left:50%;transform:translateX(-50%);transition:opacity .4s}
.status{position:absolute;left:0;right:0;top:990px;display:flex;justify-content:center;align-items:center;gap:22px;font-size:30px}
.keydrop{position:absolute;text-align:center;width:400px;font-size:40px;font-weight:700;text-shadow:0 3px 8px #000}
.keydrop .keyrow{display:flex;justify-content:center;gap:8px;margin-top:10px}
.keydrop img{display:block;height:120px;animation:keyfall 1s ease-out}
@keyframes keyfall{0%{transform:translateY(-140px) rotate(-30deg);opacity:0}40%{opacity:1}70%{transform:translateY(16px) rotate(10deg)}100%{transform:none}}
.vic{position:absolute;inset:0;background:#000a;z-index:3}
.vic > *{animation:vicin .35s ease-out}
.boss-pop{position:absolute;left:50%;transform:translateX(-50%);pointer-events:none;animation:bosspop .45s cubic-bezier(.2,1.6,.4,1) both}
.boss-pop.out{transition:opacity .4s;opacity:0}
@keyframes bosspop{0%{transform:translateX(-50%) scale(2.2);opacity:0}100%{transform:translateX(-50%) scale(1);opacity:1}}
@keyframes vicin{0%{transform:translateY(-60px);opacity:0}100%{transform:none;opacity:1}}
`);

interface BattleState { floor: number; node: FloorNode; result: FloorResult; presenter: Presenter; ended: boolean; key: KeyType | null }
let B: BattleState | null = null;
let ui: Partial<Record<'root' | 'hero' | 'foe' | 'foeBox' | 'banner' | 'fast' | 'diamonds' | 'status', HTMLElement>> = {};
let timers: { left: number; fn: () => void }[] = [];
const setTimer = (sec: number, fn: () => void) => { timers.push({ left: sec, fn }); };

const KEY_COLOR: Record<KeyType, string> = { copper: '#e08a4a', silver: '#d8e0e8', gold: '#ffd23a' };

function startFloor(run: RunState): BattleState {
  const node = DATA.floors.floor(run.floor).nodes[0];
  const stats = heroStats(run);
  run.hp = Math.min(run.hp, stats.maxHp);
  const result = simulateFloor({
    hero: { stats, hp: run.hp, effects: heroEffects(run) },
    foes: node.foes,
    seed: `${run.seed}:${run.floor}:${run.attempt}`,
    heroFirst: run.heroFirstNext,
    god: game.debug.god,
  });
  run.heroFirstNext = false;
  const state: BattleState = { floor: run.floor, node, result, ended: false, key: null, presenter: null as unknown as Presenter };
  state.presenter = new Presenter(result.log, node.foes, run.hp, stats.maxHp, run.floor, node.type === 'boss', {
    hud: () => paintHud(),
    banner: (kind) => showBanner(kind),
    status: (kind) => showStatus(kind),
    end: () => onEnd(state),
  });
  bus.emit('battle_start', { floor: run.floor, heroFirst: result.log.some((e) => e.t === 'start' && e.heroFirst) });
  return state;
}

function onEnd(s: BattleState) {
  if (s.ended || B !== s) return;
  s.ended = true;
  const run = game.run!;
  game.meta.best = Math.max(game.meta.best, s.floor);
  run.killAtk = (run.killAtk || 0) + s.result.atkGained;
  if (!s.result.cleared) {
    run.dead = true;
    run.hp = 0;
    game.screens.go('died');
    return;
  }
  run.hp = s.result.heroHp;
  const keys = rollKeys(s.node.keyDrop, s.node.keys || 1, new Rng(`${run.seed}:${s.floor}:${run.attempt}:key`));
  s.key = keys[0] || null;
  const victory = () => { if (B === s && game.screens.current === 'battle') showVictory(s); };
  if (!keys.length) { setTimer(0.3, victory); return; }
  for (const key of keys) {
    game.meta.keys[key]++;
    run.keysGained[key]++;
    bus.emit('key_drop', { key });
  }
  if (ui.root) showKeys(keys);
  setTimer(DATA.timing.keyDrop, victory);
}

function showKeys(keys: KeyType[]) {
  const types = KEY_TYPES.filter((k) => keys.includes(k));
  const text = types.map((k) => (n => t(n > 1 ? 'battle.keyMany' : 'battle.key', { n, k: t(`key.${k}`) }))(keys.filter((x) => x === k).length)).join('<br>');
  const d = place(el('div', 'keydrop', text, ui.root), FOE_X - 200, GROUND - 380 - 46 * (types.length - 1));
  d.style.color = KEY_COLOR[types[types.length - 1]];
  const row = el('div', 'keyrow', '', d);
  for (const key of keys) el('img', '', '', row).src = assetUrl(`key_${key}`);
  paintTopRight();
}

function showVictory(s: BattleState) {
  ui.root?.querySelector('.keydrop')?.remove();
  ui.root?.querySelector('.boss-pop')?.remove();
  const v = el('div', 'vic', '', ui.root);
  img(v, 'orn_victory_top', 790, 306, 340, 66);
  img(v, 'text_victory', 624, 358, 672, 139);
  const mid = place(el('div', 'abs', '', v), 0, 522, 1920, 60);
  Object.assign(mid.style, { display: 'flex', justifyContent: 'center', alignItems: 'center', gap: '18px' });
  img(mid, 'orn_victory_side', 0, 0, 152, 30, 'vside');
  el('div', 'gold', t('victory.sub', { n: s.floor }), mid).style.fontSize = '38px';
  img(mid, 'orn_victory_side', 0, 0, 152, 30, 'vside flip');
  img(v, 'orn_victory_bottom', 818, 593, 284, 44);
  const go = () => {
    if (B !== s || game.screens.current !== 'battle') return;
    game.screens.go('choose', { bigReward: s.node.type === 'boss' });
  };
  v.addEventListener('click', go);
  v.dataset.id = 'victory';
  setTimer(DATA.timing.victory, go);
}

/** Boss: huge BOSS over the scene on entry; BOSS DEFEATED when it dies (both from the original's boss clip). */
function bossText(key: 'text_boss_big' | 'text_boss_defeated', top: number, w: number, hideAfter: number) {
  if (!ui.root) return;
  ui.root.querySelector('.boss-pop')?.remove();
  const e = el('img', 'boss-pop', '', ui.root) as HTMLImageElement;
  e.src = assetUrl(key);
  Object.assign(e.style, { top: `${top}px`, width: `${w}px` });
  if (hideAfter > 0) setTimer(hideAfter, () => e.classList.add('out'));
}
const offBoss: (() => void)[] = [];

function showBanner(kind: 'ambush' | 'strike') {
  const b = ui.banner as HTMLImageElement | undefined;
  if (!b) return;
  b.src = assetUrl(kind === 'ambush' ? 'text_ambush_icon' : 'text_strike_first');
  b.style.width = kind === 'ambush' ? '336px' : '510px';
  b.style.top = `${(kind === 'ambush' ? 380 : 382) + (B?.node.type === 'boss' ? 110 : 0)}px`; // boss: under the BOSS text
  b.style.opacity = '1';
  setTimer(kind === 'ambush' ? DATA.timing.ambush + 0.4 : 1, () => { if (ui.banner === b) b.style.opacity = '0'; });
}

function showStatus(kind: 'enter' | 'fight' | 'none') {
  const s = ui.status;
  if (!s) return;
  s.innerHTML = '';
  if (kind === 'enter') {
    const e = el('span', 'mono', t('battle.entering'), s);
    e.style.color = '#c8c0b0';
  } else if (kind === 'fight') {
    el('img', '', '', s).src = assetUrl('orn_status_swords_left');
    const e = el('span', 'mono', t('battle.inProgress'), s);
    e.style.color = '#6fd47a';
    el('img', '', '', s).src = assetUrl('orn_status_swords_right');
  }
}

/** HP bar: the fill slides to the new value; on damage a light "ghost" of the lost part lingers, then drains. */
function paintBar(box: HTMLElement, hp: number, max: number) {
  const w = Math.max(0, Math.min(1, hp / max)) * 485;
  const fill = box.querySelector('.hp-fill') as HTMLElement;
  const ghost = box.querySelector('.hp-ghost') as HTMLElement;
  const fast = game.fast ? 3 : 1;
  box.style.setProperty('--hpt', `${0.18 / fast}s`);
  const prev = parseFloat(fill.style.width) || 0;
  if (w < prev) {
    if (!(parseFloat(ghost.style.width) > w)) ghost.style.width = `${prev}px`;
    void ghost.offsetWidth; // start the drain from the old width
    ghost.style.transition = `width ${0.35 / fast}s ease-in ${0.3 / fast}s`;
    ghost.style.width = `${w}px`;
  } else if (w > prev || !fill.style.width) {
    ghost.style.transition = 'none';
    ghost.style.width = `${w}px`;
  }
  fill.style.width = `${w}px`;
  (box.querySelector('.hp-val') as HTMLElement).textContent = `${Math.max(0, hp)} / ${max}`;
}

function paintTopRight() {
  const r = ui.root?.querySelector('.topright') as HTMLElement | null;
  if (!r) return;
  const k = game.meta.keys;
  r.innerHTML = `<div style="color:#9a9a9a">${t('battle.best', { n: game.meta.best || '—' })}</div><div style="color:#d9a640;margin-top:14px">${t('battle.keys', { c: k.copper, s: k.silver, g: k.gold })}</div>`;
}

function paintHud() {
  if (!B || !ui.hero || !ui.foe || !ui.foeBox) return;
  const p = B.presenter;
  paintBar(ui.hero, p.heroHp, p.heroMax);
  const foe = B.node.foes[p.foeIdx];
  ui.foeBox.style.visibility = foe ? 'visible' : 'hidden';
  ui.foeBox.style.opacity = p.foeDead ? '0.45' : '1';
  if (foe) {
    (ui.foeBox.querySelector('.foe-n') as HTMLElement).textContent = B.node.type === 'boss' ? t('battle.boss') : t('battle.foe', { i: p.foeIdx + 1, n: B.node.foes.length });
    (ui.foeBox.querySelector('.foe-name') as HTMLElement).textContent = foe.name;
    paintBar(ui.foe, p.foeHp, p.foeMax);
    (ui.foeBox.querySelector('.foe-ab') as HTMLElement).innerHTML = foe.abilities.map((a) => {
      const d = DATA.abilities.get(a)!;
      return `<span style="color:${d.color}">${d.label}</span>`;
    }).join(' · ');
  }
  if (ui.diamonds) ui.diamonds.querySelectorAll('img').forEach((im, i) => {
    const gone = i < p.foeIdx || (i === p.foeIdx && p.foeDead);
    (im as HTMLImageElement).src = assetUrl(gone ? 'diamond_empty' : 'diamond_full');
  });
}

function hpBar(parent: HTMLElement, x: number, side: 'hero' | 'foe') {
  const f = place(el('div', 'hp-frame', `<div class="hp-ghost"></div><div class="hp-fill ${side}"></div><div class="hp-val"></div>`, parent), x, 280);
  f.style.backgroundImage = `url(${assetUrl('hpbar_frame')})`;
  return f;
}

function buildHud(root: HTMLElement) {
  const s = B!;
  line(root, t('battle.floor', { n: s.floor }), 104, 86, '', 'gold');
  ui.diamonds = el('div', 'abs', '', root);
  if (s.node.type === 'boss') line(root, `☠ ${t('battle.boss')} ☠`, 196, 34, '#ff4a3a');
  else s.node.foes.forEach((_, i) => img(ui.diamonds!, 'diamond_full', 960 + (i - (s.node.foes.length - 1) / 2) * 75 - 27, 167, 54, 60));
  ui.fast = line(root, t('battle.fast'), 250, 24, '#7fd0ff', 'mono');
  const tr = place(el('div', 'abs mono topright', '', root), 1400, 34, 478);
  Object.assign(tr.style, { textAlign: 'right', fontSize: '30px' });
  paintTopRight();

  const you = place(el('div', 'hud-name mono', t('battle.you'), root), 321, 236);
  Object.assign(you.style, { fontSize: '30px', color: '#c8c0b0' });
  ui.hero = hpBar(root, 321, 'hero');
  ui.foeBox = el('div', 'abs', '', root);
  const fn = place(el('div', 'hud-name foe-n', '', ui.foeBox), 1083, 214);
  Object.assign(fn.style, { fontSize: '24px', fontWeight: '700', color: '#c8c0b0' });
  const nm = place(el('div', 'hud-name mono foe-name', '', ui.foeBox), 1083, 246);
  Object.assign(nm.style, { fontSize: '30px', color: '#a8c8e0' });
  ui.foe = hpBar(ui.foeBox, 1083, 'foe');
  const ab = place(el('div', 'hud-name mono foe-ab', '', ui.foeBox), 1083, 332);
  ab.style.fontSize = '22px';

  ui.banner = el('img', 'banner-img', '', root);
  ui.banner.style.opacity = '0';
  ui.status = el('div', 'status', '', root);
  imgButton(root, 'btn_gear', '', 1860, 1019, 86, 86, 0, openSettings, 'gear');
  paintHud();
}

/** Debug: log of the last simulated floor. */
export function lastBattleLog() {
  return B?.result.log || [];
}

/** Debug / tests: end the current floor right away. */
export function debugEndFloor() {
  B?.presenter.finish();
}

export const battleScreen: Screen = {
  enter(root, params) {
    const run = game.run;
    if (!run) { game.screens.go('menu'); return; }
    if (!(params.resume && B && B.floor === run.floor && !B.ended)) {
      timers = [];
      B = startFloor(run);
    }
    ui = { root };
    buildHud(root);
    offBoss.push(
      bus.on('boss_appear', () => bossText('text_boss_big', 340, 630, 1.6)),
      bus.on('boss_death', () => bossText('text_boss_defeated', 420, 1015, 0)),
    );
    platform.gameplayStart();
  },
  exit() {
    offBoss.splice(0).forEach((f) => f());
    platform.gameplayStop();
    snapshot(game.stage.canvas);
    game.fast = false;
    ui = {};
  },
  update(dt) {
    const run = game.run;
    game.fast = !!run && game.settings.fastReplay && isReplay(run);
    if (ui.fast) ui.fast.style.visibility = game.fast ? 'visible' : 'hidden';
    B?.presenter.update(dt);
    for (const tm of timers.slice()) if ((tm.left -= dt) <= 0) { timers.splice(timers.indexOf(tm), 1); tm.fn(); }
  },
  render(ctx) { B?.presenter.render(ctx, !!B.key && B.ended); },
};
