// Battle presenter: plays the core's log on a timeline and draws the scene on canvas.
// Positions/scales measured on the original's screenshots (1920×1080). Attack = short lunge + slash that flies to the foe.
// Events for VFX/sound are emitted here, in sync with what the player sees.
import { Ease, lerp } from '../core/tween';
import { images } from '../core/assets';
import { H, W } from '../core/stage';
import { DATA } from '../data';
import type { FoeSpec } from '../data/types';
import { bus } from '../events';
import { game } from '../game';
import { drawCover } from '../ui';
import type { LogEntry, Side } from './core';

export const HERO_X = 402;
export const FOE_X = 1343;
export const GROUND = 824;
const HERO_SCALE = 1.0;
const FOE_SCALE = 1.0;
const BOSS_SCALE = 1.25;
/** The pack's victory pose is drawn ~1.3× larger than the idle / attack frames. */
const VICTORY_SCALE = 0.78;
const LUNGE = 110;
/** Foes have no slash: they run up to the hero and hit in melee. */
const FOE_RUN = FOE_X - (HERO_X + 240);
const SLASH_FROM = HERO_X + 330;
const SLASH_TO = FOE_X - 110;
const SLASH_Y = GROUND - 240;

interface Num { x: number; y: number; text: string; color: string; size: number; t0: number }
type Pose = 'idle' | 'stance' | 'victory' | 'dead';

export interface PresenterHooks {
  hud(): void;
  banner(kind: 'ambush' | 'strike'): void;
  status(kind: 'enter' | 'fight' | 'none'): void;
  end(): void;
}

export class Presenter {
  t = 0;
  done = false;
  heroHp: number;
  foeHp = 0;
  foeMax = 1;
  foeIdx = -1;
  foeDead = false;
  pose: Pose = 'idle';
  private queue: { at: number; fn: () => void }[] = [];
  private foeT0 = 0;
  private foeOutT0 = -1;
  private lunge: { side: Side; t0: number } | null = null;
  private slashT0 = -9;
  private flash: { side: Side; t0: number } | null = null;
  private screenFlash: { color: string; t0: number; dur: number } | null = null;
  private nums: Num[] = [];
  private shakeFx: { t0: number; amp: number; dur: number } | null = null;
  private confetti: { x: number; y: number; vx: number; vy: number; rot: number; vr: number; w: number; h: number; color: string }[] = [];
  private readonly T = DATA.timing;

  constructor(log: LogEntry[], private foes: FoeSpec[], heroHp: number, public heroMax: number, private floor: number, private boss: boolean, private hooks: PresenterHooks) {
    this.heroHp = heroHp;
    this.schedule(log);
  }

  private at(time: number, fn: () => void) { this.queue.push({ at: time, fn }); }

