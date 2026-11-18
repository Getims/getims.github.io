// Run state (one descent): floor, perks, hero HP, death and continue rules.
import { computeStats, effectiveAtk, hasReplace, type SourcedMod } from './battle/stats';
import { Rng } from './core/rng';
import { DATA } from './data';
import type { Effect, KeyType, PerkDef, Stat, Stats } from './data/types';
import { game } from './game';
import { equippedMods } from './meta';

export interface RunState {
  seed: string;
  floor: number;
  perks: string[];
  hp: number;
  /** Bumped on every retry of a floor (revive) so the rolls differ. */
  attempt: number;
  /** Floors below this are a replay (Fast Replay ×3). */
  replayUntil: number;
  heroFirstNext: boolean;
  dead: boolean;
  /** Keys of the current attempt (reset on revive / continue): shown on You Died. */
  keysGained: Record<KeyType, number>;
  /** Permanent ATK from kills this run (Trophy, Warlord, Reaper); absent in older saves = 0. */
  killAtk?: number;
}

type StatsSrc = Pick<RunState, 'perks'> & { killAtk?: number };

export function newRun(): RunState {
  const run: RunState = {
    // floors up to the best floor of past runs replay fast (Fast Replay ×3), like after Continue
    seed: String(Date.now() % 1e9), floor: 1, perks: [], hp: 0, attempt: 0, replayUntil: game.meta.best,
    heroFirstNext: false, dead: false, keysGained: { copper: 0, silver: 0, gold: 0 },
  };
  run.hp = heroStats(run).maxHp;
  game.run = run;
  return run;
}

/** Equipped items; the class (stage 7) adds its mods here too. */
function metaMods(): SourcedMod[] {
  return equippedMods(game.meta);
}

export function heroMods(perks: readonly string[], killAtk = 0): SourcedMod[] {
  const out = metaMods();
  if (killAtk) out.push({ stat: 'atk', op: 'add', v: killAtk, source: 'kills' });
  for (const id of perks) for (const m of DATA.perks.get(id)?.mods || []) out.push({ ...m, source: id });
  return out;
}

export function heroStats(run: StatsSrc): Stats {
  return computeStats(DATA.balance.hero, heroMods(run.perks, run.killAtk));
}

export function heroEffects(run: Pick<RunState, 'perks'>): Effect[] {
  return run.perks.flatMap((id) => DATA.perks.get(id)?.effects || []);
}

/** Take a perk: stats change, on_pick effects (heals) apply now. */
export function pickPerk(run: RunState, perk: PerkDef) {
  run.perks.push(perk.id);
  const s = heroStats(run);
  run.hp = Math.min(run.hp, s.maxHp);
  for (const e of perk.effects || []) {
    if (e.on !== 'on_pick') continue;
    if (e.do === 'heal_missing') run.hp += Math.round((s.maxHp - run.hp) * e.k);
    if (e.do === 'heal_max') run.hp += Math.round(s.maxHp * e.k);
  }
  run.hp = Math.min(run.hp, s.maxHp);
}

/** Roll the keys of a cleared floor: one chance roll, then `count` keys on the weights ([] = no key). */
export function rollKeys(table: string, count: number, rng: Rng): KeyType[] {
  const t = DATA.balance.keyDrops[table];
  if (!t || !rng.chance(t.chance)) return [];
  return Array.from({ length: Math.max(1, count) }, () => rng.weighted(t.weights));
}

/** QUIT from settings during a run counts as a death: keys are already in the meta, next start = Continue (N−2, fast replay). */
export function abandonRun(run: RunState) {
  run.dead = true;
  run.hp = 0;
  game.meta.best = Math.max(game.meta.best, run.floor);
}

/** Revive (ad): same floor, full HP, perks kept. */
export function revive(run: RunState) {
  run.dead = false;
  run.keysGained = { copper: 0, silver: 0, gold: 0 };
  run.attempt++;
  run.hp = heroStats(run).maxHp;
  run.heroFirstNext = true;
  run.replayUntil = 0;
}

/** Continue from the menu: floor N−2, full HP, perks kept; skipped floors replay fast. */
export function continueRun(run: RunState) {
  const from = run.floor;
  run.dead = false;
  run.keysGained = { copper: 0, silver: 0, gold: 0 };
  run.attempt++;
  run.floor = Math.max(1, from - 2);
  run.replayUntil = from;
  run.hp = heroStats(run).maxHp;
  run.heroFirstNext = true;
}

/** One rule (customer): with Fast Replay on, floors below the best floor play ×3 — new run, Continue, Revive alike.
 *  replayUntil stays in RunState only for old saves. */
export function isReplay(run: RunState) {
  return run.floor < game.meta.best;
}

// ---- card preview: "ATK 12→20 · (STACKS)" ----

const pct = (v: number) => `${Math.round(v * 100)}%`;
const FMT: Partial<Record<Stat, (a: number, b: number, s: Stats) => string>> = {
  maxHp: (a, b) => `HP ${a}→${b}`,
  atk: (a, b) => `ATK ${a}→${b}`,
  def: (a, b) => `DEF ${a}→${b}`,
  crit: (a, b) => `CRIT ${Math.round(a * 100)}→${pct(b)}`,
  critMult: (a, b) => `CRIT DMG ×${a.toFixed(1)}→×${b.toFixed(1)}`,
  lifesteal: (a, b) => `LIFESTEAL ${Math.round(a * 100)}→${pct(b)}`,
  thorns: (_a, b, s) => `THORNS ${pct(b)} · MAX ${pct(s.thornsCap)} ENEMY HP`,
  execute: (_a, b) => `EXECUTE <${pct(b)}`,
  atkPerKill: (a, b) => `+${b - a} ATK PER KILL`,
};

export function perkPreview(run: RunState, perk: PerkDef): string {
  const before = heroStats(run);
  const after = heroStats({ perks: [...run.perks, perk.id], killAtk: run.killAtk });
  const parts: string[] = [];
  for (const k of Object.keys(FMT) as Stat[]) {
    if (Math.abs(after[k] - before[k]) > 1e-9) parts.push(FMT[k]!(before[k], after[k], after));
  }
  const fromDef = effectiveAtk(after) - after.atk - (effectiveAtk(before) - before.atk);
  if (fromDef > 0) parts.push(`+${fromDef} ATK FROM ${before.hpToAtk !== after.hpToAtk ? 'HP' : 'DEF'}`);
  for (const e of perk.effects || []) {
    if (e.on === 'on_pick' && e.do === 'heal_missing') parts.push(`HEALS ${Math.round((after.maxHp - Math.min(run.hp, after.maxHp)) * e.k)}`);
    if (e.on === 'on_crit' && e.do === 'heal_dmg') parts.push(`CRITS HEAL ${pct(e.k)}`);
    if (e.on === 'on_hit' && e.do === 'heal_dmg') parts.push(`HEALS ${pct(e.k)} OF DAMAGE`);
  }
  parts.push(hasReplace(perk.mods) ? '(REPLACES)' : '(STACKS)');
  return parts.join(' · ');
}
