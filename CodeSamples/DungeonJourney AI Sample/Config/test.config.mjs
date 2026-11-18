// Config for Tools/test/test.mjs (project copy of Modules/test/test.mjs) (run from the project folder after build).
// Stage filter: `npm test -- stage3` runs only that block.
const STAGE = process.argv.find((a) => /^stage\d+$/.test(a)) || 'all';
const on = (s) => STAGE === 'all' || STAGE === s;

export default {
  ready: () => window.__game && window.__game.state !== 'loading',

  viewports: STAGE === 'all' ? undefined : [[1920, 1080], [844, 390]],

  async perViewport({ page, w, h, check }) {
    const t = await page.evaluate(() => window.__game.target('play'));
    check(`${w}x${h}: PLAY on screen`, !!t && t.x > 0 && t.x < w && t.y > 0 && t.y < h);
  },

  async flows({ open, check, shot, sleep }) {
    if (!on('stage3')) return;
    const { page, errors } = await open(1280, 720);
    const ev = (fn, arg) => page.evaluate(fn, arg);
    const tap = async (id) => {
      const p = await ev((i) => window.__game.target(i), id);
      if (p) { await page.mouse.click(p.x, p.y); await sleep(150); }
      return !!p;
    };
    const state = () => ev(() => window.__game.state);
    const waitState = async (want, ms = 15000) => {
      for (let t = 0; t < ms; t += 100) { if (want.includes(await state())) return true; await sleep(100); }
      return false;
    };

    // 3.2 data + 3.3 core
    const d = await ev(() => {
      const g = window.__game;
      return { errors: g.data.errors, f10: g.floor(10).nodes[0].foes[0].hp, det: JSON.stringify(g.simRun(['brutal'], 8, 'x')) === JSON.stringify(g.simRun(['brutal'], 8, 'x')), bare: g.simRun([], 40, 's1').died };
    });
    check('stage3: data valid, F10 Brute 277 (257 + hpAdd 20)', d.errors.length === 0 && d.f10 === 277, d.errors.slice(0, 3).join(' | '));
    check('stage3: battle core deterministic', d.det);
    check('stage3: hero without perks dies around F4–F9', d.bare >= 4 && d.bare <= 9, `died on F${d.bare}`);

    // 3.4 path cards: spin, then uncommon cards
    await ev(() => { window.__game.game.debug.speed = 10; });
    await tap('play');
    await sleep(600);
    const path = await ev(() => [...document.querySelectorAll('.pcard .r')].map((e) => e.textContent));
    check('stage3: path shows 3 UNCOMMON cards after spin', path.length === 3 && path.every((r) => r === 'UNCOMMON'), path.join(','));
    await tap('card0');
    check('stage3: path → battle F1 with 1 perk', (await state()) === 'battle' && (await ev(() => window.__game.game.run.perks.length)) === 1);

    // 3.3/3.5 auto battle F1 → victory → choose
    check('stage3: F1 played to choose', await waitState(['choose']));
    const keys = await ev(() => window.__game.game.run.keysGained);
    await sleep(600);
    await shot(page, 'stage3_choose');
    const before = await ev(() => document.querySelector('[data-id="card1"] .n').textContent);
    await tap('lock1'); await tap('reroll'); await sleep(600);
    const after = await ev(() => document.querySelector('[data-id="card1"] .n').textContent);
    check('stage3: reroll keeps the locked card', before === after, `${before} / ${after}`);
    await tap('card0');
    const r2 = await ev(() => ({ f: window.__game.game.run.floor, n: window.__game.game.run.perks.length }));
    check('stage3: choose → battle F2, 2 perks', (await state()) === 'battle' && r2.f === 2 && r2.n === 2);
    await sleep(300);
    await shot(page, 'stage3_battle');

    // QUIT from settings = defeat: next start is Continue from N−2
    await ev(() => { const r = window.__game.game.run; r.floor = 5; window.__game.go('battle'); });
    await tap('gear'); await tap('quit');
    const q = await ev(() => ({ s: window.__game.state, dead: window.__game.game.run.dead, quitInHud: !!document.querySelector('[data-id="quit"]') }));
    check('stage3: QUIT in settings → menu, run counts as death', q.s === 'menu' && q.dead && !q.quitInHud, JSON.stringify(q));
    await tap('continue');
    check('stage3: continue after QUIT → F3', (await ev(() => window.__game.game.run.floor)) === 3);

    // 3.6 death → revive (same floor) → death → continue (N−2, fast replay)
    await ev(() => { const r = window.__game.game.run; r.floor = 14; window.__game.go('battle'); });
    check('stage3: F14 without build → died', await waitState(['died']));
    await sleep(400);
    check('stage3: fake rank shown', /#\d+ of/i.test(await ev(() => document.body.innerText)));
    await shot(page, 'stage3_died');
    await tap('revive');
    const rv = await ev(() => ({ s: window.__game.state, f: window.__game.game.run.floor, a: window.__game.game.run.attempt }));
    check('stage3: revive → battle on the same floor', rv.s === 'battle' && rv.f === 14 && rv.a >= 2);
    await waitState(['died']);
    await tap('menu'); await tap('continue');
    const c = await ev(() => ({ s: window.__game.state, f: window.__game.game.run.floor, u: window.__game.game.run.replayUntil }));
    // at ×10 the 1 s descent may already be over
    check('stage3: continue → descending to F12', ['descending', 'battle'].includes(c.s) && c.f === 12 && c.u === 14, JSON.stringify(c));
    await waitState(['battle'], 3000);
    check('stage3: replay floor runs in Fast Replay', await ev(() => window.__game.game.fast));
    check('stage3: keys counted', typeof keys.copper === 'number');
    check('stage3: no errors', errors.length === 0, errors.slice(0, 3).join(' | '));
    await page.close();

    // group C: inventory, chests, merge, upgrade, salvage, saves, build, settings
    const c2 = await open(1280, 720);
    const p2 = c2.page;
    const ev2 = (fn, arg) => p2.evaluate(fn, arg);
    const tap2 = async (id) => {
      const p = await ev2((i) => window.__game.target(i), id);
      if (p) { await p2.mouse.click(p.x, p.y); await sleep(150); }
      return !!p;
    };
    // whole stats object: a chest item may add only crit / lifesteal etc. (atk+hp+def stayed the same → flaky)
    const atk0 = await ev2(() => JSON.stringify(window.__game.stats()));
    await ev2(() => { window.__game.game.meta.keys.copper = 3; window.__game.go('inventory'); });
    await tap2('open_copper');
    await sleep(500); // stage 8: the key animation (0.5 s) plays before the reward modal
    const opened = await ev2(() => ({ n: window.__game.game.meta.items.length, eq: Object.keys(window.__game.game.meta.equipped).length, k: window.__game.game.meta.keys.copper, modal: !!document.querySelector('.rmodal') }));
    check('stage3: copper key opens a chest, item auto-equipped', opened.n === 1 && opened.eq === 1 && opened.k === 2 && opened.modal, JSON.stringify(opened));
    await shot(p2, 'stage3_chest');
    await tap2('r_keep');
    const atk1 = await ev2(() => JSON.stringify(window.__game.stats()));
    check('stage3: equipped item changes hero stats', atk1 !== atk0, `${atk0} → ${atk1}`);
    await tap2('conv_copper');
    check('stage3: 3→1 needs 3 keys (2 left: no-op)', (await ev2(() => window.__game.game.meta.keys.silver)) === 0);

    // merge 3 Leather Caps → uncommon+
    await ev2(() => {
      const m = window.__game.game.meta;
      m.items = []; m.equipped = {};
      for (let i = 0; i < 3; i++) m.items.push({ uid: m.nextUid++, id: 'leather_cap', level: 0 });
      m.shards = 20;
      window.__game.go('inventory');
    });
    await tap2('item0'); await tap2('p_upgrade');
    check('stage3: upgrade for shards (+1 level)', (await ev2(() => window.__game.game.meta.items[0].level)) === 1);
    await tap2('p_merge');
    const merged = await ev2(() => { const m = window.__game.game.meta; return { n: m.items.length, r: window.__game.data.items.get(m.items[0].id).rarity }; });
    check('stage3: merge 3 → 1 of the next rarity', merged.n === 1 && merged.r === 'uncommon', JSON.stringify(merged));
    await tap2('r_keep');
    await tap2('item0'); await tap2('p_salvage');
    const sv = await ev2(() => ({ n: window.__game.game.meta.items.length, s: window.__game.game.meta.shards }));
    check('stage3: salvage → shards', sv.n === 0 && sv.s > 0, JSON.stringify(sv));

    // saves: reload keeps meta + settings; broken save does not crash
    await ev2(() => { const g = window.__game.game; g.meta.best = 7; g.settings.mute = true; window.__game.save(); });
    await p2.reload();
    await p2.waitForFunction(() => window.__game && window.__game.state !== 'loading');
    const kept = await ev2(() => ({ best: window.__game.game.meta.best, mute: window.__game.game.settings.mute, s: window.__game.game.meta.shards }));
    check('stage3: save survives reload', kept.best === 7 && kept.mute === true && kept.s === sv.s, JSON.stringify(kept));
    await ev2(() => localStorage.setItem('dj_save', '{broken'));
    await p2.reload();
    await p2.waitForFunction(() => window.__game && window.__game.state !== 'loading');
    const broken = await ev2(() => ({ s: window.__game.state, best: window.__game.game.meta.best, bak: !!localStorage.getItem('dj_save_bak') }));
    check('stage3: broken save → backup + clean start', broken.s === 'menu' && broken.best === 0 && broken.bak, JSON.stringify(broken));

    // build + settings screens
    await ev2(() => { window.__game.openBuild(); });
    check('stage3: Your Build shows stats', /ATTACK/i.test(await ev2(() => document.querySelector('.overlay').innerText)));
    await tap2('close');
    await ev2(() => { window.__game.openSettings(); });
    const paused = await ev2(() => window.__game.game.loop.paused);
    await tap2('shake_off');
    const shake = await ev2(() => window.__game.game.settings.shake);
    await tap2('back');
    check('stage3: settings overlay pauses, toggles, closes', paused && !shake && !(await ev2(() => window.__game.game.loop.paused)));
    await shot(p2, 'stage3_settings');
    check('stage3: group C no errors', c2.errors.length === 0, c2.errors.slice(0, 3).join(' | '));
    await p2.close();
  },
};
