// Meta progression between runs: items, equipment, keys, shards, chests, merge / upgrade / salvage.
import type { SourcedMod } from './battle/stats';
import { DATA } from './data';
import { KEY_TYPES, RARITIES, type ItemDef, type KeyType, type Mod, type Rarity, type Slot } from './data/types';
import { bus } from './events';

export interface ItemInst { uid: number; id: string; level: number }

export interface Settings { music: number; sfx: number; mute: boolean; shake: boolean; fastReplay: boolean }

export interface MetaState {
  best: number;
  keys: Record<KeyType, number>;
  shards: number;
  items: ItemInst[];
  equipped: Partial<Record<Slot, number>>;
  nextUid: number;
  settings: Settings;
}

export function newMeta(): MetaState {
  return {
    best: 0, keys: { copper: 0, silver: 0, gold: 0 }, shards: 0, items: [], equipped: {}, nextUid: 1,
    settings: { music: 0.7, sfx: 0.8, mute: false, shake: true, fastReplay: true },
  };
}

export const def = (it: ItemInst): ItemDef => DATA.items.get(it.id)!;

/** Item mods with upgrade level: +10% per level, rounded like the stats they touch. */
export function itemMods(it: ItemInst): Mod[] {
  const k = 1 + it.level * DATA.balance.upgradeStep;
  return def(it).mods.map((m) => {
    const v = m.v * k;
    return { ...m, v: m.op === 'add' && (m.stat === 'atk' || m.stat === 'def' || m.stat === 'maxHp') ? Math.round(v) : +v.toFixed(3) };
  });
}

export function equippedItems(meta: MetaState): ItemInst[] {
  return Object.values(meta.equipped).map((uid) => meta.items.find((i) => i.uid === uid)).filter((i): i is ItemInst => !!i);
}

export function equippedMods(meta: MetaState): SourcedMod[] {
  return equippedItems(meta).flatMap((it) => itemMods(it).map((m) => ({ ...m, source: `item:${it.id}` })));
}

export function isEquipped(meta: MetaState, it: ItemInst) {
  return meta.equipped[def(it).slot] === it.uid;
}

export function equip(meta: MetaState, it: ItemInst) {
  meta.equipped[def(it).slot] = it.uid;
}

/** Takes the item off its slot (it stays in the stash). */
export function unequip(meta: MetaState, it: ItemInst) {
  if (isEquipped(meta, it)) delete meta.equipped[def(it).slot];
}

/** Rough power for "vs equipped" and auto-equip. */
export function itemScore(it: ItemInst) {
  const w: Record<string, number> = { atk: 1, def: 0.7, maxHp: 0.25, crit: 60, critMult: 20, lifesteal: 50, thorns: 15, execute: 80, defToAtk: 30, thornsCap: 10 };
  return itemMods(it).reduce((s, m) => s + m.v * (w[m.stat] || 1), 0);
}

export function addItem(meta: MetaState, id: string, level = 0): ItemInst {
  const it = { uid: meta.nextUid++, id, level };
  meta.items.push(it);
  return it;
}

function itemsOf(rarity: Rarity, slot?: Slot) {
  return [...DATA.items.values()].filter((d) => d.rarity === rarity && (!slot || d.slot === slot));
}

/** Rarity roll by the chest table; falls back to the nearest rarity that has items. */
function pickItem(rarity: Rarity, slot?: Slot): ItemDef | null {
  const order = [rarity, ...RARITIES.filter((r) => r !== rarity).sort((a, b) => Math.abs(RARITIES.indexOf(a) - RARITIES.indexOf(rarity)) - Math.abs(RARITIES.indexOf(b) - RARITIES.indexOf(rarity)))];
  for (const r of order) {
    const pool = itemsOf(r, slot);
    if (pool.length) return pool[Math.floor(Math.random() * pool.length)];
  }
  return slot ? pickItem(rarity) : null;
}

function rollRarity(weights: Partial<Record<Rarity, number>>): Rarity {
  const e = Object.entries(weights) as [Rarity, number][];
  let x = Math.random() * e.reduce((s, [, w]) => s + w, 0);
  for (const [r, w] of e) if ((x -= w) < 0) return r;
  return e[e.length - 1][0];
}

/** Opens a chest with one key. Empty slot -> the item is equipped right away. */
export function openChest(meta: MetaState, key: KeyType): { item: ItemInst; prev: ItemInst | null } | null {
  if (meta.keys[key] <= 0) return null;
  const d = pickItem(rollRarity(DATA.balance.chests[key]));
  if (!d) return null;
  meta.keys[key]--;
  const item = addItem(meta, d.id);
  const prevUid = meta.equipped[d.slot];
  const prev = meta.items.find((i) => i.uid === prevUid) || null;
  if (!prev) equip(meta, item);
  bus.emit('chest_open', { key, rarity: d.rarity });
  return { item, prev };
}

/** 3 keys -> 1 key of the next type (none for gold). */
export function convertKeys(meta: MetaState, key: KeyType): boolean {
  const i = KEY_TYPES.indexOf(key);
  if (i >= KEY_TYPES.length - 1 || meta.keys[key] < 3) return false;
  meta.keys[key] -= 3;
  meta.keys[KEY_TYPES[i + 1]]++;
  return true;
}

export function copies(meta: MetaState, it: ItemInst) {
  return meta.items.filter((i) => i.id === it.id);
}

/** 3 identical items -> 1 random item of the next rarity, same slot. Keeps the equipped one if possible. */
export function merge(meta: MetaState, it: ItemInst): ItemInst | null {
  const d = def(it);
  const n = DATA.balance.mergeCount;
  const same = copies(meta, it);
  const ri = RARITIES.indexOf(d.rarity);
  if (same.length < n || ri >= RARITIES.length - 1) return null;
  const pool = itemsOf(RARITIES[ri + 1], d.slot);
  const any = pool.length ? pool : itemsOf(RARITIES[ri + 1]); // no item of that rarity for the slot: any slot
  if (!any.length) return null;
  const next = any[Math.floor(Math.random() * any.length)];
  const used = [it, ...same.filter((i) => i !== it)].slice(0, n);
  const wasEquipped = used.some((i) => isEquipped(meta, i));
  meta.items = meta.items.filter((i) => !used.includes(i));
  if (wasEquipped) delete meta.equipped[d.slot];
  const out = addItem(meta, next.id);
  if (wasEquipped || meta.equipped[next.slot] === undefined) equip(meta, out);
  bus.emit('merge', { item: next.id, rarity: next.rarity });
  return out;
}

export function upgradeCost(it: ItemInst) {
  return DATA.balance.rarities[def(it).rarity].upgradeCost * (it.level + 1);
}

export function upgrade(meta: MetaState, it: ItemInst): boolean {
  const c = upgradeCost(it);
  if (meta.shards < c) return false;
  meta.shards -= c;
  it.level++;
  bus.emit('upgrade', { item: it.id, level: it.level });
  return true;
}

export function salvageValue(it: ItemInst) {
  return DATA.balance.rarities[def(it).rarity].salvage + it.level;
}

/** Item -> shards; every 10 shards turn into a copper key. */
export function salvage(meta: MetaState, it: ItemInst): number {
  const v = salvageValue(it);
  if (isEquipped(meta, it)) delete meta.equipped[def(it).slot];
  meta.items = meta.items.filter((i) => i !== it);
  meta.shards += v;
  while (meta.shards >= DATA.balance.shardsPerKey) { meta.shards -= DATA.balance.shardsPerKey; meta.keys.copper++; }
  bus.emit('salvage', { item: it.id, shards: v });
  return v;
}
