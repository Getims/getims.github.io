// The single list of game events (VFX, sound, analytics subscribe here).
import { Bus } from './core/bus';
import type { KeyType, Rarity } from './data/types';

export type Side = 'hero' | 'foe';

export interface EventMap {
  screen: { id: string };
  // battle
  battle_start: { floor: number; heroFirst: boolean };
  hit: { from: Side; dmg: number; crit?: boolean; exec?: boolean };
  crit: { from: Side; dmg: number };
  execute: { from: Side };
  thorns: { from: Side; dmg: number };
  heal: { side: Side; amount: number };
  boss_appear: { floor: number };
  foe_death: { floor: number; index: number };
  boss_death: { floor: number };
  hero_death: { floor: number };
  floor_clear: { floor: number };
  key_drop: { key: KeyType };
  revive: { floor: number };
  // cards
  card_spin_start: { count: number };
  card_spin_tick: { index: number };
  card_stop: { index: number; rarity: Rarity };
  card_lock: { index: number; locked: boolean };
  card_pick: { perk: string };
  // meta
  equip: { item: string };
  unequip: { item: string };
  merge: { item: string; rarity: Rarity };
  upgrade: { item: string; level: number };
  salvage: { item: string; shards: number };
  chest_open: { key: KeyType; rarity: Rarity };
}

export const bus = new Bus<EventMap>();