  private schedule(log: LogEntry[]) {
    const T = this.T;
    let t = 0;
    let impact = 0;
    this.at(0, () => this.hooks.status('enter'));
    for (const e of log) {
      switch (e.t) {
        case 'foe':
          this.at(t, () => { this.foeIdx = e.i; this.foeHp = e.hp; this.foeMax = e.max; this.foeDead = false; this.foeT0 = this.t; this.foeOutT0 = -1; this.hooks.hud(); if (this.boss) { this.shake('strong'); bus.emit('boss_appear', { floor: this.floor }); } });
          t += e.i === 0 ? T.enter + 0.1 : T.foeSpawn;
          break;
        case 'start':
          this.at(t, () => { this.pose = 'stance'; this.hooks.banner(e.heroFirst ? 'strike' : 'ambush'); this.hooks.status('fight'); });
          t += e.heroFirst ? T.strike : T.ambush;
          break;
        case 'hit': {
          const hero = e.from === 'hero';
          this.at(t, () => { this.lunge = { side: e.from, t0: this.t }; if (hero) this.slashT0 = this.t + T.lunge * 0.5; });
          impact = t + (hero ? T.lunge + T.slashFly : T.foeRun);
          this.at(impact, () => {
            const target: Side = hero ? 'foe' : 'hero';
            if (target === 'foe') this.foeHp = e.hp; else this.heroHp = e.hp;
            this.flash = { side: target, t0: this.t };
            const text = e.exec ? '999!' : e.crit ? `${e.dmg}!` : String(e.dmg);
            const color = e.exec ? '#ff3030' : e.crit ? '#ffd23a' : target === 'hero' ? '#ff4a3a' : '#ffffff';
            this.num(target, text, color, e.crit || e.exec ? 66 : 52);
            this.shake(e.crit || e.exec ? 'strong' : 'small');
            bus.emit('hit', { from: e.from, dmg: e.dmg, crit: e.crit, exec: e.exec });
            if (e.crit) bus.emit('crit', { from: e.from, dmg: e.dmg });
            if (e.exec) bus.emit('execute', { from: e.from });
            this.hooks.hud();
          });
          t = impact + T.hitGap - (hero ? T.lunge : T.foeRun) + (hero ? 0 : T.foeBack * 0.5);
          break;
        }
        case 'thorns':
          this.at(impact + 0.15, () => {
            const victim: Side = e.from === 'hero' ? 'foe' : 'hero';
            if (victim === 'foe') this.foeHp = e.hp; else this.heroHp = e.hp;
            this.num(victim, String(e.dmg), '#ff9a3c', 44);
            bus.emit('thorns', { from: e.from, dmg: e.dmg });
            this.hooks.hud();
          });
          break;
        case 'heal':
          this.at(impact + 0.1, () => {
            if (e.side === 'hero') this.heroHp = e.hp; else this.foeHp = e.hp;
            this.num(e.side, `+${e.amount}`, '#5fe36a', 40);
            bus.emit('heal', { side: e.side, amount: e.amount });
            this.hooks.hud();
          });
          break;
        case 'foe_death':
          this.at(t, () => {
            this.foeOutT0 = this.t;
            this.foeDead = true;
            this.hooks.hud();
            bus.emit('foe_death', { floor: this.floor, index: e.i });
            if (this.boss) bus.emit('boss_death', { floor: this.floor });
          });
          t += T.foeFade;
          break;
        case 'hero_death':
          this.at(t, () => { this.pose = 'dead'; this.shake('strong'); this.screenFlash = { color: '#ff2020', t0: this.t, dur: 0.25 }; this.hooks.status('none'); bus.emit('hero_death', { floor: this.floor }); });
          t += T.death;
          break;
        case 'clear':
          this.at(t, () => { this.pose = 'victory'; this.shake('victory'); this.screenFlash = { color: '#fff3a0', t0: this.t, dur: T.clearFlash }; this.hooks.status('none'); if (this.boss) this.burstConfetti(); bus.emit('floor_clear', { floor: this.floor }); });
          t += 0.1;
          break;
      }
    }
    this.at(t, () => { this.done = true; this.hooks.end(); });
    this.queue.sort((a, b) => a.at - b.at);
  }

  private num(side: Side, text: string, color: string, size: number) {
    const x = side === 'hero' ? HERO_X + 40 : FOE_X;
    this.nums.push({ x: x + (Math.random() - 0.5) * 40, y: GROUND - 150, text, color, size, t0: this.t });
  }

  /** Hit: small, crit / execute / hero death: strong, floor cleared: victory. Off in settings -> none. */
  private shake(kind: 'small' | 'strong' | 'victory') {
    if (!game.settings.shake) return;
    const s = DATA.balance.shake[kind];
    const amp = game.fast ? s.amp * 0.5 : s.amp; // Fast Replay: hits come 3x as often
    if (this.shakeFx && this.t - this.shakeFx.t0 < this.shakeFx.dur && this.shakeFx.amp > amp) return; // keep the stronger one
    this.shakeFx = { t0: this.t, amp, dur: s.dur };
  }

