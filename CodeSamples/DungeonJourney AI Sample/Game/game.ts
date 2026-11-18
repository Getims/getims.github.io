// Game-wide singletons and state.
import { Tweens } from './core/tween';
import { Loop } from './core/loop';
import { Screens } from './core/screens';
import { Stage } from './core/stage';
import { newMeta, type MetaState } from './meta';
import type { RunState } from './run';

export const game = {
  stage: null as unknown as Stage,
  loop: null as unknown as Loop,
  screens: null as unknown as Screens,
  tweens: new Tweens(),
  run: null as RunState | null,
  meta: newMeta() as MetaState,
  get settings() { return this.meta.settings; },
  /** Screen Shake OFF also tones down full-screen flashes. */
  get flashK() { return this.meta.settings.shake ? 1 : 0.3; },
  /** Fast Replay is active right now (set by the battle screen). */
  fast: false,
  debug: { god: false, speed: 1 },
};
