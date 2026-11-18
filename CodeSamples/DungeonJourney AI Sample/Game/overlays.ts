// Modal overlays on top of the current screen (blurred behind, game paused): Settings and Your Build.
// Layouts measured on the original's screenshots.
import { effectiveAtk } from './battle/stats';
import { assetUrl } from './core/assets';
import { t } from './core/strings';
import { DATA } from './data';
import { bus } from './events';
import { game } from './game';
import { abandonRun, heroStats } from './run';
import { resetSave, save, saveSoon } from './save';
import { previewSfx, syncSound } from './sound';
import { button, css, el, img, imgButton, place, rarityColor } from './ui';

css(`
.set-panel{position:absolute;left:384px;top:172px;width:1152px;height:790px;box-sizing:border-box;border:4px solid #a07a3a;outline:2px solid #3a2a14;outline-offset:-10px;
  background:radial-gradient(ellipse at 50% 40%,#183028 0%,#0f1f1a 60%,#0a1512 100%)}
.set-lbl{position:absolute;left:178px;font-size:34px;font-weight:700;color:#efe2c4;font-variant:small-caps;text-shadow:0 2px 3px #000}
.set-pct{position:absolute;left:912px;font-size:30px;font-weight:700;color:#efe2c4}
.set-track{position:absolute;left:435px;width:440px;height:16px;border-radius:8px;background:#2a2244;cursor:pointer}
.set-track i{position:absolute;left:0;top:0;bottom:0;border-radius:8px;background:#3cc8b4}
.set-track b{position:absolute;top:-16px;width:48px;height:48px;margin-left:-24px;background:center/100% no-repeat}
.tog{position:absolute;width:142px;height:69px;font-size:30px;border:3px solid #b08a3a;color:#efe2c4}
.tog.on{background:#3aa64a}.tog.off{background:#241c40}.tog.dim{filter:brightness(.55)}
.bld{position:absolute;left:340px;top:80px;width:1240px;height:920px;box-sizing:border-box;border:4px solid #c8963c;background:#0e0b16}
.bld .row{position:absolute;left:60px}
.bld .k{font-size:34px;line-height:38px;color:#d8d0c0;font-variant:small-caps;cursor:help}
.bld .v{position:absolute;left:250px;top:-4px;font-size:38px;font-weight:700;color:#fff}
.bld .s{font-size:22px;line-height:26px;font-weight:700;color:#c8c0a8;max-width:560px}
.bld .perk{position:absolute;left:680px;font-size:30px;font-weight:700;cursor:help}
.tip{position:absolute;z-index:30;max-width:520px;background:#000e;border:2px solid #6a5232;border-radius:8px;padding:10px 14px;font-size:22px;color:#e8dcc0;pointer-events:none}
`);

let openCount = 0;
function overlay(id: string) {
  const o = el('div', 'overlay', '', game.stage.ui);
  o.dataset.id = id;
  openCount++;
  game.loop.paused = true;
  const close = () => {
    o.remove();
    if (--openCount <= 0) { openCount = 0; game.loop.paused = false; }
  };
  return { o, close };
}

export function closeOverlays() {
  game.stage.ui.querySelectorAll('.overlay').forEach((e) => e.remove());
  openCount = 0;
  game.loop.paused = false;
}

