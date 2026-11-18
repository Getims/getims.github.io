// Inventory component: equipment slots, stash grid with tabs, keys/chests, item popup, chest reward.
// Reusable: the menu uses it now; the in-run rest room (stage 7) mounts the same view with mode 'run'.
// Layout measured on the original's screenshots (1920×1080); the hero sprite is placed by the screen (menus.ts).
import { assetUrl } from './core/assets';
import { t } from './core/strings';
import { DATA } from './data';
import { bus } from './events';
import { burst, floatText, goldSparks, sparkRing } from './fx';
import { game } from './game';
import { KEY_TYPES, SLOTS, type KeyType, type Mod, type Slot } from './data/types';
import {
  convertKeys, copies, def, equip, unequip, isEquipped, itemMods, itemScore, merge, openChest, salvage, salvageValue, upgrade, upgradeCost,
  type ItemInst, type MetaState,
} from './meta';
import { button, css, el, img, line, place, rarityColor } from './ui';

css(`
.inv{position:absolute;inset:0;pointer-events:none}
.inv > *{pointer-events:auto}
.inv .slot{position:absolute;width:144px;height:144px;box-sizing:border-box;border:3px solid #6a5232;outline:1px solid #2a2014;outline-offset:-8px;background:radial-gradient(#141a18,#0a0d0c);cursor:pointer;display:flex;align-items:center;justify-content:center}
.inv .slot.full{border-color:var(--rc)}
.inv .slot.empty{border:0;outline:0;background:center/104px 109px no-repeat,center/100% 100% no-repeat}
.inv .slot .ico{max-width:104px;max-height:104px}
.inv .chk{position:absolute;right:4px;top:2px;width:30px}
.inv .lvl{position:absolute;left:8px;bottom:4px;color:#ffd23a;font-size:20px;font-weight:700}
.inv .lbl{position:absolute;width:200px;text-align:center;font-size:22px;font-weight:700;color:#c8b890;text-shadow:0 2px 3px #000}
.inv .tab{position:absolute;top:200px;width:142px;height:52px;box-sizing:border-box;border:2px solid #6a5232;border-radius:8px 8px 2px 2px;background:linear-gradient(#11201c,#0a1210);font-family:Cinzel,serif;font-weight:700;font-size:24px;color:#d8c8a8;cursor:pointer}
.inv .tab.on{color:#fff4d8;border-color:#c8a050;background:linear-gradient(#1d3a32,#11251f)}
.inv .panel{position:absolute;left:1085px;top:263px;width:750px;height:672px;box-sizing:border-box;border:3px solid #9a7a3a;background:radial-gradient(ellipse at 50% 40%,#16271f,#0d1a15 70%,#0a130f)}
.inv .cell{position:absolute;width:112px;height:112px;box-sizing:border-box;border:2px solid var(--rc);background:radial-gradient(#13201c,#080e0c);cursor:pointer;display:flex;align-items:center;justify-content:center;text-align:center}
.inv .cell .ico{max-width:92px;max-height:92px}
.inv .ph{font-size:16px;font-weight:700;color:var(--rc);padding:4px;line-height:1.1}
.inv .kbtn{position:absolute;top:995px;height:56px;border:0;background:center/100% 100% no-repeat;cursor:pointer}
.inv .kbtn:hover:not(:disabled){filter:brightness(1.18)}
.inv .kbtn.cv:disabled{filter:grayscale(.7) brightness(.55)}
.imodal{position:absolute;inset:0;z-index:5;background:#0008}
.imodal .pan{position:absolute;left:503px;top:236px;width:912px;height:584px;box-sizing:border-box;border:4px solid #a07a3a;background:radial-gradient(ellipse at 50% 40%,#183028,#0f1f1a 65%,#0a1512)}
.imodal .bar{position:absolute;left:34px;right:34px;top:2px;height:8px}
.mbtn{position:absolute;top:470px;width:170px;height:73px;border:3px solid;font-size:28px;font-family:Cinzel,serif;font-weight:700}
.rmodal{position:absolute;inset:0;z-index:5;background:rgba(26,26,46,.8)}
.rmodal .flash{position:absolute;inset:0;pointer-events:none;animation:rflash .55s ease-out forwards;background:radial-gradient(circle at 50% 47%,var(--rc) 0,transparent 55%)}
.rmodal .flash.full{background:var(--rc);animation-duration:.4s}
@keyframes rflash{0%{opacity:var(--fk,1)}100%{opacity:0}}
.rmodal .glow{position:absolute;left:660px;top:210px;width:600px;height:600px;pointer-events:none;border-radius:50%;background:radial-gradient(circle,#ffd23a 0,#ffb02055 35%,transparent 68%);animation:gglow 1.4s ease-in-out infinite alternate}
@keyframes gglow{0%{opacity:.45;transform:scale(.9)}100%{opacity:.9;transform:scale(1.08)}}
.kintro{position:absolute;inset:0;z-index:5;background:rgba(10,10,20,.85)}
.kintro img{position:absolute;left:910px;top:380px;width:100px;height:280px;object-fit:contain;filter:drop-shadow(0 0 30px var(--kc));animation:kin .5s ease-out forwards}
@keyframes kin{0%{transform:translateY(-120px) scale(.5) rotate(-25deg);opacity:0}45%{transform:translateY(10px) scale(1.15) rotate(8deg);opacity:1}70%{transform:scale(1) rotate(-6deg)}85%{transform:rotate(4deg)}100%{transform:scale(1.25);filter:brightness(2.2) drop-shadow(0 0 60px var(--kc))}}
.rbtn{position:absolute;top:863px;width:262px;height:82px;border:3px solid;font-size:34px;font-family:Cinzel,serif;font-weight:700}
`);

