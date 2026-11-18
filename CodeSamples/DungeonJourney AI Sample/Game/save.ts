// Saves: one JSON in storage (platform.storage, localStorage now), versioned with migrations.
// A broken save is backed up to KEY_BAK and the game starts clean. Written on events, never on a timer.
import { DATA } from './data';
import { bus } from './events';
import { game } from './game';
import { newMeta, type MetaState } from './meta';
import { platform } from './platform';
import type { RunState } from './run';

export const SAVE_KEY = 'dj_save';
export const SAVE_BAK = 'dj_save_bak';
export const SAVE_VERSION = 1;

export interface SaveFile { v: number; meta: MetaState; run: RunState | null }

/** migrate[n] turns a v(n) file into v(n+1). */
const migrate: Record<number, (s: any) => any> = {};

export function serialize(): SaveFile {
  return { v: SAVE_VERSION, meta: game.meta, run: game.run };
}

export function save() {
  platform.storage.set(SAVE_KEY, JSON.stringify(serialize()));
}

let pending = false;
/** Coalesce several events of one frame into one write. */
export function saveSoon() {
  if (pending) return;
  pending = true;
  queueMicrotask(() => { pending = false; save(); });
}

/** Fills missing fields from a fresh state (new fields added without a version bump). */
function fill<T extends object>(base: T, data: any): T {
  if (!data || typeof data !== 'object') return base;
  for (const k of Object.keys(base) as (keyof T)[]) {
    const b = base[k];
    const d = data[k];
    if (d === undefined) continue;
    const record = b && typeof b === 'object' && !Array.isArray(b) && Object.keys(b).length === 0; // e.g. equipped: keys are not known up front
    base[k] = b && typeof b === 'object' && !Array.isArray(b) && !record ? fill(b as object, d) as T[keyof T] : d;
  }
  return base;
}

export function load(): 'none' | 'ok' | 'reset' {
  const raw = platform.storage.get(SAVE_KEY);
  if (!raw) return 'none';
  try {
    let s = JSON.parse(raw);
    if (typeof s?.v !== 'number' || s.v > SAVE_VERSION) throw new Error(`bad version ${s?.v}`);
    while (s.v < SAVE_VERSION) { const f = migrate[s.v]; if (!f) throw new Error(`no migration from v${s.v}`); s = f(s); s.v++; }
    const meta = fill(newMeta(), s.meta);
    meta.items = (meta.items || []).filter((i) => DATA.items.has(i.id)); // items removed from the config
    for (const [slot, uid] of Object.entries(meta.equipped)) if (!meta.items.some((i) => i.uid === uid)) delete meta.equipped[slot as keyof typeof meta.equipped];
    game.meta = meta;
    const run = s.run as RunState | null;
    game.run = run && typeof run.floor === 'number' && Array.isArray(run.perks) ? { ...run, perks: run.perks.filter((p) => DATA.perks.has(p)) } : null;
    return 'ok';
  } catch (e) {
    console.warn('save is broken, starting clean:', e);
    platform.storage.set(SAVE_BAK, raw);
    platform.storage.remove(SAVE_KEY);
    return 'reset';
  }
}

export function resetSave() {
  platform.storage.remove(SAVE_KEY);
  game.meta = newMeta();
  game.run = null;
}

/** Autosave points: anything that changes run or meta. */
export function initAutosave() {
  const events = ['floor_clear', 'hero_death', 'card_pick', 'revive', 'key_drop', 'merge', 'upgrade', 'salvage', 'chest_open', 'screen'] as const;
  for (const e of events) bus.on(e, saveSoon);
}
