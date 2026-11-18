// Procedural SFX (no files, license-free): rendered to mono buffers once at start, played like the pack's sounds.
// salvage, merge, boss roar, upgrade anvil, rarity chimes (chime_uncommon..chime_legendary).

export const RATE = 44100;

const buf = (sec: number) => new Float32Array(Math.ceil(sec * RATE));
const noise = () => Math.random() * 2 - 1;

/** One-pole low-pass in place; cutoff may change over time (fn of seconds). */
function lowpass(d: Float32Array, cutoff: (t: number) => number) {
  let y = 0;
  for (let i = 0; i < d.length; i++) {
    const a = 1 - Math.exp((-2 * Math.PI * cutoff(i / RATE)) / RATE);
    y += a * (d[i] - y);
    d[i] = y;
  }
}

/** Bell-ish tone added into d: base + a quieter inharmonic partial, exponential decay. */
function bell(d: Float32Array, at: number, f: number, vol: number, decay: number) {
  const s = Math.floor(at * RATE);
  for (let i = 0; s + i < d.length; i++) {
    const t = i / RATE;
    const env = Math.exp(-t / decay) * Math.min(1, t / 0.003);
    if (env < 0.001) break;
    d[s + i] += vol * env * (Math.sin(2 * Math.PI * f * t) + 0.35 * Math.sin(2 * Math.PI * f * 2.76 * t) + 0.15 * Math.sin(2 * Math.PI * f * 5.4 * t));
  }
}

function normalize(d: Float32Array, peak = 0.9) {
  let m = 0;
  for (const v of d) m = Math.max(m, Math.abs(v));
  if (m > 0) for (let i = 0; i < d.length; i++) d[i] *= peak / m;
  return d;
}

/** Salvage: crunch of the item + falling shard pings. */
function salvage() {
  const d = buf(0.6);
  const crunch = buf(0.6);
  for (const g of [0, 0.045, 0.09]) {
    const s = Math.floor(g * RATE);
    for (let i = 0; i < 0.07 * RATE; i++) crunch[s + i] += noise() * Math.exp(-i / RATE / 0.02);
  }
  lowpass(crunch, () => 2500);
  for (let i = 0; i < d.length; i++) d[i] = crunch[i];
  [0.12, 0.17, 0.23, 0.3].forEach((at, i) => bell(d, at, 2600 + i * 420 + Math.random() * 200, 0.25, 0.08));
  return normalize(d);
}

/** Merge: rising magic sweep + whoosh, ends in a sparkle. */
function merge() {
  const d = buf(1.1);
  let ph = 0;
  const w = buf(1.1);
  for (let i = 0; i < d.length; i++) {
    const t = i / RATE;
    const k = Math.min(1, t / 0.6);
    const f = 300 + 900 * k * k;
    ph += (2 * Math.PI * f) / RATE;
    const env = Math.min(1, t / 0.05) * (t < 0.6 ? 0.5 + 0.5 * k : Math.exp(-(t - 0.6) / 0.15));
    d[i] = env * 0.35 * (Math.sin(ph) + 0.5 * Math.sin(ph * 1.5) + 0.25 * Math.sin(ph * 2.01)) * (0.8 + 0.2 * Math.sin(2 * Math.PI * 14 * t));
    w[i] = noise() * env * 0.5;
  }
  lowpass(w, (t) => 300 + 3000 * Math.min(1, t / 0.6));
  for (let i = 0; i < d.length; i++) d[i] += w[i];
  [1568, 2093, 2637, 3136].forEach((f, i) => bell(d, 0.58 + i * 0.05, f, 0.3, 0.25));
  return normalize(d);
}

/** Boss roar: low growling saw with a pitch drop, noise breath, soft clipping. */
function roar() {
  const d = buf(1.5);
  const n = buf(1.5);
  let ph = 0;
  for (let i = 0; i < d.length; i++) {
    const t = i / RATE;
    const f = 95 - 35 * Math.min(1, t / 1.3) + 6 * Math.sin(2 * Math.PI * 23 * t) * Math.random();
    ph += f / RATE;
    const env = Math.min(1, t / 0.12) * Math.exp(-Math.max(0, t - 0.5) / 0.4);
    d[i] = env * (2 * (ph % 1) - 1);
    n[i] = env * noise() * 0.8;
  }
  lowpass(d, () => 900);
  lowpass(n, (t) => 1400 - 900 * Math.min(1, t / 1.3));
  for (let i = 0; i < d.length; i++) d[i] = Math.tanh(2.5 * (d[i] + n[i]));
  return normalize(d);
}

/** Upgrade: hammer on an anvil — click + inharmonic metal ring. */
function anvil() {
  const d = buf(0.7);
  for (let i = 0; i < 0.01 * RATE; i++) d[i] += noise() * (1 - i / (0.01 * RATE));
  for (const [f, v, dec] of [[880, 0.5, 0.25], [1404, 0.35, 0.18], [2310, 0.25, 0.12], [3150, 0.15, 0.08]]) bell(d, 0.002, f, v, dec);
  return normalize(d);
}

/** Rarity chime: more notes and a longer tail with each rarity. */
function chime(level: number) {
  const notes = [1047, 1319, 1568, 2093, 2637];
  const d = buf(0.5 + level * 0.25);
  for (let i = 0; i <= level; i++) bell(d, i * 0.07, notes[i], 0.4, 0.18 + level * 0.06);
  if (level >= 2) {
    const w = buf(d.length / RATE);
    for (let i = 0; i < w.length; i++) { const t = i / RATE; w[i] = noise() * Math.min(1, t / 0.05) * Math.exp(-t / 0.2) * 0.4; }
    lowpass(w, (t) => 600 + 4000 * Math.min(1, t / 0.2));
    for (let i = 0; i < d.length; i++) d[i] += w[i];
  }
  return normalize(d, 0.8);
}

export function synthSounds(): Record<string, Float32Array> {
  return {
    salvage: salvage(), merge: merge(), roar: roar(), anvil: anvil(),
    chime_uncommon: chime(1), chime_rare: chime(2), chime_epic: chime(3), chime_legendary: chime(4),
  };
}
