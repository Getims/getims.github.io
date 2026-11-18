// Project copy of Modules/build/build.mjs (2026-10-23): the project repo builds on its own, without AI-Lab/Modules.
import { createHash } from 'node:crypto';
import { execFileSync } from 'node:child_process';
import fs from 'node:fs';
import path from 'node:path';
import zlib from 'node:zlib';
import { createRequire } from 'node:module';
import { fileURLToPath, pathToFileURL } from 'node:url';

// npm packages live in the project (cwd), not next to this file.
const projectRequire = createRequire(path.join(process.cwd(), 'package.json'));
const dep = async (name) => (await import(pathToFileURL(projectRequire.resolve(name)).href)).default;
const esbuild = await dep('esbuild');
const ffmpegPath = await dep('ffmpeg-static');
const sharp = await dep('sharp');
const subsetFont = await dep('subset-font');

// Single-file HTML build.
// Project-specific data moved to <project>/Config/build.config.mjs (run from the project folder):
//   export default {
//     src: 'Assets',                 // assets root, relative to project
//     entry: 'Game/main.ts',
//     title: 'My Playable',
//     assets: { bg: { kind: 'image', file: 'UI/bg.jpg', jpeg: 78 }, ... },
//       // kinds: image {width?, jpeg?} | audio {bitrate?, trim?} | video {scale?} | text {mime} | font {glyphs, family}
//     body: '<canvas id="game"></canvas>',  // optional extra markup
//     css: '',                       // optional extra CSS
//   };
// Fonts become @font-face (family = spec.family), everything else goes to window.__A as data URIs.
// usage: node <Modules>/build/build.mjs [--dev]  (--dev: serve Build/index.html on PORT (5173), rebuild on change in Game/, assets and Modules/; no size fail)
// Output: Build/index.html (deliverable only), BuildReport/size-report.txt, Cache/ (compressed assets, safe to delete).
//   env NETWORK=auto|facebook|... (define __NETWORK__ for Build/index.html; default auto)
// Network packages: --all (networks from CFG.networks, default all) or env NETWORKS=facebook,vungle,...
//   -> Build/<network>/<file>.html|.zip; file name from CFG.fileName, '{kb}' -> size in KB,
//      e.g. fileName: 'slk_plb_EH1415i001v001_en_any_{kb}kb'. Per-network contract: Docs/rules/cta_networks.md.
const ROOT = process.cwd();
const CFG = (await import(pathToFileURL(path.join(ROOT, 'Config/build.config.mjs')).href)).default;
const SRC = path.resolve(ROOT, CFG.src || 'Assets');
const ASSETS = CFG.assets;
const CACHE = path.join(ROOT, 'Cache');
const DIST = path.join(ROOT, 'Build');
const REPORT = path.join(ROOT, 'BuildReport');
const LIMIT = 5_000_000;
const NETWORK = process.env.NETWORK || 'auto';
const DEV = process.argv.includes('--dev');

// head: extra markup in <head>; zip: null (single html) | 'root' (index.html) | 'folder' (<name>/<name>.html)
const NETWORKS = {
  facebook: { zip: null, head: '' },
  moloco: { zip: null, head: '' },
  google: {
    zip: 'root',
    head: '<meta name="ad.orientation" content="portrait,landscape"><script src="https://tpc.googlesyndication.com/pagead/gadgets/html5/api/exitapi.js"></script>',
  },
  unityads: { zip: null, head: '<script src="mraid.js"></script>' },
  applovin: { zip: null, head: '' }, // AppLovin injects mraid and ALPlayableAnalytics itself
  vungle: { zip: 'root', head: '' },
  mintegral: { zip: 'folder', head: '' },
};
const PACK = process.env.NETWORKS ? process.env.NETWORKS.split(',').map((s) => s.trim()).filter(Boolean)
  : process.argv.includes('--all') ? (CFG.networks || Object.keys(NETWORKS)) : [];
