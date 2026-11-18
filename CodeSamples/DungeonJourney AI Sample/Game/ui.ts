// DOM helpers and the shared UI theme. Stage coordinates are 1920×1080 px; layouts follow the original's screenshots.
import { assetUrl } from './core/assets';
import { DATA } from './data';
import type { Rarity } from './data/types';

const CSS = `
#ui{font-family:Cinzel,serif}
.screen{position:absolute;inset:0}
.abs{position:absolute}
.cx{position:absolute;left:0;right:0;text-align:center;font-weight:700}
.gold{font-family:Cinzel,serif;font-weight:700;background:linear-gradient(#fff1a8 10%,#f4c64a 45%,#c98a1c 85%);-webkit-background-clip:text;background-clip:text;color:transparent;filter:drop-shadow(0 4px 6px #000)}
.mono{font-family:Mono,'Courier New',monospace}
.ibtn{position:absolute;border:0;padding:0;background:center/100% 100% no-repeat;font-family:Cinzel,serif;font-weight:700;color:#f3e3c0;text-shadow:0 2px 3px #000;cursor:pointer;display:flex;align-items:center;justify-content:center;flex-direction:column;line-height:1}
.ibtn:hover:not(:disabled){filter:brightness(1.18)}
.ibtn:disabled{filter:grayscale(.6) brightness(.55);cursor:default}
.tbtn{position:absolute;border:0;background:none;font-family:Cinzel,serif;font-weight:700;cursor:pointer;padding:0}
.tbtn:hover{filter:brightness(1.25)}
.box{position:absolute;box-sizing:border-box;font-family:Cinzel,serif;font-weight:700;cursor:pointer;display:flex;align-items:center;justify-content:center}
.box:disabled{opacity:.45;cursor:default}
.overlay{position:absolute;inset:0;z-index:20;background:#0009;-webkit-backdrop-filter:blur(7px);backdrop-filter:blur(7px)}
.stub{position:absolute;bottom:12px;right:16px;font-size:18px;color:#888}
`;

export function injectCss() {
  const s = document.createElement('style');
  s.textContent = CSS;
  document.head.appendChild(s);
}

export function css(text: string) {
  const s = document.createElement('style');
  s.textContent = text;
  document.head.appendChild(s);
}

export function el<K extends keyof HTMLElementTagNameMap>(tag: K, cls = '', html = '', parent?: HTMLElement): HTMLElementTagNameMap[K] {
  const e = document.createElement(tag);
  if (cls) e.className = cls;
  if (html) e.innerHTML = html;
  parent?.appendChild(e);
  return e;
}

/** Absolute box: x, y = left/top in stage px. */
export function place(e: HTMLElement, x: number, y: number, w?: number, h?: number) {
  e.style.left = `${x}px`;
  e.style.top = `${y}px`;
  if (w !== undefined) e.style.width = `${w}px`;
  if (h !== undefined) e.style.height = `${h}px`;
  return e;
}

export function button(parent: HTMLElement, label: string, onClick: () => void, cls = 'box', id = ''): HTMLButtonElement {
  const b = el('button', cls, label, parent);
  if (id) b.dataset.id = id;
  b.addEventListener('click', onClick);
  return b;
}

/** Button drawn with a cut sprite (frame) and our text on top, centered at (cx, cy). */
export function imgButton(parent: HTMLElement, sprite: string, label: string, cx: number, cy: number, w: number, h: number, fontSize: number, onClick: () => void, id = '') {
  const b = button(parent, label, onClick, 'ibtn', id);
  b.style.backgroundImage = `url(${assetUrl(sprite)})`;
  b.style.fontSize = `${fontSize}px`;
  place(b, cx - w / 2, cy - h / 2, w, h);
  return b;
}

export function img(parent: HTMLElement, key: string, x: number, y: number, w?: number, h?: number, cls = 'abs') {
  const i = el('img', cls, '', parent);
  i.src = assetUrl(key);
  i.draggable = false;
  place(i, x, y, w, h);
  return i;
}

/** Text centered horizontally on the stage at baseline-ish y (center of the line). */
export function line(parent: HTMLElement, html: string, cy: number, size: number, color: string, cls = '') {
  const e = el('div', `cx ${cls}`, html, parent);
  e.style.top = `${cy - size * 0.62}px`;
  e.style.fontSize = `${size}px`;
  if (color) e.style.color = color;
  return e;
}

export function rarityColor(r: Rarity) {
  return DATA.balance.rarities[r].color;
}

/** Draws an image covering the whole stage. */
export function drawCover(ctx: CanvasRenderingContext2D, image: CanvasImageSource | undefined, w: number, h: number) {
  const iw = (image as HTMLImageElement | undefined)?.width || 0;
  const ih = (image as HTMLImageElement | undefined)?.height || 0;
  if (!image || !iw) { ctx.fillStyle = '#0b0a10'; ctx.fillRect(0, 0, w, h); return; }
  const k = Math.max(w / iw, h / ih);
  ctx.drawImage(image, (w - iw * k) / 2, (h - ih * k) / 2, iw * k, ih * k);
}
