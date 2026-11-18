// Screen state machine: one active screen; DOM root is recreated on every enter.
import { bus } from '../events';

export type ScreenId = 'menu' | 'path' | 'battle' | 'choose' | 'died' | 'descending' | 'inventory';

export interface Screen {
  enter(root: HTMLElement, params: any): void;
  exit?(): void;
  update?(dt: number): void;
  /** Canvas layer (battle scene, backgrounds). */
  render?(ctx: CanvasRenderingContext2D): void;
}

export class Screens {
  current: ScreenId | 'loading' = 'loading';
  private prev: { id: ScreenId; params: any } | null = null;
  private params: any = null;
  private active: Screen | null = null;
  private root: HTMLElement | null = null;
  private reg = new Map<ScreenId, Screen>();

  constructor(private ui: HTMLElement) {}

  add(id: ScreenId, screen: Screen) { this.reg.set(id, screen); }

  go(id: ScreenId, params: any = {}) {
    const next = this.reg.get(id);
    if (!next) throw new Error(`unknown screen ${id}`);
    this.active?.exit?.();
    this.root?.remove();
    if (this.current !== 'loading') this.prev = { id: this.current, params: this.params };
    this.current = id;
    this.params = params;
    this.active = next;
    this.root = document.createElement('div');
    this.root.className = `screen screen-${id}`;
    this.ui.appendChild(this.root);
    next.enter(this.root, params);
    bus.emit('screen', { id });
  }

  /** Back to the previous screen. */
  back(fallback: ScreenId = 'menu') {
    const p = this.prev;
    if (p) this.go(p.id, { ...p.params, resume: true });
    else this.go(fallback);
  }

  update(dt: number) { this.active?.update?.(dt); }
  render(ctx: CanvasRenderingContext2D) { this.active?.render?.(ctx); }
}