for (const n of PACK) {
  if (!NETWORKS[n]) { console.error(`Unknown network "${n}". Known: ${Object.keys(NETWORKS).join(', ')}`); process.exit(1); }
}

fs.mkdirSync(CACHE, { recursive: true });
fs.mkdirSync(DIST, { recursive: true });
fs.mkdirSync(REPORT, { recursive: true });

function ffmpeg(args) {
  execFileSync(ffmpegPath, ['-y', '-hide_banner', '-loglevel', 'error', ...args]);
}

function cacheKey(name, spec, srcBuf) {
  return createHash('sha1').update(JSON.stringify(spec)).update(srcBuf).digest('hex').slice(0, 12) + '_' + name;
}

async function compress(name, spec) {
  const srcPath = path.join(SRC, spec.file);
  const srcBuf = fs.readFileSync(srcPath);
  const key = cacheKey(name, spec, srcBuf);
  const ext = { image: spec.jpeg ? 'jpg' : 'png', audio: 'mp3', video: 'mp4', text: 'txt', font: 'woff2' }[spec.kind];
  const out = path.join(CACHE, `${key}.${ext}`);
  const mime = spec.mime || { jpg: 'image/jpeg', png: 'image/png', mp3: 'audio/mpeg', mp4: 'video/mp4', woff2: 'font/woff2' }[ext];

  if (!fs.existsSync(out)) {
    if (spec.kind === 'image') {
      let s = sharp(srcBuf);
      if (spec.width) s = s.resize({ width: spec.width, withoutEnlargement: true });
      const buf = spec.jpeg
        ? await s.jpeg({ quality: spec.jpeg, mozjpeg: true }).toBuffer()
        : await s.png({ palette: true, quality: 90, effort: 10, compressionLevel: 9 }).toBuffer();
      fs.writeFileSync(out, buf.length < srcBuf.length || spec.jpeg ? buf : srcBuf);
    } else if (spec.kind === 'audio') {
      const filters = spec.trim === false ? [] : ['-af', 'silenceremove=start_periods=1:start_threshold=-50dB'];
      ffmpeg(['-i', srcPath, '-map', '0:a', ...filters, '-ac', '1', '-ar', '44100', '-b:a', spec.bitrate || '64k', out]);
    } else if (spec.kind === 'video') {
      ffmpeg(['-i', srcPath, '-an', '-c:v', 'libx264', '-profile:v', 'main', '-pix_fmt', 'yuv420p',
        '-vf', spec.scale || 'scale=640:-2', '-crf', '24', '-preset', 'veryslow', '-movflags', '+faststart', out]);
    } else if (spec.kind === 'font') {
      fs.writeFileSync(out, await subsetFont(srcBuf, spec.glyphs, { targetFormat: 'woff2' }));
    } else {
      fs.copyFileSync(srcPath, out);
    }
  }
  const buf = fs.readFileSync(out);
  return { name, src: srcBuf.length, out: buf.length, mime, buf };
}

const CRC_TABLE = Array.from({ length: 256 }, (_, n) => {
  let c = n;
  for (let k = 0; k < 8; k++) c = c & 1 ? 0xedb88320 ^ (c >>> 1) : c >>> 1;
  return c >>> 0;
});
function crc32(buf) {
  let c = 0xffffffff;
  for (const b of buf) c = CRC_TABLE[(c ^ b) & 0xff] ^ (c >>> 8);
  return (c ^ 0xffffffff) >>> 0;
}

