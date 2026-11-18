// Typed event bus: game logic emits, VFX/sound/UI subscribe.
export class Bus<M extends object> {
  private handlers: { [K in keyof M]?: ((p: M[K]) => void)[] } = {};

  on<K extends keyof M>(key: K, fn: (p: M[K]) => void): () => void {
    (this.handlers[key] ||= []).push(fn);
    return () => this.off(key, fn);
  }

  off<K extends keyof M>(key: K, fn: (p: M[K]) => void) {
    const list = this.handlers[key];
    if (list) this.handlers[key] = list.filter((f) => f !== fn);
  }

  emit<K extends keyof M>(key: K, payload: M[K]) {
    const list = this.handlers[key];
    if (list) for (const fn of list.slice()) fn(payload);
  }
}
