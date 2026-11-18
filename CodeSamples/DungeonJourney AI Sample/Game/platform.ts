// Platform layer: stubs now, Poki SDK on stage 9 (only this file changes).
export const platform = {
  async init() {},
  gameplayStart() {},
  gameplayStop() {},
  /** Rewarded ad (Reroll, Revive). Resolves true if the reward is granted. */
  async rewarded(_place: 'reroll' | 'revive'): Promise<boolean> { return true; },
  async commercialBreak(): Promise<void> {},
  storage: {
    get(key: string): string | null { try { return localStorage.getItem(key); } catch { return null; } },
    set(key: string, value: string) { try { localStorage.setItem(key, value); } catch { /* quota / private mode */ } },
    remove(key: string) { try { localStorage.removeItem(key); } catch { /* ignore */ } },
  },
};
