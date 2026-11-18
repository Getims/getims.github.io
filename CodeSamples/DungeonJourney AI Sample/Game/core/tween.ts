// Project copy of Modules/tween.ts (2026-10-23): the project repo builds on its own, without AI-Lab/Modules.
// Minimal tween + timer engine driven by game time (pauses with the game loop).
export type Ease = (t: number) => number;

export const Ease = {
  linear: (t: number) => t,
  outQuad: (t: number) => 1 - (1 - t) * (1 - t),
  inQuad: (t: number) => t * t,
  inOutSine: (t: number) => -(Math.cos(Math.PI * t) - 1) / 2,
  outBack: (t: number) => { const c1 = 1.70158, c3 = c1 + 1; return 1 + c3 * Math.pow(t - 1, 3) + c1 * Math.pow(t - 1, 2); },
  inBack: (t: number) => { const c1 = 1.70158, c3 = c1 + 1; return c3 * t * t * t - c1 * t * t; },
  inCubic: (t: number) => t * t * t,
  outCubic: (t: number) => 1 - Math.pow(1 - t, 3),
  inOutQuad: (t: number) => (t < 0.5 ? 2 * t * t : 1 - Math.pow(-2 * t + 2, 2) / 2),
  outBounce: (t: number) => {
    const n1 = 7.5625, d1 = 2.75;
    if (t < 1 / d1) return n1 * t * t;
    if (t < 2 / d1) return n1 * (t -= 1.5 / d1) * t + 0.75;
    if (t < 2.5 / d1) return n1 * (t -= 2.25 / d1) * t + 0.9375;
    return n1 * (t -= 2.625 / d1) * t + 0.984375;
  },
};

export interface Job {
  t: number; dur: number; delay: number;
  update?: (k: number) => void; done?: () => void;
  ease: Ease; yoyo: boolean; repeat: number; dir: number;
  tag?: string; dead?: boolean;
}

export class Tweens {
  private jobs: Job[] = [];

  /** Animate 0..1 over dur seconds. repeat=-1 means forever. */
  tween(dur: number, update: (k: number) => void, o: { ease?: Ease; delay?: number; done?: () => void; yoyo?: boolean; repeat?: number; tag?: string } = {}) {
    const j: Job = { t: 0, dur: Math.max(1e-4, dur), delay: o.delay ?? 0, update, done: o.done, ease: o.ease ?? Ease.outQuad, yoyo: !!o.yoyo, repeat: o.repeat ?? 0, dir: 1, tag: o.tag };
    this.jobs.push(j);
    return j;
  }

  /** Tween a numeric property of an object. */
  to<T extends object>(obj: T, key: keyof T, to: number, dur: number, o: Parameters<Tweens['tween']>[2] = {}) {
    let from = 0;
    let started = false;
    return this.tween(dur, (k) => {
      if (!started) { from = obj[key] as unknown as number; started = true; }
      (obj as any)[key] = from + (to - from) * k;
    }, o);
  }

  delay(sec: number, fn: () => void, tag?: string) {
    return this.tween(0, () => {}, { delay: sec, done: fn, tag });
  }

  kill(tag: string) { for (const j of this.jobs) if (j.tag === tag) j.dead = true; }
  killJob(j: object | undefined | null) { if (j) (j as Job).dead = true; }

  update(dt: number) {
    const list = this.jobs;
    for (let i = 0; i < list.length; i++) {
      const j = list[i];
      if (j.dead) continue;
      let step = dt;
      if (j.delay > 0) { j.delay -= dt; if (j.delay > 0) continue; step = -j.delay; j.delay = 0; }
      j.t += step;
      let p = Math.min(1, j.t / j.dur);
      const eased = j.ease(j.dir > 0 ? p : 1 - p);
      j.update?.(eased);
      if (p >= 1) {
        if (j.repeat !== 0) {
          if (j.repeat > 0) j.repeat--;
          j.t = 0;
          if (j.yoyo) j.dir = -j.dir;
        } else { j.dead = true; j.done?.(); }
      }
    }
    this.jobs = this.jobs.filter((j) => !j.dead);
  }
}

// Helpers.
export const lerp = (a: number, b: number, k: number) => a + (b - a) * k;
export const rand = (a: number, b: number) => a + Math.random() * (b - a);

/** Piecewise-linear lookup: table rows [x, v1, v2, ...], returns column `col` at x. */
export function lerpTable(table: number[][], x: number, col: number) {
  if (x <= table[0][0]) return table[0][col];
  const last = table[table.length - 1];
  if (x >= last[0]) return last[col];
  for (let i = 1; i < table.length; i++) {
    if (x <= table[i][0]) {
      const a = table[i - 1], b = table[i];
      return lerp(a[col], b[col], (x - a[0]) / (b[0] - a[0]));
    }
  }
  return last[col];
}
