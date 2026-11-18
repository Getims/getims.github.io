// Perk cards: CHOOSE YOUR PATH (run start, no reroll) and CHOOSE ONE (after a floor: lock + reroll for an ad).
// Background = blurred last battle frame. Spinner on the cards: names/frames flicker, cards stop left to right, then text appears.
// Layout measured on the original's screenshots.
import { images } from '../core/assets';
import { drawBackdrop, snapshot } from '../core/backdrop';
import type { Screen } from '../core/screens';
import { t } from '../core/strings';
import { DATA } from '../data';
import { RARITIES, type PerkDef, type Rarity } from '../data/types';
import { bus } from '../events';
import { game } from '../game';
import { openBuild, openSettings } from '../overlays';
import { platform } from '../platform';
import { perkPreview, pickPerk } from '../run';
import { burst } from '../fx';
import { button, css, el, img, imgButton, line, place, rarityColor } from '../ui';
import { assetUrl } from '../core/assets';

css(`
.pcard{position:absolute;top:345px;width:395px;height:545px;background:center/100% 100% no-repeat;cursor:pointer;text-align:center;transition:filter .15s,transform .15s ease-out}
.pcard:hover:not(.spin){filter:drop-shadow(0 0 22px var(--rc));transform:scale(1.04)}
.pcard.spin{cursor:default}
.pcard > div{position:absolute;left:30px;right:30px}
.pcard .r{top:38px;font-size:32px;font-weight:700;color:var(--rc)}
.pcard .n{top:150px;height:72px;display:flex;align-items:center;justify-content:center;font-size:52px;font-weight:700;color:var(--nc);white-space:nowrap}
.pcard .dv{top:256px;height:40px;background:center/288px 40px no-repeat}
.pcard .d{top:310px;font-size:32px;line-height:1.22;color:#d8d0c0}
.pcard .p{top:425px;font-size:26px;font-weight:700;line-height:1.25;color:#e5b85a}
.pcard .lock{position:absolute;left:auto;right:-14px;top:-26px;width:44px;height:44px;border:0;background:none;font-size:28px;cursor:pointer;filter:grayscale(1) brightness(.75)}
.pcard .lock.on{filter:none}
.cards-shake{animation:cshake .12s linear infinite}
@keyframes cshake{0%{transform:translate(0,0)}25%{transform:translate(-4px,2px)}50%{transform:translate(3px,-3px)}75%{transform:translate(-2px,-2px)}100%{transform:translate(0,0)}}
.pick-flash{position:fixed;inset:0;z-index:50;pointer-events:none;animation:pflash .45s ease-out forwards}
@keyframes pflash{0%{opacity:calc(.85 * var(--fk,1))}100%{opacity:0}}
.pcard.pop{animation:cpop .22s ease-out}
@keyframes cpop{0%{transform:scale(1.1);filter:brightness(1.8) drop-shadow(0 0 30px var(--rc))}100%{transform:scale(1)}}
.bigpulse{animation:bpulse .7s ease-in-out infinite alternate}
@keyframes bpulse{0%{opacity:.55;transform:scale(.97)}100%{opacity:1;transform:scale(1.03)}}
`);

/** Full-screen flash in the picked card's rarity color (common = white); lives on body so it survives the screen change. */
function pickFlash(r: Rarity) {
  const f = el('div', 'pick-flash', '', document.body);
  f.style.background = r === 'common' ? '#ffffff' : rarityColor(r);
  f.style.setProperty('--fk', String(game.flashK));
  f.addEventListener('animationend', () => f.remove());
}

const CENTERS = [464, 959, 1454];

interface Slot { perk: PerkDef; locked: boolean; stopAt: number; shown: PerkDef; tick: number; el: HTMLElement }

const GOOD: Rarity[] = ['rare', 'epic', 'legendary'];

/** Weighted random perks, no duplicates on screen. Normal screens (rarities = null): every Rare+ card already on
 *  screen (locked ones too) halves the Rare / Epic / Legendary weights for the next cards; BIG REWARD keeps its weights. */
