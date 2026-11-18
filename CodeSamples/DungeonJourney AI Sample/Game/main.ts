// Entry: stage, loop, screens, data check, debug panel; window.__game for tests.
import { loadAssets } from './core/assets';
import { Loop } from './core/loop';
import { Screens } from './core/screens';
import { Stage } from './core/stage';
import { checkData, DATA } from './data';
import { debugEnabled, initDebugPanel } from './debug/panel';
import { bus } from './events';
import { game } from './game';
import { initFx, renderFx, updateFx } from './fx';
import { platform } from './platform';
import { simulateFloor } from './battle/core';
import { Presenter } from './battle/presenter';
import { heroEffects, heroStats } from './run';
import { debugEndFloor } from './screens/battle';
import { registerScreens } from './screens/menus';
import { load, initAutosave, resetSave, save } from './save';
import { initSound, soundLog, syncSound } from './sound';
import { injectCss } from './ui';
import { openBuild, openSettings } from './overlays';

declare global { interface Window { __game: unknown } }

async function boot() {
  injectCss();
  game.stage = new Stage();
  game.screens = new Screens(game.stage.ui);
  game.loop = new Loop(
    (dt) => { game.tweens.update(dt); game.screens.update(dt); updateFx(dt); syncSound(); },
    () => {
      game.loop.timeScale = game.debug.speed * (game.fast ? DATA.timing.fastScale : 1);
      game.screens.render(game.stage.ctx);
      renderFx();
    },
  );
  window.__game = {
    get state() { return game.screens.current; },
    go: (id: never, p?: unknown) => game.screens.go(id, p),
    floor: (n: number) => DATA.floors.floor(n),
    data: DATA,
    endFloor: debugEndFloor,
    save,
    openSettings,
    /** Debug: presenter class (frame-by-frame checks of battle animation). */
    Presenter,
    openBuild,
    stats: () => heroStats(game.run || { perks: [] }),
    resetSave,
    /** Played sound keys (tests). */
    sounds: soundLog,
    /** Headless: play floors from..to with the given perks; returns the floor of death (or to+1). */
    simRun(perks: string[], to = 30, seed = 'sim') {
      let hp = heroStats({ perks }).maxHp;
      let killAtk = 0;
      for (let n = 1; n <= to; n++) {
        const r = simulateFloor({ hero: { stats: heroStats({ perks, killAtk }), hp, effects: heroEffects({ perks }) }, foes: DATA.floors.floor(n).nodes[0].foes, seed: `${seed}:${n}` });
        if (!r.cleared) return { died: n, log: r.log.length };
        hp = r.heroHp;
        killAtk += r.atkGained;
      }
      return { died: to + 1 };
    },
    game,
    /** Center of a [data-id] button in page CSS px (for tests). */
    target(id: string) {
      const e = document.querySelector(`[data-id="${id}"]`);
      if (!e) return null;
      const r = e.getBoundingClientRect();
      return { x: r.left + r.width / 2, y: r.top + r.height / 2 };
    },
  };

  await platform.init();
  await loadAssets();
  checkData();
  const saveState = load();
  if (saveState === 'reset') console.warn('save was broken: backed up and reset');
  initAutosave();
  await initSound();
  initFx();
  registerScreens();
  if (debugEnabled()) {
    const panel = initDebugPanel();
    bus.on('screen', () => panel.refresh());
  }
  game.loop.start();
  game.screens.go('menu');
}

boot().catch((e) => console.error(e));