const STAT_NAME: Record<string, string> = { maxHp: 'MaxHp', atk: 'Attack', def: 'Defense', crit: 'Crit', critMult: 'Crit Dmg', lifesteal: 'Lifesteal', thorns: 'Thorns', thornsCap: 'Thorns cap', execute: 'Execute', defToAtk: 'Def→Atk' };
const PCT = new Set(['crit', 'lifesteal', 'thorns', 'thornsCap', 'execute', 'defToAtk']);

export function modText(m: Mod) {
  const v = PCT.has(m.stat) ? `${Math.round(m.v * 100)}%` : m.stat === 'critMult' ? m.v.toFixed(1) : String(m.v);
  const sign = m.op === 'set' ? '' : m.v >= 0 ? '+' : '';
  return `${sign}${m.op === 'mul' ? `${Math.round(m.v * 100)}%` : v} ${STAT_NAME[m.stat]}`;
}

/** Slot frame top-left and label center. */
const SLOT_POS: Record<Slot, { x: number; y: number }> = {
  head: { x: 68, y: 229 }, body: { x: 68, y: 405 }, arms: { x: 68, y: 581 }, legs: { x: 68, y: 757 },
  weapon: { x: 929, y: 265 }, secondary: { x: 929, y: 485 }, accessory: { x: 929, y: 705 },
};
const TABS: [string, (s: Slot) => boolean][] = [
  ['ALL', () => true],
  ['WEAP', (s) => s === 'weapon'],
  ['ARMR', (s) => ['head', 'body', 'arms', 'legs', 'secondary'].includes(s)],
  ['ACCE', (s) => s === 'accessory'],
  ['MISC', () => false],
];
const KEY_COLOR: Record<KeyType, string> = { copper: '#e0884a', silver: '#d8dde4', gold: '#f2c23c' };

export interface InventoryOptions { mode: 'menu' | 'run'; onChange(): void }

export class InventoryView {
  private tab = 0;
  private root: HTMLElement;
  private found: { text: string; color: string } | null = null;

  constructor(parent: HTMLElement, private meta: MetaState, private opts: InventoryOptions) {
    this.root = el('div', 'inv', '', parent);
    this.render();
  }

  private changed() { this.opts.onChange(); this.render(); }

  private icon(parent: HTMLElement, it: ItemInst, cls = 'ico') {
    const d = def(it);
    if (d.icon && assetUrl(d.icon)) {
      const i = el('img', cls, '', parent);
      i.src = assetUrl(d.icon);
      return i;
    }
    const p = el('div', 'ph', d.name.toUpperCase(), parent);
    p.style.setProperty('--rc', rarityColor(d.rarity));
    return p;
  }