function rollPerks(count: number, exclude: Set<string>, rarities: Rarity[] | null, goodOnScreen = 0): PerkDef[] {
  let good = goodOnScreen;
  const out: PerkDef[] = [];
  const all = [...DATA.perks.values()].filter((p) => !exclude.has(p.id) && (!rarities || rarities.includes(p.rarity)));
  for (let i = 0; i < count && all.length; i++) {
    const w: Partial<Record<Rarity, number>> = {};
    for (const r of RARITIES) if (all.some((p) => p.rarity === r)) w[r] = DATA.balance.rarities[r].weight * (!rarities && GOOD.includes(r) ? 0.5 ** good : 1);
    const rs = Object.keys(w) as Rarity[];
    let x = Math.random() * rs.reduce((s, r) => s + w[r]!, 0);
    let rarity = rs[0];
    for (const r of rs) if ((x -= w[r]!) < 0) { rarity = r; break; }
    const pool = all.filter((p) => p.rarity === rarity);
    const p = pool[Math.floor(Math.random() * pool.length)];
    out.push(p);
    if (GOOD.includes(p.rarity)) good++;
    all.splice(all.indexOf(p), 1);
  }
  return out;
}

const frameOf = (r: Rarity) => `card_frame_${r}`;

function makeCardsScreen(mode: 'path' | 'choose'): Screen {
  let slots: Slot[] = [];
  let clock = 0;
  let spinning = false;
  let rerollBtn: HTMLButtonElement | null = null;
  let bigEl: HTMLElement | null = null;
  let rootEl: HTMLElement | null = null;
  let rarities: Rarity[] | null = null;
  const all = () => [...DATA.perks.values()];

  const paint = (s: Slot, final: boolean) => {
    const p = final ? s.perk : s.shown;
    const c = rarityColor(p.rarity);
    s.el.style.backgroundImage = `url(${assetUrl(frameOf(p.rarity))})`;
    s.el.style.setProperty('--rc', c);
    s.el.style.setProperty('--nc', p.rarity === 'common' ? '#f0e8d8' : c);
    s.el.classList.toggle('spin', !final);
    (s.el.querySelector('.n') as HTMLElement).textContent = p.name.toUpperCase();
    (s.el.querySelector('.r') as HTMLElement).textContent = final ? t(`rarity.${p.rarity}`) : '';
    (s.el.querySelector('.dv') as HTMLElement).style.visibility = final ? 'visible' : 'hidden';
    (s.el.querySelector('.d') as HTMLElement).textContent = final ? p.desc.toUpperCase() : '';
    (s.el.querySelector('.p') as HTMLElement).textContent = final && game.run ? perkPreview(game.run, p) : '';
    fitText(s.el.querySelector('.n') as HTMLElement, 52);
    if (final) fitBody(s.el);
  };

  /** Card stopped: pop + flash in the rarity color; sparks grow with the rarity (common: none). */
  const stopFx = (s: Slot, i: number) => {
    s.el.classList.remove('pop');
    void s.el.offsetWidth;
    s.el.classList.add('pop');
    const r = s.perk.rarity;
    const lvl = RARITIES.indexOf(r);
    if (lvl > 0) burst(CENTERS[i], 617, { count: 6 + lvl * 8, color: rarityColor(r), shape: 'spark', speed: [200, 380 + lvl * 120], life: [0.4, 0.7 + lvl * 0.1], size: [14, 22 + lvl * 4], drag: 3, spread: 160 });
    bus.emit('card_stop', { index: i, rarity: r });
  };

  const spin = () => {
    const T = DATA.timing;
    const keep = new Set(slots.filter((s) => s.locked).map((s) => s.perk.id));
    const fresh = rollPerks(slots.filter((s) => !s.locked).length, keep, rarities, slots.filter((s) => s.locked && GOOD.includes(s.perk.rarity)).length);
    let k = 0;
    let order = 0;
    clock = 0;
    spinning = true;
    if (game.settings.shake) rootEl?.classList.add('cards-shake');
    bus.emit('card_spin_start', { count: slots.filter((s) => !s.locked).length });
    for (const s of slots) {
      if (s.locked) { s.stopAt = 0; continue; }
      s.perk = fresh[k++] || s.perk;
      s.stopAt = T.spinMin + order++ * T.lockStep;
      s.tick = 0;
    }
    if (rerollBtn) rerollBtn.disabled = true;
    if (bigEl) bigEl.style.visibility = 'visible';
  };

  return {
    render: (ctx) => drawBackdrop(ctx),
    enter(root, params) {
      rootEl = root;
      if (mode === 'path') snapshot(images.bg_battle);
      const C = DATA.balance.cards;
      rarities = mode === 'path' ? [C.pathRarity] : params.bigReward ? C.bigRewardRarities : null;
      img(root, 'orn_header_card', 816, 41, 289, 58);
      line(root, t(mode === 'path' ? 'path.title' : 'choose.title'), 155, 110, '', 'gold');
      img(root, 'orn_small_divider', 877, 232, 166, 26);
      bigEl = params.bigReward ? line(root, t('choose.big'), 300, 40, '#ffd23a') : null;
      bigEl?.classList.add('bigpulse');
      slots = [];
      for (let i = 0; i < C.count; i++) {
        const c = place(el('div', 'pcard spin', '<div class="r"></div><div class="n"></div><div class="dv"></div><div class="d"></div><div class="p"></div>', root), CENTERS[i] - 197, 345);
        (c.querySelector('.dv') as HTMLElement).style.backgroundImage = `url(${assetUrl('orn_card_divider')})`;
        c.dataset.id = `card${i}`;
        const s: Slot = { perk: all()[0], shown: all()[0], locked: false, stopAt: 0, tick: 0, el: c };
        slots.push(s);
        c.addEventListener('click', () => {
          if (spinning || !game.run) return;
          pickPerk(game.run, s.perk);
          pickFlash(s.perk.rarity);
          bus.emit('card_pick', { perk: s.perk.id });
          if (mode === 'choose') game.run.floor++;
          game.screens.go('battle');
        });
        if (mode === 'choose') {
          const lock = el('button', 'lock', '🔒', c);
          lock.dataset.id = `lock${i}`;
          lock.addEventListener('click', (e) => {
            e.stopPropagation();
            if (spinning) return;
            s.locked = !s.locked;
            lock.classList.toggle('on', s.locked);
            bus.emit('card_lock', { index: i, locked: s.locked });
          });
        }
      }
      if (mode === 'choose') {
        rerollBtn = button(root, t('choose.reroll'), async () => {
          if (spinning || slots.every((s) => s.locked)) return;
          if (await platform.rewarded('reroll')) spin();
        }, 'tbtn', 'reroll');
        Object.assign(rerollBtn.style, { left: '0', right: '0', top: '998px', fontSize: '38px', color: '#8ec8ff' });
        imgButton(root, 'btn_stats', '', 1772, 68, 90, 90, 0, openBuild, 'stats');
      } else rerollBtn = null;
      imgButton(root, 'btn_gear', '', 1860, 1020, 86, 86, 0, openSettings, 'gear');
      spin();
    },
    update(dt) {
      if (!spinning) return;
      clock += dt;
      const T = DATA.timing;
      let still = false;
      slots.forEach((s, i) => {
        if (s.locked || clock >= s.stopAt) { if (s.stopAt >= 0) { paint(s, true); s.stopAt = -1; if (!s.locked) stopFx(s, i); } return; }
        still = true;
        if ((s.tick -= dt) <= 0) {
          s.tick = T.spinTick;
          const pool = all();
          s.shown = pool[Math.floor(Math.random() * pool.length)];
          paint(s, false);
          bus.emit('card_spin_tick', { index: i });
        }
      });
      if (!still) {
        spinning = false;
        rootEl?.classList.remove('cards-shake');
        if (rerollBtn) rerollBtn.disabled = false;
        if (bigEl) bigEl.style.visibility = 'hidden';
      }
    },
  };
}

const BODY_TOP = 310, BODY_BOTTOM = 505; // inside the frame, under the divider

/** Description + preview: the preview goes right under the description; both shrink until they end inside the frame. */
function fitBody(card: HTMLElement) {
  const d = card.querySelector('.d') as HTMLElement, p = card.querySelector('.p') as HTMLElement;
  for (let k = 0; k <= 7; k++) {
    d.style.fontSize = `${32 - k * 2}px`;
    p.style.fontSize = `${26 - k * 2}px`;
    p.style.top = `${BODY_TOP + d.offsetHeight + 12}px`;
    if (p.offsetTop + p.offsetHeight <= BODY_BOTTOM) return;
  }
}

/** Shrinks the font until the text fits its box (long names in the original overflow the frame). */
function fitText(e: HTMLElement, max: number) {
  let size = max;
  e.style.fontSize = `${size}px`;
  while (size > 20 && e.scrollWidth > e.clientWidth) e.style.fontSize = `${(size -= 2)}px`;
}

export const pathScreen = makeCardsScreen('path');
export const chooseScreen = makeCardsScreen('choose');