export function openSettings() {
  const { o, close } = overlay('settings');
  const s = game.settings;
  imgButton(o, 'btn_back', '', 172, 82, 316, 78, 0, close, 'back');
  const p = el('div', 'set-panel', '', o);
  const title = el('div', 'gold cx', t('settings.title'), p);
  Object.assign(title.style, { top: '58px', fontSize: '84px' });
  img(p, 'orn_settings_divider', 308, 155, 535, 40);

  const slider = (label: string, key: 'music' | 'sfx', y: number) => {
    place(el('div', 'set-lbl', label, p), 178, y - 24);
    const tr = place(el('div', 'set-track', '', p), 435, y - 8);
    tr.dataset.id = `vol_${key}`;
    const fill = el('i', '', '', tr);
    const thumb = el('b', '', '', tr);
    thumb.style.backgroundImage = `url(${assetUrl('slider_thumb')})`;
    const pct = place(el('div', 'set-pct', '', p), 912, y - 20);
    const paint = () => { const v = Math.round(s[key] * 100); fill.style.width = `${v}%`; thumb.style.left = `${v}%`; pct.textContent = `${v}%`; };
    const setFrom = (e: PointerEvent) => {
      const r = tr.getBoundingClientRect();
      s[key] = Math.max(0, Math.min(1, (e.clientX - r.left) / r.width));
      paint();
      syncSound();
      saveSoon();
    };
    tr.addEventListener('pointerdown', (e) => { tr.setPointerCapture(e.pointerId); setFrom(e); });
    tr.addEventListener('pointermove', (e) => { if (tr.hasPointerCapture(e.pointerId)) setFrom(e); });
    if (key === 'sfx') tr.addEventListener('pointerup', previewSfx);
    paint();
  };
  const toggle = (label: string, key: 'shake' | 'fastReplay', y: number) => {
    place(el('div', 'set-lbl', label, p), 178, y - 24);
    const on = button(p, t('common.on'), () => { s[key] = true; paint(); saveSoon(); }, 'box tog on', `${key}_on`);
    const off = button(p, t('common.off'), () => { s[key] = false; paint(); saveSoon(); }, 'box tog off', `${key}_off`);
    place(on, 564, y - 35);
    place(off, 714, y - 35);
    const paint = () => { on.classList.toggle('dim', !s[key]); off.classList.toggle('dim', s[key]); };
    paint();
  };
  slider(t('settings.music'), 'music', 255);
  slider(t('settings.sfx'), 'sfx', 359);
  toggle(t('settings.shake'), 'shake', 479);
  toggle(t('settings.fast'), 'fastReplay', 591);
  if (game.run && ['battle', 'choose'].includes(game.screens.current)) {
    imgButton(p, 'btn_quit', '', 576, 702, 405, 100, 0, () => { close(); abandonRun(game.run!); save(); game.screens.go('menu'); }, 'quit');
  }
  // main menu: wipe all progress (save, items, keys, best); asks once more before doing it
  if (game.screens.current === 'menu') {
    let armed = false;
    const b = button(p, t('settings.restart'), () => {
      if (!armed) { armed = true; b.textContent = t('settings.restartSure'); b.style.color = '#ff6a5a'; return; }
      resetSave();
      save();
      closeOverlays();
      syncSound();
      game.screens.go('menu');
    }, 'box', 'restart');
    Object.assign(b.style, { position: 'absolute', left: '376px', top: '676px', width: '405px', height: '76px', fontSize: '34px', borderColor: '#c04040', background: '#301414', color: '#f08a8a', fontFamily: 'Cinzel,serif', fontWeight: '700' });
  }
}

/** Hover tooltip inside the stage. */
function tip(root: HTMLElement, target: HTMLElement, text: string) {
  let box: HTMLElement | null = null;
  target.addEventListener('mouseenter', () => {
    box = el('div', 'tip', text, root);
    const r = target.getBoundingClientRect();
    const o = root.getBoundingClientRect();
    const s = game.stage.scale;
    place(box, (r.left - o.left) / s, (r.bottom - o.top) / s + 6);
  });
  target.addEventListener('mouseleave', () => { box?.remove(); box = null; });
}

const pct = (v: number) => `${Math.round(v * 100)}%`;