  render() {
    const m = this.meta;
    this.root.innerHTML = '';
    // equipment
    for (const s of SLOTS) {
      const { x, y } = SLOT_POS[s];
      const box = place(el('div', 'slot', '', this.root), x, y);
      box.dataset.id = `slot_${s}`;
        const it = m.items.find((i) => i.uid === m.equipped[s]);
      if (it) {
        box.classList.add('full');
        box.style.setProperty('--rc', rarityColor(def(it).rarity));
        this.icon(box, it);
        img(box, 'icon_checkmark', 108, 4, 30, 28, 'chk');
        box.addEventListener('click', () => { unequip(m, it); bus.emit('unequip', { item: it.id }); this.changed(); });
      } else {
        box.classList.add('empty');
        box.style.backgroundImage = `url(${assetUrl(`slot_${s}`)}),url(${assetUrl('slot_frame')})`;
      }
      place(el('div', 'lbl', s.toUpperCase(), this.root), x + 72 - 100, y + 146);
    }
    // tabs + stash
    TABS.forEach(([name], i) => {
      const b = button(this.root, name, () => { this.tab = i; this.render(); }, `tab${i === this.tab ? ' on' : ''}`, `tab_${name}`);
      b.style.left = `${1085 + i * 152}px`;
    });
    const panel = el('div', 'panel', '', this.root);
    const list = m.items.filter((i) => TABS[this.tab][1](def(i).slot));
    list.forEach((it, n) => {
      const c = place(el('div', 'cell', '', panel), 44 + (n % 5) * 136, 41 + Math.floor(n / 5) * 136);
      c.dataset.id = `item${n}`;
      c.style.setProperty('--rc', rarityColor(def(it).rarity));
      this.icon(c, it);
      if (isEquipped(m, it)) img(c, 'icon_checkmark', 78, 2, 30, 28, 'chk');
      if (it.level) el('div', 'lvl', `+${it.level}`, c);
      c.title = `${def(it).name} (${def(it).rarity}) ${itemMods(it).map(modText).join(' ')}`;
      c.addEventListener('click', () => this.popup(it));
    });
    if (!list.length) {
      const e = place(el('div', 'abs', t('inv.empty'), panel), 0, 330, 744);
      Object.assign(e.style, { textAlign: 'center', fontSize: '28px', color: '#4e5e56', fontWeight: '700' });
    }
    const sh = place(el('div', 'abs', t('inv.shards', { n: m.shards, max: DATA.balance.shardsPerKey }), panel), 0, 616, 744);
    Object.assign(sh.style, { textAlign: 'center', fontSize: '24px', color: '#e0b860', fontWeight: '700' });
    // keys
    KEY_TYPES.forEach((k, i) => {
      const X = i * 372;
      img(this.root, `key_${k}`, 18 + X, 988, 43, 70);
      const cnt = place(el('div', 'abs', String(m.keys[k]), this.root), 92 + X, 998, 60);
      Object.assign(cnt.style, { textAlign: 'center', fontSize: '40px', fontWeight: '700', color: KEY_COLOR[k] });
      const open = button(this.root, '', () => this.open(k), 'kbtn', `open_${k}`);
      place(open, 152 + X, 995, 118);
      open.disabled = m.keys[k] <= 0;
      open.style.backgroundImage = `url(${assetUrl(open.disabled ? 'btn_open_disabled' : `btn_open_${k}`)})`;
      if (k !== 'gold') {
        const cv = button(this.root, '', () => { if (convertKeys(m, k)) this.changed(); }, 'kbtn cv', `conv_${k}`);
        place(cv, 284 + X, 995, 72);
        cv.style.backgroundImage = `url(${assetUrl(k === 'copper' ? 'btn_3to1_silver' : 'btn_3to1_gold')})`;
        cv.disabled = m.keys[k] < 3;
      }
    });
    if (this.found) {
      const f = place(el('div', 'abs', this.found.text, this.root), 140, 940, 800);
      Object.assign(f.style, { textAlign: 'center', fontSize: '30px', fontWeight: '700', color: this.found.color, textShadow: '0 2px 4px #000' });
    }
  }

  private setFound(it: ItemInst) {
    const d = def(it);
    this.found = { text: t('inv.found', { name: d.name, r: t(`rarity.${d.rarity}`) }).toUpperCase(), color: d.rarity === 'common' ? '#e8e0d0' : rarityColor(d.rarity) };
  }

