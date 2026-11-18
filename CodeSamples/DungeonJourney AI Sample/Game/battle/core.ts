// Pure battle core: no timers, no rendering. Simulates a whole floor (foes one after another, hero HP carries over)
// and returns an event log that the presenter plays back. Same inputs + seed -> same log.
import { Rng } from '../core/rng';
import { DATA } from '../data';
import type { Effect, FoeSpec, Hook, Stats } from '../data/types';
import { computeStats, effectiveAtk } from './stats';

export type Side = 'hero' | 'foe';

export type LogEntry =
  | { t: 'foe'; i: number; hp: number; max: number }
  | { t: 'start'; heroFirst: boolean }
  | { t: 'hit'; from: Side; dmg: number; crit: boolean; exec: boolean; hp: number }
  | { t: 'thorns'; from: Side; dmg: number; hp: number }
  | { t: 'heal'; side: Side; amount: number; hp: number }
  | { t: 'foe_death'; i: number }
  | { t: 'hero_death' }
  | { t: 'clear' };

export interface Fighter {
  side: Side;
  stats: Stats;
  hp: number;
  effects: Effect[];
  hits: number;
  lowFired: boolean;
}

export interface FloorInput {
  hero: { stats: Stats; hp: number; effects: Effect[] };
  foes: FoeSpec[];
  seed: string;
  /** After Revive / Continue the first fight starts with the hero. */
  heroFirst?: boolean;
  god?: boolean;
}

/** atkGained: permanent ATK from kills (Trophy, Warlord, Reaper), kept for the rest of the run. */
export interface FloorResult { cleared: boolean; heroHp: number; log: LogEntry[]; atkGained: number }

const LOW_HP = 0.3;
const MAX_HITS = 400;

export function foeFighter(f: FoeSpec): Fighter {
  const b = DATA.balance.hero;
  const mods = f.abilities.flatMap((a) => DATA.abilities.get(a)?.mods || []).filter((m) => m.stat !== 'maxHp'); // HP already scaled by the generator
  const base: Stats = { ...b, maxHp: f.hp, atk: f.atk, def: f.def, crit: 0, critMult: 2, lifesteal: 0, thorns: 0, execute: 0, defToAtk: 0, hpToAtk: 0, atkPerKill: 0 };
  const stats = computeStats(base, mods);
  stats.maxHp = f.hp;
  return { side: 'foe', stats, hp: f.hp, effects: f.abilities.flatMap((a) => DATA.abilities.get(a)?.effects || []), hits: 0, lowFired: false };
}

interface Ctx { self: Fighter; other: Fighter; dmg: number; bonus: number; heroFirst?: boolean }