/** Minimal deflate zip. files: [{ name, data: Buffer }]. Fixed timestamp keeps output deterministic. */
function zip(files) {
  const local = [];
  const central = [];
  let offset = 0;
  for (const f of files) {
    const name = Buffer.from(f.name, 'utf8');
    const data = zlib.deflateRawSync(f.data, { level: 9 });
    const crc = crc32(f.data);
    const h = Buffer.alloc(30);
    h.writeUInt32LE(0x04034b50, 0); h.writeUInt16LE(20, 4); h.writeUInt16LE(0x0800, 6); h.writeUInt16LE(8, 8);
    h.writeUInt32LE(0x00210000, 10);
    h.writeUInt32LE(crc, 14); h.writeUInt32LE(data.length, 18); h.writeUInt32LE(f.data.length, 22);
    h.writeUInt16LE(name.length, 26);
    local.push(h, name, data);
    const c = Buffer.alloc(46);
    c.writeUInt32LE(0x02014b50, 0); c.writeUInt16LE(20, 4); c.writeUInt16LE(20, 6); c.writeUInt16LE(0x0800, 8); c.writeUInt16LE(8, 10);
    c.writeUInt32LE(0x00210000, 12);
    c.writeUInt32LE(crc, 16); c.writeUInt32LE(data.length, 20); c.writeUInt32LE(f.data.length, 24);
    c.writeUInt16LE(name.length, 28); c.writeUInt32LE(offset, 42);
    central.push(c, name);
    offset += h.length + name.length + data.length;
  }
  const cd = Buffer.concat(central);
  const end = Buffer.alloc(22);
  end.writeUInt32LE(0x06054b50, 0); end.writeUInt16LE(files.length, 8); end.writeUInt16LE(files.length, 10);
  end.writeUInt32LE(cd.length, 12); end.writeUInt32LE(offset, 16);
  return Buffer.concat([...local, cd, end]);
}

/** Writes Build/<network>/<name>.html|.zip; '{kb}' in the name becomes the final file size. */
function writePackage(net, htmlBuf) {
  const dir = path.join(DIST, net);
  fs.rmSync(dir, { recursive: true, force: true });
  fs.mkdirSync(dir, { recursive: true });
  const tpl = CFG.fileName || 'playable_{kb}kb';
  const mode = NETWORKS[net].zip;
  // in 'folder' mode the name is inside the zip, so the size depends on it: iterate until stable
  let name = tpl.replace('{kb}', '0');
  let data;
  for (let i = 0; i < 4; i++) {
    data = !mode ? htmlBuf
      : mode === 'root' ? zip([{ name: 'index.html', data: htmlBuf }])
      : zip([{ name: `${name}/${name}.html`, data: htmlBuf }]);
    const next = tpl.replace('{kb}', String(Math.round(data.length / 1024)));
    if (next === name) break;
    name = next;
  }
  const file = path.join(dir, `${name}.${mode ? 'zip' : 'html'}`);
  fs.writeFileSync(file, data);
  return { file, size: data.length };
}


