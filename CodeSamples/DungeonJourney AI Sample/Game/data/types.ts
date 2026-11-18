// Data schemas (frozen after 3.2: the floor editor of stage 5 and saves depend on them).

export const RARITIES = ['common', 'uncommon', 'rare', 'epic', 'legendary'] as const;
export type Rarity = (typeof RARITIES)[number];

export const KEY_TYPES = ['copper', 'silver', 'gold'] as const;
export type KeyType = (typeof KEY_TYPES)[number];

export const SLOTS = ['head', 'body', 'arms', 'legs', 'weapon', 'secondary', 'accessory'] as const;
export type Slot = (typeof SLOTS)[number];

/** Fractions are 0..1 (crit 0.08 = 8%); critMult is a multiplier (2.0). */
export const STATS = ['maxHp', 'atk', 'def', 'crit', 'critMult', 'lifesteal', 'thorns', 'thornsCap', 'execute', 'defToAtk', 'hpToAtk', 'atkPerKill'] as const;
export type Stat = (typeof STATS)[number];
export type Stats = Record<Stat, number>;

/** add: +v; mul: ×(1+v), stacks additively within one stat; set: replaces (REPLACES cards, last one wins). */
export interface Mod { stat: Stat; op: 'add' | 'mul' | 'set'; v: number }

export const HOOKS = ['on_pick', 'on_battle_start', 'on_turn_start', 'on_before_hit', 'on_hit', 'on_crit', 'on_damaged', 'on_kill', 'on_low_hp', 'on_floor_start', 'on_floor_end'] as const;
export type Hook = (typeof HOOKS)[number];

export const EFFECTS = ['heal_dmg', 'heal_missing', 'heal_max', 'bonus_dmg', 'strike_first'] as const;
export type EffectKind = (typeof EFFECTS)[number];

/** Data-driven hook: perks, enemy abilities and (stage 7) class passives use the same engine. */
export interface Effect {
  on: Hook;
  do: EffectKind;
  k: number;
  if?: { selfHpBelow?: number; targetHpBelow?: number; chance?: number; firstTurn?: boolean };
}

export interface RarityDef { color: string; weight: number; salvage: number; upgradeCost: number }

export interface PerkDef {
  id: string;
  name: string;
  rarity: Rarity;
  desc: string;
  mods?: Mod[];
  effects?: Effect[];
  /** Not seen in the original clips: text/values made up, to be checked. */
  guess?: boolean;
}

export interface AbilityDef { id: string; label: string; color: string; mods?: Mod[]; effects?: Effect[] }

export interface EnemyDef {
  id: string;
  name: string;
  sprite: string;
  /** Multipliers over the floor curve. */
  hpK: number;
  atkK: number;
  minFloor: number;
  maxFloor: number;
  boss?: boolean;
}

/** icon: asset key, '' = placeholder; guess: stats made up (not seen in the original). */
export interface ItemDef { id: string; name: string; slot: Slot; rarity: Rarity; icon: string; mods: Mod[]; guess?: boolean }

/** count: keys per drop (each rolled on weights); the generator raises it with depth (FloorGenConfig.keyCount). */
export interface KeyDropTable { chance: number; weights: Partial<Record<KeyType, number>>; count?: number }

export interface BalanceConfig {
  hero: Stats;
  /** Defense blocks a share of the hit: block = DEF / (DEF + defK), at most maxBlock. dmg = max(minDmg, raw × (1 − block)). */
  damage: { defK: number; minDmg: number; maxBlock: number };
  /** Who strikes first, rolled once per floor: hero / foe / alternate (who opens is 50/50, then turns swap foe to foe). */
  firstStrike: { hero: number; foe: number; alternate: number };
  rarities: Record<Rarity, RarityDef>;
  keyDrops: Record<string, KeyDropTable>;
  chests: Record<KeyType, Partial<Record<Rarity, number>>>;
  shardsPerKey: number;
  upgradeStep: number;
  mergeCount: number;
  cards: { pathRarity: Rarity; bigRewardRarities: Rarity[]; count: number };
  /** Screen shake: amplitude px, duration s. */
  shake: Record<'small' | 'strong' | 'victory', { amp: number; dur: number }>;
}

export interface Timing {
  enter: number; lunge: number; lungeBack: number; slashFly: number; foeRun: number; foeBack: number; hitGap: number; ambush: number; strike: number; number: number; flash: number;
  foeFirst: number; foeFade: number; foeSpawn: number; clearFlash: number; keyDrop: number; death: number; victory: number;
  spinMin: number; lockStep: number; spinTick: number; descend: number; rankDelay: number; fastScale: number;
}

// ---- floors: authoring format (floors.json, stage 5 editor writes it) ----

/** Missing fields (or `{}`) are filled by the generator. */
export interface FoeAuthor { enemy?: string; hp?: number; atk?: number; def?: number; abilities?: string[] }

export interface FloorAuthor {
  floor: number;
  foes?: FoeAuthor[];
  boss?: FoeAuthor;
  keyDrop?: string;
  /** Floor buff id; '' = none (overrides the generator roll). */
  buff?: string;
}

export interface FloorGenConfig {
  hp: { base: number; growth: number };
  atk: { base: number; growth: number; add: number };
  /** DEF = base + perFloor·(n − fromFloor + 1) from fromFloor on (default 2: F1 = 0); bosses + boss. */
  def: { base: number; perFloor: number; fromFloor?: number; boss?: number };
  /** Flat HP on top of the curve and of authored HP (bosses too): first entry whose toFloor >= n. */
  hpAdd?: { toFloor: number; add: number }[];
  bossHpK: number;
  bossAtkK: number;
  foesPerFloor: number;
  bossEvery: number;
  keyEvery: number;
  /** toFloor: last floor the ability can roll on (TOUGH is seen only on early floors in the original). */
  abilityChances: { ability: string; fromFloor: number; toFloor?: number; chance: number }[];
  /** Floor buff: one random ability from the pool for every foe of the floor; always on `floors`, then with `chance` from `fromFloor`. */
  floorBuff: { pool: string[]; floors: number[]; fromFloor: number; chance: number };
  /** Keys per drop: 1, then +1 every `every` floors starting at `from` (20 → 2, 30 → 3). */
  keyCount: { from: number; every: number };
}

export interface FloorsFile { version: number; seed: string; generator: FloorGenConfig; floors: FloorAuthor[] }

// ---- floors: runtime graph (one node per floor in the clone; rooms on stage 7) ----

export interface FoeSpec { enemy: string; name: string; sprite: string; hp: number; atk: number; def: number; abilities: string[] }

export type NodeType = 'fight' | 'boss';

/** buff: floor ability shown in the HUD (also listed in every foe's abilities); keys: keys per drop. */
export interface FloorNode { id: string; type: NodeType; foes: FoeSpec[]; keyDrop: string; buff?: string; keys?: number }

export interface Floor { id: string; n: number; nodes: FloorNode[]; edges: [string, string][] }