  private vsText(it: ItemInst): { text: string; up: boolean } | null {
    const m = this.meta;
    const cur = m.items.find((i) => i.uid === m.equipped[def(it).slot]);
    if (!cur) return { text: t('inv.pureGain'), up: true };
    if (cur === it) return null;
    const a = new Map(itemMods(cur).map((x) => [x.stat, x.v]));
    const b = new Map(itemMods(it).map((x) => [x.stat, x.v]));
    const parts = [...new Set([...a.keys(), ...b.keys()])].map((s) => {
      const d = (b.get(s) || 0) - (a.get(s) || 0);
      const v = PCT.has(s) ? `${Math.round(Math.abs(d) * 100)}%` : String(Math.abs(+d.toFixed(2)));
      return d > 0 ? `${STAT_NAME[s]} ▲ +${v}` : d < 0 ? `${STAT_NAME[s]} ▼ -${v}` : `${STAT_NAME[s]} =`;
    });
    return { text: `${t('inv.vs')} ${parts.join(' · ')}`, up: itemScore(it) >= itemScore(cur) };
  }

  popup(it: ItemInst) {
    const m = this.meta;
    const d = def(it);
    const rc = rarityColor(d.rarity);
    const mo = el('div', 'imodal', '', this.root);
    const close = () => mo.remove();
    const p = el('div', 'pan', '', mo);
    el('div', 'bar', '', p).style.background = rc;
    const iconBox = place(el('div', 'abs', '', p), 0, 60, 904, 140);
    Object.assign(iconBox.style, { display: 'flex', justifyContent: 'center', alignItems: 'center' });
    const ic = this.icon(iconBox, it);
    if (ic instanceof HTMLImageElement) ic.style.maxHeight = '130px';
    const L = (html: string, cy: number, size: number, color: string) => line(p, html, cy, size, color);
    L(`${d.name.toUpperCase()}${it.level ? ` +${it.level}` : ''}`, 236, 40, d.rarity === 'common' ? '#ece4d4' : rc).style.fontWeight = '700';
    L(`${t(`rarity.${d.rarity}`)} · ${d.slot.toUpperCase()}`, 276, 26, '#a8a090').style.fontWeight = '400';
    L(itemMods(it).map(modText).join('  ').toUpperCase(), 321, 28, '#ece4d4').style.fontWeight = '400';
    const vs = this.vsText(it);
    if (vs) L(vs.text.toUpperCase(), 362, 22, vs.up ? '#5fe36a' : '#ff6a5a');
    const n = DATA.balance.mergeCount;
    L(t('inv.salvageInfo', { v: salvageValue(it), k: DATA.balance.shardsPerKey }).toUpperCase(), 403, 22, '#e8a090').style.fontWeight = '400';
    L(t('inv.mergeInfo', { c: copies(m, it).length, n, cost: upgradeCost(it) }).toUpperCase(), 444, 22, '#a8a090').style.fontWeight = '400';
    const mb = (label: string, i: number, border: string, bg: string, color: string, fn: () => void, id: string) => {
      const b = button(p, label, fn, 'box mbtn', id);
      Object.assign(b.style, { left: `${16 + i * 176}px`, borderColor: border, background: bg, color });
      return b;
    };
    const eq = mb(isEquipped(m, it) ? t('inv.equipped') : t('inv.equip'), 0, '#3aa64a', '#143020', '#e8f0d0', () => { if (!isEquipped(m, it)) bus.emit('equip', { item: it.id }); equip(m, it); close(); this.changed(); }, 'p_equip');
    eq.disabled = isEquipped(m, it);
    eq.style.opacity = '1';
    const mg = mb(`${t('inv.merge')} ${copies(m, it).length}/${n}`, 1, '#3a70b0', '#122030', '#d8e4f0', () => {
      const out = merge(m, it);
      close();
      if (out) { this.setFound(out); this.changed(); this.reward(out, true); }
    }, 'p_merge');
    mg.disabled = copies(m, it).length < n || d.rarity === 'legendary';
    const up = mb(`◆ ${upgradeCost(it)}`, 2, '#a08a30', '#262010', '#f0d080', () => { if (upgrade(m, it)) { close(); this.changed(); this.popup(it); floatText(this.root, `+${it.level}`, 959, 330, '#ffd23a'); burst(959, 366, { count: 18, color: '#ffd23a', shape: 'spark', speed: [200, 500], life: [0.3, 0.6], size: [14, 26], drag: 4 }); } }, 'p_upgrade');
    up.disabled = m.shards < upgradeCost(it);
    mb(t('inv.salvage'), 3, '#c04040', '#301414', '#f08a8a', () => {
      const v = salvage(m, it);
      close();
      this.changed();
      floatText(this.root, `◆ +${v}`, 1457, 850, '#e0b860');
      burst(1457, 879, { count: 14, color: '#e0b860', shape: 'spark', speed: [150, 400], life: [0.3, 0.6], size: [12, 22], gravity: 300, drag: 3 });
    }, 'p_salvage');
    mb(t('inv.cancel'), 4, '#6a6aa0', '#1a1830', '#e0e0f0', close, 'p_cancel');
  }