  /** Boss defeated: confetti shoots up and inwards from the bottom corners of the screen. */
  private burstConfetti() {
    const colors = ['#ff4a5a', '#ffd23a', '#4fc25a', '#3f8cff', '#b04fff', '#ff9a3c', '#ffffff'];
    for (let i = 0; i < 160; i++) {
      const left = i % 2 === 0;
      this.confetti.push({
        x: left ? -20 : W + 20, y: H * (0.88 + Math.random() * 0.12),
        vx: (left ? 1 : -1) * (300 + Math.random() * 800), vy: -(1000 + Math.random() * 900),
        rot: Math.random() * 6.28, vr: (Math.random() - 0.5) * 16,
        w: 12 + Math.random() * 12, h: 7 + Math.random() * 7, color: colors[i % colors.length],
      });
    }
  }

  /** Skip the rest of the timeline (debug: end floor now). */
  finish() {
    while (this.queue.length) this.queue.shift()!.fn();
  }

  update(dt: number) {
    this.t += dt;
    while (this.queue.length && this.queue[0].at <= this.t) this.queue.shift()!.fn();
    this.nums = this.nums.filter((n) => this.t - n.t0 < this.T.number);
    for (const c of this.confetti) {
      c.vx *= Math.pow(0.35, dt); c.vy += 1400 * dt;
      c.x += c.vx * dt; c.y += c.vy * dt; c.rot += c.vr * dt;
    }
    this.confetti = this.confetti.filter((c) => c.y < H + 40);
  }

  /** Hero: short lunge (the slash flies on). Foe: runs up to the hero, hits, runs back. */
  private lungeOffset(side: Side) {
    const l = this.lunge;
    if (!l || l.side !== side) return 0;
    const T = this.T;
    const hero = side === 'hero';
    const go = hero ? T.lunge : T.foeRun;
    const back = hero ? T.lungeBack : T.foeBack;
    const dt = this.t - l.t0;
    let v = 0;
    if (dt < go) v = Ease.outQuad(dt / go);
    else if (dt < go + 0.08) v = 1;
    else if (dt < go + 0.08 + back) v = 1 - Ease.inOutQuad((dt - go - 0.08) / back);
    return hero ? LUNGE * v : -FOE_RUN * v;
  }

