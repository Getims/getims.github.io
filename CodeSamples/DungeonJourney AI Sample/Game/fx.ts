// VFX layer: a canvas over the whole UI (battle and menus share it) + screen fades. Subscribes to the bus;
// battle logic knows nothing about it. Particles: pooled, capped, halved in Fast Replay (and live 3x shorter there:
// the loop's dt is scaled).
import { FOE_X, GROUND, HERO_X } from './battle/presenter';
import { Particles, type BurstOpts } from './core/particles';
import { W } from './core/stage';
import { bus } from './events';
import { game } from './game';

let canvas: HTMLCanvasElement;
let ctx: CanvasRenderingContext2D;
let k = 1;
let fade: HTMLDivElement;
const parts = new Particles(400);

const FOE_Y = GROUND - 170;
const HERO_POS = { x: HERO_X + 60, y: GROUND - 150 };

function resize() {
  const main = game.stage.canvas;
  canvas.width = main.width;
  canvas.height = main.height;
  k = main.width / W;
}

/** Burst in stage px; counts are halved in Fast Replay. */
export function burst(x: number, y: number, o: BurstOpts) {
  parts.burst(x, y, game.fast ? { ...o, count: Math.ceil(o.count / 2) } : o);
}

/** Reward: ring of 8 sparks in the rarity color + a soft puff (original: chest reward screen). */
export function sparkRing(x: number, y: number, color: string) {
  burst(x, y, { count: 8, color, shape: 'spark', ring: true, speed: [380, 380], life: [0.7, 0.8], size: [34, 40], drag: 3, spread: 60 });
  burst(x, y, { count: 24, color, speed: [100, 420], life: [0.4, 0.9], size: [14, 28], drag: 3 });
}

/** Merge: golden sparks rising around the item. */
export function goldSparks(x: number, y: number) {
  burst(x, y, { count: 40, color: '#ffd23a', shape: 'spark', speed: [150, 520], life: [0.6, 1.2], size: [16, 30], gravity: -160, drag: 2.5, spread: 90 });
  burst(x, y, { count: 30, color: '#ffb020', speed: [40, 160], life: [0.8, 1.5], size: [10, 22], gravity: -220, drag: 1, spread: 140 });
}

/** Floating text in the UI (◆ +N, LV +1): rises and fades. */
export function floatText(parent: HTMLElement, text: string, x: number, y: number, color: string) {
  const e = document.createElement('div');
  e.className = 'fx-float';
  e.textContent = text;
  Object.assign(e.style, { left: `${x}px`, top: `${y}px`, color });
  parent.appendChild(e);
  e.addEventListener('animationend', () => e.remove());
}

export function initFx() {
  const st = document.createElement('style');
  st.textContent = `
#fx{pointer-events:none;z-index:40}
#fade{position:absolute;inset:0;background:#000;pointer-events:none;z-index:41;opacity:0}
#fade.on{animation:fadein .28s ease-out forwards}
@keyframes fadein{0%{opacity:1}100%{opacity:0}}
.fx-float{position:absolute;transform:translate(-50%,-50%);font-family:Cinzel,serif;font-weight:700;font-size:44px;white-space:nowrap;pointer-events:none;z-index:30;text-shadow:0 0 6px #000,0 3px 3px #000;animation:fxfloat 1.1s ease-out forwards}
@keyframes fxfloat{0%{opacity:0;margin-top:20px;transform:translate(-50%,-50%) scale(.6)}15%{opacity:1;transform:translate(-50%,-50%) scale(1.15)}30%{transform:translate(-50%,-50%) scale(1)}100%{opacity:0;margin-top:-90px}}`;
  document.head.appendChild(st);
  canvas = document.createElement('canvas');
  canvas.id = 'fx';
  game.stage.el.appendChild(canvas);
  ctx = canvas.getContext('2d')!;
  fade = document.createElement('div');
  fade.id = 'fade';
  game.stage.el.appendChild(fade);
  resize();
  addEventListener('resize', resize);

  // screen change: the new screen fades in from black (switching stays synchronous for the game and tests)
  bus.on('screen', () => {
    fade.classList.remove('on');
    void fade.offsetWidth; // restart the animation
    fade.classList.add('on');
  });

  // battle
  bus.on('hit', ({ from, crit, exec }) => {
    if (from === 'hero') {
      const color = exec ? '#ff3030' : crit ? '#ffd23a' : '#fff4d0';
      burst(FOE_X - 60, FOE_Y, { count: exec ? 30 : crit ? 22 : 12, color, shape: 'spark', speed: [250, exec || crit ? 900 : 600], life: [0.2, 0.45], size: [14, crit || exec ? 34 : 24], angle: [-2.4, 2.4], drag: 5, spread: 20 });
    } else {
      // original: red dots around the hero when hit
      burst(HERO_POS.x, HERO_POS.y, { count: crit || exec ? 24 : 14, color: '#ff2a2a', speed: [120, 420], life: [0.35, 0.7], size: [10, 22], gravity: 500, drag: 2, spread: 50 });
    }
  });
  bus.on('thorns', ({ from }) => {
    const p = from === 'hero' ? { x: FOE_X - 40, y: FOE_Y } : HERO_POS;
    burst(p.x, p.y, { count: 8, color: '#ff9a3c', shape: 'spark', speed: [200, 450], life: [0.2, 0.4], size: [12, 20], drag: 5 });
  });
  bus.on('heal', ({ side }) => {
    const p = side === 'hero' ? HERO_POS : { x: FOE_X, y: FOE_Y };
    burst(p.x, p.y + 60, { count: 12, color: '#5fe36a', speed: [20, 80], life: [0.6, 1], size: [12, 22], gravity: -260, drag: 1, spread: 70 });
  });
  bus.on('foe_death', () => {
    burst(FOE_X, FOE_Y + 40, { count: 26, color: '#c8c0d8', speed: [60, 300], life: [0.4, 0.9], size: [22, 46], gravity: -120, drag: 3, spread: 70 });
    burst(FOE_X, FOE_Y + 40, { count: 10, color: '#ffffff', shape: 'spark', speed: [300, 600], life: [0.2, 0.4], size: [16, 26], drag: 5 });
  });
  bus.on('hero_death', () => burst(HERO_POS.x, HERO_POS.y, { count: 40, color: '#ff2020', speed: [150, 600], life: [0.5, 1], size: [12, 28], gravity: 600, drag: 1.5, spread: 40 }));
}

export function updateFx(dt: number) { parts.update(dt); }

let drawn = false;
export function renderFx() {
  if (!parts.count && !drawn) return;
  ctx.setTransform(1, 0, 0, 1, 0, 0);
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  parts.draw(ctx, k);
  drawn = parts.count > 0;
}