export function simulateFloor(input: FloorInput): FloorResult {
  const rng = new Rng(input.seed);
  const log: LogEntry[] = [];
  const { defK, minDmg, maxBlock } = DATA.balance.damage;
  const hero: Fighter = { side: 'hero', stats: { ...input.hero.stats }, hp: input.hero.hp, effects: input.hero.effects, hits: 0, lowFired: false };
  let atkGained = 0;

  const heal = (f: Fighter, amount: number) => {
    const a = Math.min(Math.round(amount), f.stats.maxHp - f.hp);
    if (a <= 0) return;
    f.hp += a;
    log.push({ t: 'heal', side: f.side, amount: a, hp: f.hp });
  };

  const run = (hook: Hook, ctx: Ctx) => {
    for (const e of ctx.self.effects) {
      if (e.on !== hook) continue;
      const c = e.if;
      if (c) {
        if (c.selfHpBelow !== undefined && ctx.self.hp / ctx.self.stats.maxHp >= c.selfHpBelow) continue;
        if (c.targetHpBelow !== undefined && ctx.other.hp / ctx.other.stats.maxHp >= c.targetHpBelow) continue;
        if (c.firstTurn && ctx.self.hits > 0) continue;
        if (c.chance !== undefined && !rng.chance(c.chance)) continue;
      }
      switch (e.do) {
        case 'heal_dmg': heal(ctx.self, ctx.dmg * e.k); break;
        case 'heal_missing': heal(ctx.self, (ctx.self.stats.maxHp - ctx.self.hp) * e.k); break;
        case 'heal_max': heal(ctx.self, ctx.self.stats.maxHp * e.k); break;
        case 'bonus_dmg': ctx.bonus += e.k; break;
        case 'strike_first': ctx.heroFirst = ctx.self.side === 'hero'; break;
      }
    }
  };

  const checkLow = (f: Fighter, other: Fighter) => {
    if (!f.lowFired && f.hp > 0 && f.hp / f.stats.maxHp < LOW_HP) { f.lowFired = true; run('on_low_hp', { self: f, other, dmg: 0, bonus: 0 }); }
  };

  const damage = (target: Fighter, dmg: number) => {
    target.hp = Math.max(0, target.hp - dmg);
    if (input.god && target.side === 'hero') target.hp = Math.max(1, target.hp);
  };

  /** One attack; returns true if someone died. */
  const attack = (a: Fighter, d: Fighter): boolean => {
    const ctx: Ctx = { self: a, other: d, dmg: 0, bonus: 0 };
    run('on_before_hit', ctx);
    const crit = rng.chance(a.stats.crit);
    const raw = effectiveAtk(a.stats) * (crit ? a.stats.critMult : 1) * (1 + ctx.bonus);
    const block = d.stats.def > 0 ? Math.min(maxBlock, d.stats.def / (d.stats.def + defK)) : 0;
    let dmg = Math.max(minDmg, Math.round(raw * (1 - block)));
    damage(d, dmg);
    let exec = false;
    if (d.hp > 0 && a.stats.execute > 0 && d.hp / d.stats.maxHp < a.stats.execute) { dmg += d.hp; d.hp = 0; exec = true; }
    a.hits++;
    log.push({ t: 'hit', from: a.side, dmg, crit, exec, hp: d.hp });
    ctx.dmg = dmg;
    if (a.stats.lifesteal > 0) heal(a, dmg * a.stats.lifesteal);
    run('on_hit', ctx);
    if (crit) run('on_crit', ctx);
    if (d.hp > 0) {
      run('on_damaged', { self: d, other: a, dmg, bonus: 0 });
      if (d.stats.thorns > 0 && a.hp > 0) {
        const th = Math.max(1, Math.round(Math.min(dmg * d.stats.thorns, d.stats.thornsCap * a.stats.maxHp)));
        damage(a, th);
        log.push({ t: 'thorns', from: d.side, dmg: th, hp: a.hp });
      }
      checkLow(d, a);
      checkLow(a, d);
    }
    if (d.hp <= 0) {
      if (a.stats.atkPerKill > 0) { a.stats.atk += a.stats.atkPerKill; if (a === hero) atkGained += a.stats.atkPerKill; }
      run('on_kill', ctx);
      return true;
    }
    return a.hp <= 0;
  };

  // who strikes first: one roll per floor (hero / foe / alternate); after Revive / Continue the hero strikes first on the whole floor
  const fs = DATA.balance.firstStrike;
  const roll = rng.next() * (fs.hero + fs.foe + fs.alternate);
  const mode = input.heroFirst ? 'hero' : roll < fs.hero ? 'hero' : roll < fs.hero + fs.foe ? 'foe' : 'alternate';
  let altHero = rng.chance(0.5);
  for (let i = 0; i < input.foes.length; i++) {
    const foe = foeFighter(input.foes[i]);
    log.push({ t: 'foe', i, hp: foe.hp, max: foe.stats.maxHp });
    hero.hits = 0;
    hero.lowFired = hero.hp / hero.stats.maxHp < LOW_HP;
    const start: Ctx = { self: hero, other: foe, dmg: 0, bonus: 0, heroFirst: mode === 'alternate' ? altHero : mode === 'hero' };
    altHero = !altHero;
    run('on_battle_start', start);
    const foeStart: Ctx = { self: foe, other: hero, dmg: 0, bonus: 0, heroFirst: start.heroFirst };
    run('on_battle_start', foeStart);
    const heroFirst = start.heroFirst! && foeStart.heroFirst !== false;
    log.push({ t: 'start', heroFirst });

    let turn: Side = heroFirst ? 'hero' : 'foe';
    for (let n = 0; n < MAX_HITS; n++) {
      const over = turn === 'hero' ? attack(hero, foe) : attack(foe, hero);
      if (over) break;
      turn = turn === 'hero' ? 'foe' : 'hero';
    }
    if (hero.hp <= 0) { log.push({ t: 'hero_death' }); return { cleared: false, heroHp: 0, log, atkGained }; }
    if (foe.hp > 0) { log.push({ t: 'hero_death' }); return { cleared: false, heroHp: 0, log, atkGained }; } // stalemate safeguard
    log.push({ t: 'foe_death', i });
  }
  log.push({ t: 'clear' });
  return { cleared: true, heroHp: hero.hp, log, atkGained };
}