  render(ctx: CanvasRenderingContext2D, hideFoe = false) {
    const T = this.T;
    ctx.save();
    const sh = this.shakeFx;
    if (sh && this.t - sh.t0 < sh.dur) {
      const a = sh.amp * (1 - (this.t - sh.t0) / sh.dur);
      ctx.translate((Math.random() * 2 - 1) * a, (Math.random() * 2 - 1) * a);
    }
    // background oversized by 20 px on each side so the shake shows no edges
    ctx.translate(-20, -20);
    drawCover(ctx, images.bg_battle, W + 40, H + 40);
    ctx.translate(20, 20);

    // hero: runs in from the left at the start
    const enterK = Math.min(1, this.t / T.enter);
    const heroX = lerp(-200, HERO_X, Ease.outQuad(enterK)) + this.lungeOffset('hero');
    const lunging = this.lunge?.side === 'hero' && this.t - this.lunge.t0 < T.lunge + 0.08;
    // new pack: idle 2 frames, attack 3 frames (wind-up / strike / recover), victory; no hurt / death frames
    const lungeK = lunging ? (this.t - this.lunge!.t0) / (T.lunge + 0.08) : 0;
    const heroImg = this.pose === 'victory' ? images.hero_victory
      : lunging ? images[lungeK < 0.25 ? 'hero_attack_1' : lungeK < 0.75 ? 'hero_attack_2' : 'hero_attack_3']
      : this.pose === 'dead' ? images.hero_idle_1
      : images[Math.floor(this.t / 0.45) % 2 ? 'hero_idle_2' : 'hero_idle_1'];
    const heroHurt = this.pose === 'dead' || (this.flash?.side === 'hero' && this.t - this.flash.t0 < T.flash * 1.5);
    drawSprite(ctx, heroImg, heroX, GROUND, HERO_SCALE * (this.pose === 'victory' ? VICTORY_SCALE : 1), heroHurt ? '#ff2020' : null, 0.65, 1);

    // foe
    const foe = this.foes[this.foeIdx];
    if (foe && !hideFoe) {
      let alpha = Math.min(1, (this.t - this.foeT0) / 0.12);
      if (this.foeOutT0 >= 0) alpha = Math.max(0, 1 - (this.t - this.foeOutT0) / T.foeFade);
      const foeHit = this.flash?.side === 'foe' && this.t - this.flash.t0 < T.flash;
      drawSprite(ctx, images[foe.sprite], FOE_X + this.lungeOffset('foe'), GROUND, this.boss ? BOSS_SCALE : FOE_SCALE, foeHit ? '#ffffff' : null, 1, alpha);
    }

    // slash flying from the sword to the foe
    const sk = (this.t - this.slashT0) / T.slashFly;
    const slash = images.fx_slash;
    if (slash && sk >= 0 && sk < 1) {
      const x = lerp(SLASH_FROM, SLASH_TO, Ease.inQuad(sk));
      const s = 1.05;
      ctx.globalAlpha = sk < 0.75 ? 0.95 : 0.95 * (1 - (sk - 0.75) / 0.25);
      ctx.drawImage(slash, x - (slash.width * s) / 2, SLASH_Y - (slash.height * s) / 2, slash.width * s, slash.height * s);
      ctx.globalAlpha = 1;
    }

    // floating numbers
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    for (const n of this.nums) {
      const k = (this.t - n.t0) / T.number;
      ctx.globalAlpha = 1 - Ease.inQuad(k);
      ctx.font = `700 ${n.size}px Cinzel, serif`;
      ctx.lineWidth = 6;
      ctx.strokeStyle = '#000';
      ctx.strokeText(n.text, n.x, n.y - 60 * k);
      ctx.fillStyle = n.color;
      ctx.fillText(n.text, n.x, n.y - 60 * k);
    }
    ctx.globalAlpha = 1;

    for (const c of this.confetti) {
      ctx.save();
      ctx.translate(c.x, c.y);
      ctx.rotate(c.rot);
      ctx.scale(1, Math.abs(Math.cos(c.rot * 1.7)) * 0.8 + 0.2);
      ctx.fillStyle = c.color;
      ctx.fillRect(-c.w / 2, -c.h / 2, c.w, c.h);
      ctx.restore();
    }

    const f = this.screenFlash;
    if (f && this.t - f.t0 < f.dur) {
      ctx.globalAlpha = 0.85 * game.flashK * (1 - (this.t - f.t0) / f.dur);
      ctx.fillStyle = f.color;
      ctx.fillRect(0, 0, W, H);
      ctx.globalAlpha = 1;
    }
    ctx.restore();
  }
}

// ---- sprites with a flat tint (hit flash), cached per image+color ----

const tintCache = new Map<HTMLImageElement, Map<string, HTMLCanvasElement>>();

function tinted(img: HTMLImageElement, color: string) {
  let m = tintCache.get(img);
  if (!m) tintCache.set(img, (m = new Map()));
  let c = m.get(color);
  if (!c) {
    c = document.createElement('canvas');
    c.width = img.width;
    c.height = img.height;
    const x = c.getContext('2d')!;
    x.drawImage(img, 0, 0);
    x.globalCompositeOperation = 'source-atop';
    x.fillStyle = color;
    x.fillRect(0, 0, c.width, c.height);
    m.set(color, c);
  }
  return c;
}

/** Bottom-center anchored sprite; tintAlpha 1 = full silhouette (enemy hit), <1 = color wash (hero hurt). */
export function drawSprite(ctx: CanvasRenderingContext2D, img: HTMLImageElement | undefined, x: number, ground: number, scale: number, tint: string | null, tintAlpha: number, alpha: number) {
  if (!img || !img.width || alpha <= 0) return;
  const w = img.width * scale;
  const h = img.height * scale;
  ctx.save();
  ctx.globalAlpha = alpha;
  if (!tint || tintAlpha < 1) ctx.drawImage(img, x - w / 2, ground - h, w, h);
  if (tint) { ctx.globalAlpha = alpha * tintAlpha; ctx.drawImage(tinted(img, tint), x - w / 2, ground - h, w, h); }
  ctx.restore();
}
