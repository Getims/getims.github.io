// Sound: game events (bus) -> SFX, screens -> music. Volumes from Settings (Music / SFX, mute).
import { audio } from './core/audio';
import type { Rarity } from './data/types';
import { DATA } from './data';
import { bus } from './events';
import { game } from './game';
import { RATE, synthSounds } from './synth';

/** Sound keys = asset keys in build.config.mjs (Assets/Audio/*.wav, converted to MP3 by the build) + procedural (synth.ts). */
type Sfx = 'click' | 'crit' | 'death' | 'equip' | 'hit' | 'hurt' | 'key' | 'legendary' | 'reward' | 'slot_spin' | 'slot_stop' | 'victory'
  | 'salvage' | 'merge' | 'roar' | 'anvil' | 'chime_uncommon' | 'chime_rare' | 'chime_epic' | 'chime_legendary';

/** Per-sound mix on top of the SFX slider. */
const MIX: Record<Sfx, number> = {
  click: 0.6, crit: 1, death: 0.9, equip: 0.9, hit: 0.7, hurt: 0.8, key: 1, legendary: 1, reward: 1, slot_spin: 0.7, slot_stop: 0.8, victory: 1,
  salvage: 0.7, merge: 0.8, roar: 1, anvil: 0.6, chime_uncommon: 0.45, chime_rare: 0.5, chime_epic: 0.55, chime_legendary: 0.6,
};

/** Sounds that repeat a lot: small random pitch so a series does not sound like one sample. */
const JITTER = new Set<Sfx>(['hit', 'hurt', 'crit', 'slot_stop', 'click']);

/** Screen -> track. Card screens keep the current track: after PLAY the menu music plays on until the battle.
 *  died: the battle music already faded out on hero_death; the menu track comes in slowly after a pause. */
const MUSIC_OF: Record<string, [string, number, number]> = {
  menu: ['menu', 0.4, 0], inventory: ['menu', 0.4, 0], died: ['menu', 2.5, 1],
  battle: ['combat', 0.4, 0], descending: ['combat', 0.4, 0],
};

/** Last played keys (sfx and music), for tests: window.__game.sounds. */
export const soundLog: string[] = [];
const logSound = (key: string) => { soundLog.push(key); if (soundLog.length > 200) soundLog.shift(); };

const lastAt: Partial<Record<Sfx, number>> = {};

/** Anti machine-gun: one key at most every 45 ms (110 ms in Fast Replay), max 3 voices (2 in Fast Replay). */
function sfx(key: Sfx, k = 1, rate = 1, force = false) {
  const now = performance.now();
  if (!force && now - (lastAt[key] ?? -1e9) < (game.fast ? 110 : 45)) return;
  if (game.fast) k *= 0.6; // Fast Replay: x3 speed, many hits per second
  if (JITTER.has(key)) rate *= 0.95 + Math.random() * 0.1;
  if (!audio.play(key, game.settings.sfx * MIX[key] * k, rate, game.fast ? 2 : 3)) return;
  lastAt[key] = now;
  logSound(key);
}

const chime = (r: Rarity) => { if (r !== 'common') sfx(`chime_${r}` as Sfx); };

let musicVol = -1;
let muted: boolean | null = null;

/** Applies Music / mute from Settings: every frame and from the sliders (the loop is paused while Settings is open). */
export function syncSound() {
  const s = game.settings;
  if (s.music !== musicVol) audio.setMusicVolume((musicVol = s.music) * 1.2);
  if (s.mute !== muted) audio.setMuted((muted = s.mute));
}

/** SFX slider released: a click at the new volume. */
export function previewSfx() { sfx('click'); }

export async function initSound() {
  const sources: Record<string, string> = {};
  for (const [k, uri] of Object.entries(window.__A || {})) if (uri.startsWith('data:audio')) sources[k] = uri;
  // decode in the background: the menu does not wait for it
  audio.init(sources);
  for (const [k, d] of Object.entries(synthSounds())) audio.addBuffer(k, d, RATE);
  syncSound();

  const unlock = () => audio.unlock();
  addEventListener('pointerdown', unlock, true);
  addEventListener('keydown', unlock, true);
  document.addEventListener('visibilitychange', () => audio.setHidden(document.hidden));

  // UI click: any button inside the stage (cards are picked by click too)
  game.stage.ui.addEventListener('pointerdown', (e) => {
    const b = (e.target as HTMLElement).closest('button, .pcard:not(.spin)') as HTMLButtonElement | null;
    if (b && !b.disabled) sfx('click');
  });

  bus.on('screen', ({ id }) => { const m = MUSIC_OF[id]; if (m) { logSound(`music:${m[0]}`); audio.playMusic(...m); } });

  // battle
  bus.on('hit', ({ from, crit, exec }) => {
    if (exec) sfx('crit', 1.1, 0.75); // heavy chop: the crit sample lower
    else if (crit) sfx('crit');
    else sfx(from === 'hero' ? 'hit' : 'hurt');
  });
  bus.on('thorns', ({ from }) => sfx(from === 'hero' ? 'hit' : 'hurt', 0.5, 1.3)); // from = who reflects the damage
  bus.on('boss_appear', () => sfx('roar'));
  bus.on('foe_death', () => sfx('death', 0.6, 1.15));
  bus.on('boss_death', () => sfx('death', 1, 0.85, true)); // right after foe_death: past the repeat guard
  bus.on('hero_death', () => { sfx('death'); audio.playMusic(null, 0.6); });
  bus.on('floor_clear', () => sfx('victory'));
  bus.on('key_drop', () => sfx('key'));
  bus.on('revive', () => sfx('reward'));
  // cards
  bus.on('card_spin_start', () => sfx('slot_spin'));
  bus.on('card_stop', ({ rarity }) => { sfx('slot_stop'); chime(rarity); });
  bus.on('card_pick', ({ perk }) => {
    const r = DATA.perks.get(perk)?.rarity || 'common';
    if (r === 'legendary') sfx('legendary'); else if (r === 'common') sfx('reward', 0.5); else chime(r);
  });
  // meta: the key shows for 0.5 s before the reward flash (inventory.ts)
  bus.on('chest_open', ({ rarity }) => {
    sfx('key');
    setTimeout(() => { sfx(rarity === 'legendary' ? 'legendary' : 'reward'); chime(rarity); }, 500);
  });
  bus.on('merge', ({ rarity }) => { sfx('merge'); if (rarity === 'legendary') sfx('legendary', 0.7); });
  bus.on('upgrade', () => sfx('anvil'));
  bus.on('salvage', () => sfx('salvage'));
  bus.on('equip', () => sfx('equip'));
  bus.on('unequip', () => sfx('equip', 0.7, 0.8));
}
