// Project copy of Modules/test/test.mjs (2026-10-23): the project repo tests on its own, without AI-Lab/Modules.
// Playwright autotest for a single-file playable.
// Generic part kept here: size limit, viewport matrix, screenshots, console errors, external requests.
// Game flow (taps, state checks) moved to optional <project>/Config/test.config.mjs:
//   export default {
//     ready: () => window.__game && window.__game.state !== 'loading',   // evaluated in page
//     viewports: [[390, 844], ...],                                       // optional
//     async perViewport({ page, w, h, tag, check, shot, sleep }) { ... }, // optional, runs per viewport
//     async flows({ open, check, shot, sleep }) { ... },                  // optional, extra scenarios
//   };
// usage (from the project folder): node <Modules>/test/test.mjs   env PW_CHANNEL=chrome
import fs from 'node:fs';
import path from 'node:path';
import { createRequire } from 'node:module';
import { pathToFileURL } from 'node:url';

// playwright lives in the project (cwd), not next to this file.
const pw = await import(pathToFileURL(createRequire(path.join(process.cwd(), 'package.json')).resolve('playwright')).href);
const { chromium } = pw.chromium ? pw : pw.default;

const ROOT = process.cwd();
const HTML = path.join(ROOT, 'Build/index.html');
const OUT = path.join(ROOT, 'BuildReport/Screenshots');
fs.mkdirSync(OUT, { recursive: true });
const url = pathToFileURL(HTML).href;
const cfgPath = path.join(ROOT, 'Config/test.config.mjs');
const CFG = fs.existsSync(cfgPath) ? (await import(pathToFileURL(cfgPath).href)).default : {};

const VIEWPORTS = CFG.viewports || [
  [390, 844], [360, 800], [412, 915], [844, 390], [1920, 1080], [2400, 1080], [768, 1024], [1024, 768], [1200, 1200],
];

const browser = await chromium.launch({ channel: process.env.PW_CHANNEL || 'chrome', args: ['--autoplay-policy=no-user-gesture-required'] });
const results = [];
const check = (name, ok, info = '') => { results.push({ name, ok, info }); console.log(`${ok ? 'PASS' : 'FAIL'}  ${name}${info ? '  — ' + info : ''}`); };
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
const shot = (page, name) => page.screenshot({ path: path.join(OUT, `${name}.png`) });

async function open(w, h, query = '') {
  const page = await browser.newPage({ viewport: { width: w, height: h } });
  const errors = [];
  const external = [];
  page.on('pageerror', (e) => errors.push(String(e)));
  page.on('console', (m) => { if (m.type() === 'error') errors.push(m.text()); });
  page.on('request', (r) => { const u = r.url(); if (/^https?:/.test(u)) external.push(u); });
  await page.goto(url + query);
  if (CFG.ready) await page.waitForFunction(CFG.ready, null, { timeout: 15000 });
  else await sleep(1000);
  return { page, errors, external };
}

const size = fs.statSync(HTML).size;
check('Build/index.html <= 5 MB', size <= 5_000_000, `${(size / 1024 / 1024).toFixed(2)} MB`);

for (const [w, h] of VIEWPORTS) {
  const tag = `${w}x${h}`;
  const { page, errors, external } = await open(w, h);
  await sleep(500);
  await shot(page, `${tag}_1_start`);
  if (CFG.perViewport) await CFG.perViewport({ page, w, h, tag, check, sleep, shot: (n) => shot(page, `${tag}_${n}`) });
  check(`${tag}: no errors`, errors.length === 0, errors.slice(0, 3).join(' | '));
  check(`${tag}: no external requests`, external.length === 0, external.slice(0, 3).join(' | '));
  await page.close();
}

if (CFG.flows) await CFG.flows({ open, check, sleep, shot });

await browser.close();
const failed = results.filter((r) => !r.ok);
console.log(`\n${results.length - failed.length}/${results.length} checks passed. Screens: BuildReport/Screenshots/`);
process.exit(failed.length ? 1 : 0);