  /** Key: the screen dims, the key drops in, wobbles and flares (0.5 s, as in the original), then the reward. */
  private open(k: KeyType) {
    const r = openChest(this.meta, k);
    if (!r) return;
    this.setFound(r.item);
    this.changed();
    const ki = el('div', 'kintro', '', this.root);
    ki.style.setProperty('--kc', KEY_COLOR[k]);
    el('img', '', '', ki).setAttribute('src', assetUrl(`key_${k}`));
    setTimeout(() => { ki.remove(); this.reward(r.item); }, 500);
  }

  /** Chest / merge result: flash in the rarity color, the item, Equip / Keep. */
  reward(it: ItemInst, merged = false) {
    const d = def(it);
    const rc = rarityColor(d.rarity);
    const mo = el('div', 'rmodal', '', this.root);
    mo.style.setProperty('--rc', rc);
    mo.style.setProperty('--fk', String(game.flashK));
    if (merged) el('div', 'glow', '', mo); // GDD 8.5: golden glow + sparks on merge
    // uncommon and up: a full-screen peak in the rarity color first (original), then the radial flash
    if (d.rarity !== 'common') el('div', 'flash full', '', mo);
    el('div', 'flash', '', mo);
    sparkRing(960, 510, rc);
    if (merged) goldSparks(960, 510);
    const close = () => mo.remove();
    line(mo, `${t(`rarity.${d.rarity}`)} · ${d.slot.toUpperCase()}`, 300, 34, '#ece4d4').style.fontWeight = '700';
    const iconBox = place(el('div', 'abs', '', mo), 0, 390, 1920, 240);
    Object.assign(iconBox.style, { display: 'flex', justifyContent: 'center', alignItems: 'center' });
    const ic = this.icon(iconBox, it);
    if (ic instanceof HTMLImageElement) { ic.style.height = '240px'; ic.style.maxWidth = '300px'; ic.style.objectFit = 'contain'; } else ic.style.fontSize = '40px';
    line(mo, d.name.toUpperCase(), 703, 52, d.rarity === 'common' ? '#ece4d4' : rc).style.fontWeight = '700';
    line(mo, itemMods(it).map(modText).join('  ').toUpperCase(), 746, 30, '#ece4d4').style.fontWeight = '400';
    const vs = isEquipped(this.meta, it) ? { text: t('inv.pureGain'), up: true } : this.vsText(it);
    if (vs) line(mo, vs.text.toUpperCase(), 788, 24, vs.up ? '#5fe36a' : '#ff6a5a').style.fontWeight = '700';
    const eq = button(mo, isEquipped(this.meta, it) ? t('inv.equipped') : t('inv.equip'), () => { if (!isEquipped(this.meta, it)) bus.emit('equip', { item: it.id }); equip(this.meta, it); close(); this.changed(); }, 'box rbtn', 'r_equip');
    Object.assign(eq.style, { left: '669px', borderColor: '#3ab870', background: '#143020', color: '#e8f0d0' });
    eq.disabled = isEquipped(this.meta, it);
    eq.style.opacity = '1';
    const keep = button(mo, t('inv.keep'), close, 'box rbtn', 'r_keep');
    Object.assign(keep.style, { left: '989px', borderColor: '#c8963c', background: '#2a2048', color: '#ece4d4' });
  }
}
