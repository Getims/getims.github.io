// Blurred, darkened copy of the last battle frame: background of the card screens (as in the original).
// Blur = draw into a tiny canvas and scale back up (works without ctx.filter, e.g. Safari).
import { H, W } from './stage';

const small = document.createElement('canvas');
small.width = Math.round(W / 12);
small.height = Math.round(H / 12);
let ready = false;

export function snapshot(src: CanvasImageSource) {
  const c = small.getContext('2d')!;
  c.imageSmoothingQuality = 'high';
  c.drawImage(src, 0, 0, small.width, small.height);
  ready = true;
}

export function hasBackdrop() {
  return ready;
}

export function drawBackdrop(ctx: CanvasRenderingContext2D, dark = 0.55) {
  ctx.imageSmoothingEnabled = true;
  ctx.imageSmoothingQuality = 'high';
  if (ready) ctx.drawImage(small, 0, 0, W, H);
  else { ctx.fillStyle = '#0b0a10'; ctx.fillRect(0, 0, W, H); }
  ctx.fillStyle = `rgba(5,4,10,${dark})`;
  ctx.fillRect(0, 0, W, H);
}
