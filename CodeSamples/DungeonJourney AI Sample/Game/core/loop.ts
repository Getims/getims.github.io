// Main loop: real dt clamped, pause, timeScale (Fast Replay ×3, debug ×10).
export class Loop {
  paused = false;
  timeScale = 1;
  private last = 0;

  constructor(private update: (dt: number) => void, private render: () => void) {}

  start() {
    const frame = (now: number) => {
      const dt = this.last ? Math.min(0.1, (now - this.last) / 1000) : 0;
      this.last = now;
      if (!this.paused) this.update(dt * this.timeScale);
      this.render();
      requestAnimationFrame(frame);
    };
    requestAnimationFrame(frame);
    document.addEventListener('visibilitychange', () => { this.last = 0; });
  }
}
