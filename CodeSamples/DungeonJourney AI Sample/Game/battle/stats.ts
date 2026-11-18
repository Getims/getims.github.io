// Stats = base + modifiers. add sums, mul sums into ×(1+Σ), set replaces (last one wins: REPLACES cards).
import { STATS } from '../data/types';
import type { Mod, Stat, Stats } from '../data/types';

export interface SourcedMod extends Mod { source: string }

const ROUNDED: Stat[] = ['maxHp', 'atk', 'def'];

export function computeStats(base: Stats, mods: readonly Mod[]): Stats {
  const add = {} as Record<Stat, number>;
  const mul = {} as Record<Stat, number>;
  const set = {} as Partial<Record<Stat, number>>;
  for (const s of STATS) { add[s] = 0; mul[s] = 0; }
  for (const m of mods) {
    if (m.op === 'add') add[m.stat] += m.v;
    else if (m.op === 'mul') mul[m.stat] += m.v;
    else set[m.stat] = m.v;
  }
  const out = {} as Stats;
  for (const s of STATS) {
    let v = set[s] ?? (base[s] + add[s]) * (1 + mul[s]);
    if (ROUNDED.includes(s)) v = Math.max(s === 'maxHp' ? 1 : 0, Math.round(v));
    out[s] = v;
  }
  return out;
}

/** Attack actually used in a hit: ATK + defToAtk·DEF (Stoneblood, Juggernaut) + hpToAtk·Max HP (Bloodforge). */
export function effectiveAtk(s: Stats) {
  return s.atk + Math.round(s.defToAtk * s.def) + Math.round(s.hpToAtk * s.maxHp);
}

export function hasReplace(mods: readonly Mod[] | undefined) {
  return !!mods?.some((m) => m.op === 'set');
}
