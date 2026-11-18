// Floor generator: authored values from floors.json win, the rest comes from the curves (seeded, same for everyone).
import { Rng } from '../core/rng';
import type { AbilityDef, EnemyDef, Floor, FloorAuthor, FloorsFile, FoeAuthor, FoeSpec, NodeType } from './types';

export class FloorGen {
  private authored = new Map<number, FloorAuthor>();
  private cache = new Map<number, Floor>();
  private enemies: Map<string, EnemyDef>;
  private abilities: Map<string, AbilityDef>;

  constructor(private file: FloorsFile, enemies: EnemyDef[], abilities: AbilityDef[]) {
    for (const f of file.floors) this.authored.set(f.floor, f);
    this.enemies = new Map(enemies.map((e) => [e.id, e]));
    this.abilities = new Map(abilities.map((a) => [a.id, a]));
  }

  isBossFloor(n: number) {
    const a = this.authored.get(n);
    if (a) return !!a.boss;
    return n % this.file.generator.bossEvery === 0;
  }

  /** Floor n (1-based); floors past the authored list are generated endlessly. */
  floor(n: number): Floor {
    let f = this.cache.get(n);
    if (!f) this.cache.set(n, (f = this.build(n)));
    return f;
  }

  private build(n: number): Floor {
    const g = this.file.generator;
    const rng = new Rng(`${this.file.seed}:${n}`);
    const a = this.authored.get(n);
    const type: NodeType = this.isBossFloor(n) ? 'boss' : 'fight';
    const slots: FoeAuthor[] = type === 'boss' ? [a?.boss || {}] : a?.foes || Array.from({ length: g.foesPerFloor }, () => ({}));
    const foes = slots.map((s) => this.foe(n, s, type, rng));
    const keyDrop = a?.keyDrop || (type === 'boss' ? 'boss' : n % g.keyEvery === 0 ? 'every5' : 'normal');
    const keys = n < g.keyCount.from ? 1 : 2 + Math.floor((n - g.keyCount.from) / g.keyCount.every);
    // floor buff: own RNG stream so adding it did not reshuffle the foes of existing floors
    const fb = g.floorBuff, brng = new Rng(`${this.file.seed}:${n}:buff`);
    const buff = a?.buff ?? (fb.floors.includes(n) || (n >= fb.fromFloor && brng.chance(fb.chance)) ? brng.pick(fb.pool) : '');
    if (buff) for (const f of foes) if (!f.abilities.includes(buff)) f.abilities = [...f.abilities, buff];
    return { id: `f${n}`, n, nodes: [{ id: `f${n}n0`, type, foes, keyDrop, keys, ...(buff ? { buff } : {}) }], edges: [] };
  }

  private foe(n: number, s: FoeAuthor, type: NodeType, rng: Rng): FoeSpec {
    const g = this.file.generator;
    const boss = type === 'boss';
    const id = s.enemy || this.pickEnemy(n, boss, rng);
    const e = this.enemies.get(id)!;
    const abilities = s.abilities || (boss ? [] : g.abilityChances.filter((c) => n >= c.fromFloor && n <= (c.toFloor ?? Infinity) && rng.chance(c.chance)).map((c) => c.ability));
    let hp = s.hp ?? g.hp.base * Math.pow(g.hp.growth, n - 1) * e.hpK * (boss ? g.bossHpK : 1);
    // authored hp is final; generated hp gets ability multipliers (TOUGH)
    if (s.hp === undefined) for (const ab of abilities) for (const m of this.abilities.get(ab)?.mods || []) if (m.stat === 'maxHp' && m.op === 'mul') hp *= 1 + m.v;
    hp += g.hpAdd?.find((h) => n <= h.toFloor)?.add || 0;
    const atk = s.atk ?? (g.atk.base * Math.pow(g.atk.growth, n - 1) + g.atk.add) * e.atkK * (boss ? g.bossAtkK : 1);
    const D = g.def;
    const def = s.def ?? D.base + D.perFloor * Math.max(0, n - (D.fromFloor ?? 2) + 1) + (boss ? D.boss || 0 : 0);
    return { enemy: id, name: e.name, sprite: e.sprite, hp: Math.max(1, Math.round(hp)), atk: Math.max(1, Math.round(atk)), def: Math.round(def), abilities };
  }

  private pickEnemy(n: number, boss: boolean, rng: Rng): string {
    const all = [...this.enemies.values()].filter((e) => !!e.boss === boss);
    let pool = all.filter((e) => n >= e.minFloor && n <= e.maxFloor);
    if (!pool.length) pool = all.filter((e) => n >= e.minFloor); // deeper than the content: anything unlocked
    if (!pool.length) pool = all;
    return rng.pick(pool).id;
  }
}