async function buildAll() {
  const results = [];
  for (const [name, spec] of Object.entries(ASSETS)) results.push(await compress(name, spec));

  const assetMap = {};
  let fontCss = '';
  for (const r of results) {
    const spec = ASSETS[r.name];
    if (spec.kind === 'font') fontCss += `@font-face{font-family:${spec.family || r.name};src:url(data:font/woff2;base64,${r.buf.toString('base64')}) format('woff2')}`;
    else assetMap[r.name] = `data:${r.mime};base64,${r.buf.toString('base64')}`;
  }

  const { html, js } = await page(NETWORK, assetMap, fontCss);
  fs.writeFileSync(path.join(DIST, 'index.html'), html);

  const kb = (n) => (n / 1024).toFixed(1).padStart(8);
  const lines = ['asset'.padEnd(14) + '     src KB   out KB  b64 KB'];
  let rawTotal = 0;
  for (const r of results) {
    rawTotal += r.out;
    lines.push(r.name.padEnd(14) + kb(r.src) + ' ' + kb(r.out) + ' ' + kb(Math.ceil(r.out / 3) * 4));
  }
  lines.push('js bundle'.padEnd(14) + '        -' + ' ' + kb(js.length) + ' ' + kb(js.length));
  lines.push(`raw assets total: ${kb(rawTotal)} KB`);
  lines.push(`Build/index.html: ${kb(html.length)} KB  (${html.length} bytes, limit ${LIMIT})`);
  const report = lines.join('\n');
  fs.writeFileSync(path.join(REPORT, 'size-report.txt'), report + '\n');
  console.log(report);

  const external = html.match(/https?:\/\/[^\s"'`)]+/g) || [];
  const allowed = external.filter((u) => !/apps\.apple\.com|play\.google\.com|www\.w3\.org|tpc\.googlesyndication\.com/.test(u));
  if (allowed.length) console.warn('WARNING: external URLs in output:', [...new Set(allowed)].slice(0, 10));

  let fail = html.length > LIMIT;
  if (fail) console.error(`FAIL: Build/index.html is ${html.length} bytes, over the ${LIMIT} limit`);

  if (PACK.length) {
    const out = ['', 'network packages:'];
    for (const net of PACK) {
      const r = writePackage(net, Buffer.from((await page(net, assetMap, fontCss)).html, 'utf8'));
      const over = r.size > LIMIT;
      fail ||= over;
      out.push(`  ${net.padEnd(10)}${kb(r.size)} KB  ${path.relative(ROOT, r.file)}${over ? '  FAIL: over limit' : ''}`);
    }
    console.log(out.join('\n'));
    fs.appendFileSync(path.join(REPORT, 'size-report.txt'), out.join('\n') + '\n');
  }

  if (fail && !DEV) process.exit(1);
}

async function page(network, assetMap, fontCss) {
  const bundle = await esbuild.build({
    entryPoints: [path.join(ROOT, CFG.entry || 'Game/main.ts')],
    bundle: true,
    minify: true,
    format: 'iife',
    target: 'es2017',
    write: false,
    legalComments: 'none',
    nodePaths: [path.join(ROOT, 'node_modules')], // Modules/*.ts import npm packages installed in the project
    define: { __NETWORK__: JSON.stringify(network) },
  });
  const js = bundle.outputFiles[0].text.replace(/<\/script/gi, '<\\/script');

  const networkHead = NETWORKS[network]?.head || '';
  const html = `<!DOCTYPE html><html lang="en"><head><meta charset="utf-8">
  <meta name="viewport" content="width=device-width,initial-scale=1,maximum-scale=1,user-scalable=no,viewport-fit=cover">
  <title>${CFG.title || 'Playable'}</title>${networkHead}
  <style>${fontCss}
  html,body{margin:0;width:100%;height:100%;overflow:hidden;background:#000;touch-action:none;-webkit-user-select:none;user-select:none;-webkit-touch-callout:none;-webkit-tap-highlight-color:transparent}
  canvas{position:fixed;left:0;top:0;display:block}#spine{pointer-events:none}
  video{position:fixed;left:0;top:0;width:1px;height:1px;opacity:0;pointer-events:none}${CFG.css || ''}</style></head>
  <body>${CFG.body ?? '<canvas id="game"></canvas>'}
  <script>window.__A=${JSON.stringify(assetMap)};</script>
  <script>${js}</script></body></html>`;

  return { html, js };
}

await buildAll();

if (DEV) {
  const http = await import('node:http');
  const port = +(process.env.PORT || 5173);
  http.createServer((req, res) => {
    res.writeHead(200, { 'Content-Type': 'text/html', 'Cache-Control': 'no-store' });
    res.end(fs.readFileSync(path.join(DIST, 'index.html')));
  }).listen(port, () => console.log(`dev server: http://localhost:${port}`));
  let timer = null;
  const rebuild = () => { clearTimeout(timer); timer = setTimeout(() => buildAll().catch((e) => console.error(e.message)), 150); };
  const MODULES = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
  for (const dir of [path.dirname(path.join(ROOT, CFG.entry || 'Game/main.ts')), SRC, MODULES]) if (fs.existsSync(dir)) fs.watch(dir, { recursive: true }, rebuild);
}
