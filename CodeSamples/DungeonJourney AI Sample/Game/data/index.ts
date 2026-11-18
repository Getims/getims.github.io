// Game data: JSON configs + floor generator, validated at boot.
import balanceJson from './balance.json';
import perksJson from './perks.json';
import enemiesJson from './enemies.json';
import abilitiesJson from './abilities.json';
import itemsJson from './items.json';
import floorsJson from './floors.json';
import timingJson from './timing.json';
import { FloorGen } from './floorgen';
import { validate, type DataSet } from './validate';
import type { Timing } from './types';

const set = {
  balance: balanceJson,
  perks: perksJson,
  enemies: enemiesJson,
  abilities: abilitiesJson,
  items: itemsJson,
  floors: floorsJson,
} as unknown as DataSet;

const byId = <T extends { id: string }>(list: T[]) => new Map(list.map((x) => [x.id, x]));

export const DATA = {
  balance: set.balance,
  perks: byId(set.perks),
  enemies: byId(set.enemies),
  abilities: byId(set.abilities),
  items: byId(set.items),
  floors: new FloorGen(set.floors, set.enemies, set.abilities),
  floorsFile: set.floors,
  timing: timingJson as Timing,
  errors: [] as string[],
};

export function checkData() {
  DATA.errors = validate(set, window.__A || null);
  for (const e of DATA.errors) console.error(`[data] ${e}`);
  return DATA.errors;
}
