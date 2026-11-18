// Config validator: runs at boot; errors go to the console (the autotest fails on them) and to the debug panel.
import { EFFECTS, HOOKS, KEY_TYPES, RARITIES, SLOTS, STATS } from './types';
import type { AbilityDef, BalanceConfig, EnemyDef, Effect, FloorsFile, ItemDef, Mod, PerkDef } from './types';

export interface DataSet { balance: BalanceConfig; perks: PerkDef[]; enemies: EnemyDef[]; abilities: AbilityDef[]; items: ItemDef[]; floors: FloorsFile }

export function validate(d: DataSet, assets: Record<string, string> | null): string[] {
  const err: string[] = [];
  const has = (list: readonly string[], v: string) => list.includes(v);

  const mods = (where: string, list?: Mod[]) => list?.forEach((m, i) => {
    if (!has(STATS, m.stat)) err.push(`${where}.mods[${i}]: unknown stat "${m.stat}"`);
    if (!has(['add', 'mul', 'set'], m.op)) err.push(`${where}.mods[${i}]: unknown op "${m.op}"`);
    if (typeof m.v !== 'number' || !isFinite(m.v)) err.push(`${where}.mods[${i}]: v is not a number`);
  });
  const effects = (where: string, list?: Effect[]) => list?.forEach((e, i) => {
    if (!has(HOOKS, e.on)) err.push(`${where}.effects[${i}]: unknown hook "${e.on}"`);
    if (!has(EFFECTS, e.do)) err.push(`${where}.effects[${i}]: unknown effect "${e.do}"`);
    if (typeof e.k !== 'number') err.push(`${where}.effects[${i}]: k is not a number`);
  });
  const unique = (kind: string, list: { id: string }[]) => {
    const seen = new Set<string>();
    for (const x of list) { if (seen.has(x.id)) err.push(`${kind}: duplicate id "${x.id}"`); seen.add(x.id); }
    return seen;
  };

  const b = d.balance;
  for (const s of STATS) if (typeof b.hero[s] !== 'number') err.push(`balance.hero.${s} missing`);
  for (const r of RARITIES) if (!b.rarities[r]) err.push(`balance.rarities.${r} missing`);
  for (const [name, t] of Object.entries(b.keyDrops)) {
    if (!(t.chance >= 0 && t.chance <= 1)) err.push(`balance.keyDrops.${name}: chance out of 0..1`);
    for (const k of Object.keys(t.weights)) if (!has(KEY_TYPES, k)) err.push(`balance.keyDrops.${name}: unknown key "${k}"`);
    if (!Object.values(t.weights).some((w) => (w || 0) > 0)) err.push(`balance.keyDrops.${name}: no positive weights`);
  }
  for (const k of KEY_TYPES) {
    const c = b.chests[k];
    if (!c) { err.push(`balance.chests.${k} missing`); continue; }
    for (const r of Object.keys(c)) if (!has(RARITIES, r)) err.push(`balance.chests.${k}: unknown rarity "${r}"`);
  }

  unique('perks', d.perks);
  for (const p of d.perks) {
    if (!has(RARITIES, p.rarity)) err.push(`perk ${p.id}: unknown rarity "${p.rarity}"`);
    if (!p.mods?.length && !p.effects?.length) err.push(`perk ${p.id}: no mods and no effects`);
    mods(`perk ${p.id}`, p.mods);
    effects(`perk ${p.id}`, p.effects);
  }

  const abilityIds = unique('abilities', d.abilities);
  for (const a of d.abilities) { mods(`ability ${a.id}`, a.mods); effects(`ability ${a.id}`, a.effects); }

  const enemyIds = unique('enemies', d.enemies);
  for (const e of d.enemies) {
    if (!(e.hpK > 0 && e.atkK > 0)) err.push(`enemy ${e.id}: hpK/atkK must be > 0`);
    if (e.minFloor > e.maxFloor) err.push(`enemy ${e.id}: minFloor > maxFloor`);
    if (assets && !assets[e.sprite]) err.push(`enemy ${e.id}: sprite "${e.sprite}" not in assets`);
  }
  if (!d.enemies.some((e) => e.boss)) err.push('enemies: no boss');

  unique('items', d.items);
  for (const it of d.items) {
    if (!has(SLOTS, it.slot)) err.push(`item ${it.id}: unknown slot "${it.slot}"`);
    if (!has(RARITIES, it.rarity)) err.push(`item ${it.id}: unknown rarity "${it.rarity}"`);
    if (assets && it.icon && !assets[it.icon]) err.push(`item ${it.id}: icon "${it.icon}" not in assets`);
    mods(`item ${it.id}`, it.mods);
  }

  const f = d.floors;
  if (f.version !== 1) err.push(`floors.version ${f.version}: expected 1`);
  const seenFloors = new Set<number>();
  for (const fl of f.floors) {
    const w = `floor ${fl.floor}`;
    if (!(fl.floor >= 1)) err.push(`${w}: floor must be >= 1`);
    if (seenFloors.has(fl.floor)) err.push(`${w}: duplicate`);
    seenFloors.add(fl.floor);
    if (!!fl.foes === !!fl.boss) err.push(`${w}: needs exactly one of foes / boss`);
    if (fl.keyDrop && !b.keyDrops[fl.keyDrop]) err.push(`${w}: unknown keyDrop "${fl.keyDrop}"`);
    for (const foe of [...(fl.foes || []), ...(fl.boss ? [fl.boss] : [])]) {
      if (foe.enemy && !enemyIds.has(foe.enemy)) err.push(`${w}: unknown enemy "${foe.enemy}"`);
      for (const ab of foe.abilities || []) if (!abilityIds.has(ab)) err.push(`${w}: unknown ability "${ab}"`);
    }
    if (fl.buff && !abilityIds.has(fl.buff)) err.push(`${w}: unknown buff "${fl.buff}"`);
  }
  for (const id of f.generator.floorBuff.pool) if (!abilityIds.has(id)) err.push(`floors.generator.floorBuff: unknown ability "${id}"`);
  for (const c of f.generator.abilityChances) if (!abilityIds.has(c.ability)) err.push(`floors.generator: unknown ability "${c.ability}"`);
  for (const k of ['normal', 'every5', 'boss']) if (!b.keyDrops[k]) err.push(`balance.keyDrops.${k} missing (used by the generator)`);
  return err;
}