export function openBuild() {
  const { o, close } = overlay('build');
  const p = el('div', 'bld', '', o);
  const title = el('div', 'gold cx', t('build.title'), p);
  Object.assign(title.style, { top: '22px', fontSize: '64px' });
  const run = game.run;
  const s = heroStats(run || { perks: [] });
  const head = place(el('div', 'abs', t('build.head', { f: run?.floor ?? 1, hp: Math.max(0, run?.hp ?? s.maxHp), max: s.maxHp }), p), 60, 128);
  Object.assign(head.style, { fontSize: '32px', fontWeight: '700', color: '#e8d8b0' });

  const fromDef = effectiveAtk(s) - s.atk;
  const { defK, maxBlock } = DATA.balance.damage;
  const block = s.def > 0 ? Math.min(maxBlock, s.def / (s.def + defK)) : 0;
  const rows: [string, string, string][] = [
    [t('build.maxHp'), String(s.maxHp), ''],
    [t('build.atk'), String(s.atk), fromDef ? t('build.sub.atk', { e: effectiveAtk(s), d: fromDef }) : ''],
    [t('build.def'), String(s.def), t(block >= maxBlock ? 'build.sub.defMax' : 'build.sub.def', { v: pct(block), m: pct(maxBlock) })],
    [t('build.crit'), String(Math.round(s.crit * 100)), t('build.sub.crit', { c: pct(s.crit), m: s.critMult.toFixed(1) })],
  ];
  if (s.lifesteal) rows.push([t('build.lifesteal'), String(Math.round(s.lifesteal * 100)), t('build.sub.lifesteal', { v: pct(s.lifesteal) })]);
  if (s.thorns) rows.push([t('build.thorns'), String(Math.round(s.thorns * 100)), t('build.sub.thorns', { v: pct(s.thorns), c: pct(s.thornsCap) })]);
  if (s.execute) rows.push([t('build.execute'), '', t('build.sub.execute', { v: pct(s.execute) })]);
  if (s.atkPerKill) rows.push([t('build.perKill'), '', t('build.sub.perKill', { v: s.atkPerKill, g: game.run?.killAtk || 0 })]);
  if (s.hpToAtk) rows.push([t('build.hpToAtk'), '', t('build.sub.hpToAtk', { v: pct(s.hpToAtk) })]);
  if (s.defToAtk) rows.push([t('build.defToAtk'), '', t('build.sub.defToAtk', { v: pct(s.defToAtk) })]);
  let y = 192;
  for (const [k, v, sub] of rows) {
    const r = place(el('div', 'row', '', p), 60, y);
    el('div', 'k', k, r);
    if (v) el('div', 'v', v, r);
    if (sub) el('div', 's', sub.toUpperCase(), r);
    y += sub.length > 42 ? 94 : sub ? 68 : 48; // as in the original: label 48, +sub 68, two-line sub 94
  }

  const up = place(el('div', 'abs', t('build.upgrades'), p), 680, 136);
  Object.assign(up.style, { fontSize: '32px', fontWeight: '700', color: '#e8d8b0' });
  const hint = place(el('div', 'abs', t('build.hint'), p), 680, 178);
  Object.assign(hint.style, { fontSize: '18px', fontWeight: '700', color: '#8a8478' });
  const counts = new Map<string, number>();
  for (const id of run?.perks || []) counts.set(id, (counts.get(id) || 0) + 1);
  const list = [...counts.keys()].map((id) => DATA.perks.get(id)!);
  // columns as in the original; denser rows when there are many upgrades
  const twoCols = list.length > 11;
  const dense = list.length > 22;
  const step = dense ? 38 : 52;
  const perCol = dense ? 15 : 11;
  list.forEach((pk, i) => {
    const n = counts.get(pk.id)!;
    const e = place(el('div', 'perk', `${pk.name.toUpperCase()}${n > 1 ? ` ×${n}` : ''}`, p), 680 + Math.floor(i / perCol) * 265, 205 + (i % perCol) * step);
    if (twoCols) Object.assign(e.style, { fontSize: dense ? '21px' : '23px', maxWidth: '255px', whiteSpace: 'nowrap', overflow: 'hidden' });
    e.style.color = pk.rarity === 'common' ? '#e8e0d0' : rarityColor(pk.rarity);
    tip(o, e, pk.desc);
  });
  const c = button(p, t('build.close'), close, 'tbtn', 'close');
  Object.assign(c.style, { left: '0', right: '0', top: '820px', fontSize: '44px', color: '#f3e3c0' });
}

// A screen change closes any open overlay.
bus.on('screen', closeOverlays);
