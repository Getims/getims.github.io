// Project version of Modules/particles.ts: the module needs an atlas image and allocates a particle per spawn.
// Here: fixed pool with a hard cap (weak mobiles), procedural sprites (soft dot / 4-point spark) baked per color,
// gravity. Coordinates are stage px (1920×1080).

export type Shape = 'dot' | 'spark';

export interface BurstOpts {
  count: number;
  color: string;
  shape?: Shape;
  speed: [number, number];   // px/s
  life: [number, number];    // s
  size: [number, number];    // px
  gravity?: number;          // px/s², negative = floats up
  drag?: number;             // velocity damping per second
  spread?: number;           // spawn radius px
  angle?: [number, number];  // emission angle range, radians (default full circle)
  ring?: boolean;            // even angles (sparkle ring)
}

interface P {
  alive: boolean; x: number; y: number; vx: number; vy: number;
  t: number; life: number; size: number; g: number; drag: number; rot: number; spin: number; img: HTMLCanvasElement;
}

const rnd = (r: [number, number]) => r[0] + Math.random() * (r[1] - r[0]);

const sprites = new Map<string, HTMLCanvasElement>();
function sprite(shape: Shape, color: string) {
  const key = `${shape}:${color}`;
  let c = sprites.get(key);
  if (c) return c;
  c = document.createElement('canvas');
  c.width = c.height = 32;
  const x = c.getContext('2d')!;
  if (shape === 'dot') {
    const g = x.createRadialGradient(16, 16, 0, 16, 16, 16);
    g.addColorStop(0, '#fff');
    g.addColorStop(0.25, color);
    g.addColorStop(1, 'transparent');
    x.fillStyle = g;
    x.fillRect(0, 0, 32, 32);
  } else {
    x.fillStyle = color;
    x.beginPath();
    for (let i = 0; i < 8; i++) {
      const a = (i / 8) * Math.PI * 2, r = i % 2 ? 3 : 16;
      x.lineTo(16 + Math.cos(a) * r, 16 + Math.sin(a) * r);
    }
    x.fill();
    x.fillStyle = '#fff';
    x.beginPath();
    x.arc(16, 16, 3, 0, Math.PI * 2);
    x.fill();
  }
  sprites.set(key, c);
  return c;
}

export class Particles {
  private pool: P[];
  private live = 0;

  constructor(readonly cap = 400) {
    this.pool = Array.from({ length: cap }, () => ({ alive: false, x: 0, y: 0, vx: 0, vy: 0, t: 0, life: 1, size: 1, g: 0, drag: 0, rot: 0, spin: 0, img: null as unknown as HTMLCanvasElement }));
  }

  get count() { return this.live; }

  /** Spawns up to o.count particles; when the pool is full the rest are dropped. */
  burst(x: number, y: number, o: BurstOpts) {
    const img = sprite(o.shape || 'dot', o.color);
    const [a0, a1] = o.angle || [0, Math.PI * 2];
    let n = o.count;
    for (const p of this.pool) {
      if (n <= 0) break;
      if (p.alive) continue;
      const i = o.count - n--;
      const a = o.ring ? a0 + ((a1 - a0) * i) / o.count : a0 + Math.random() * (a1 - a0);
      const sp = rnd(o.speed), r = Math.random() * (o.spread || 0);
      Object.assign(p, {
        alive: true, x: x + Math.cos(a) * r, y: y + Math.sin(a) * r, vx: Math.cos(a) * sp, vy: Math.sin(a) * sp,
        t: 0, life: rnd(o.life), size: rnd(o.size), g: o.gravity || 0, drag: o.drag ?? 2,
        rot: Math.random() * 6.28, spin: (Math.random() - 0.5) * 8, img,
      });
      this.live++;
    }
  }

  update(dt: number) {
    if (!this.live) return;
    for (const p of this.pool) {
      if (!p.alive) continue;
      if ((p.t += dt) >= p.life) { p.alive = false; this.live--; continue; }
      const k = Math.exp(-p.drag * dt);
      p.vx *= k; p.vy = p.vy * k + p.g * dt;
      p.x += p.vx * dt; p.y += p.vy * dt;
      p.rot += p.spin * dt;
    }
  }

  /** k = canvas px per stage px (the context's base scale). */
  draw(ctx: CanvasRenderingContext2D, k: number) {
    if (!this.live) return;
    ctx.save();
    ctx.globalCompositeOperation = 'lighter';
    for (const p of this.pool) {
      if (!p.alive) continue;
      const f = p.t / p.life;
      ctx.globalAlpha = f > 0.6 ? (1 - f) / 0.4 : 1;
      const s = p.size * (f < 0.1 ? f / 0.1 : 1 - (f - 0.1) * 0.5);
      const c = Math.cos(p.rot) * k, n = Math.sin(p.rot) * k;
      ctx.setTransform(c, n, -n, c, p.x * k, p.y * k);
      ctx.drawImage(p.img, -s / 2, -s / 2, s, s);
    }
    ctx.restore();
  }
}
