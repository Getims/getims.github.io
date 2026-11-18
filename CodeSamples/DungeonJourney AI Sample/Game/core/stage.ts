// Logical scene 1920×1080 with letterbox; canvas (battle) and DOM #ui share one transform.
export const W = 1920;
export const H = 1080;

export class Stage {
  readonly el = document.getElementById('stage') as HTMLDivElement;
  readonly canvas = document.getElementById('game') as HTMLCanvasElement;
  readonly ui = document.getElementById('ui') as HTMLDivElement;
  readonly ctx = this.canvas.getContext('2d')!;
  private rotate = document.getElementById('rotate') as HTMLDivElement;
  scale = 1;
  portrait = false;

  constructor() {
    this.resize();
    addEventListener('resize', () => this.resize());
  }

  resize() {
    const vw = innerWidth;
    const vh = innerHeight;
    const s = Math.min(vw / W, vh / H);
    this.scale = s;
    this.el.style.transform = `translate(${(vw - W * s) / 2}px,${(vh - H * s) / 2}px) scale(${s})`;
    const k = Math.min(2, s * (devicePixelRatio || 1));
    this.canvas.width = Math.round(W * k);
    this.canvas.height = Math.round(H * k);
    this.ctx.setTransform(k, 0, 0, k, 0, 0);
    this.portrait = vh > vw; // portrait layout: stage 9 (Poki)
    this.rotate.style.display = this.portrait ? 'flex' : 'none';
  }

  /** Stage coordinates -> CSS px of the page (for tests). */
  toPage(x: number, y: number) {
    const r = this.el.getBoundingClientRect();
    return { x: r.left + x * this.scale, y: r.top + y * this.scale };
  }
}
