// Dev panel (3.10): floor jump, speed, god mode, floor table, give keys/shards/items, force perk, reset save, battle log.
// Shown on localhost or with ?debug in the URL; toggled with the ` key. Never visible on Poki.
import { DATA } from '../data';
import { KEY_TYPES } from '../data/types';
import { game } from '../game';
import { addItem } from '../meta';
import { heroStats, newRun, pickPerk } from '../run';
import { resetSave, save } from '../save';
import { debugEndFloor, lastBattleLog } from '../screens/battle';

const CSS = `
#dbg{position:fixed;right:8px;top:8px;z-index:50;width:380px;max-height:90vh;overflow:auto;background:#000d;color:#cfc;font:12px/1.4 monospace;border:1px solid #4a4;border-radius:6px;padding:8px}
#dbg button{font:12px monospace;margin:2px;background:#232;color:#cfc;border:1px solid #4a4;border-radius:3px;cursor:pointer}
#dbg button.on{background:#4a4;color:#000}
#dbg input,#dbg select{font:12px monospace;max-width:200px}
#dbg input[type=number]{width:50px}
#dbg table{border-collapse:collapse;width:100%}#dbg td{padding:1px 4px;border-bottom:1px solid #232}
#dbg .err{color:#f66}
#dbg pre{max-height:180px;overflow:auto;background:#111;padding:4px;margin:4px 0;white-space:pre-wrap}
`;

export function debugEnabled() {
  return /[?&]debug/.test(location.search) || location.hostname === 'localhost' || location.hostname === '127.0.0.1';
}

function logText() {
  return lastBattleLog().map((e) => {
    switch (e.t) {
      case 'hit': return `${e.from} hits ${e.dmg}${e.crit ? ' CRIT' : ''}${e.exec ? ' EXEC' : ''} → ${e.hp}`;
      case 'thorns': return `  thorns(${e.from}) ${e.dmg} → ${e.hp}`;
      case 'heal': return `  heal ${e.side} +${e.amount} → ${e.hp}`;
      case 'foe': return `-- foe ${e.i + 1}: ${e.hp} HP`;
      case 'start': return e.heroFirst ? 'STRIKE FIRST' : 'AMBUSH';
      default: return e.t;
    }
  }).join('\n');
}

export function initDebugPanel() {
  const style = document.createElement('style');
  style.textContent = CSS;
  document.head.appendChild(style);
  const box = document.createElement('div');
  box.id = 'dbg';
  document.body.appendChild(box);
  addEventListener('keydown', (e) => { if (e.key === '`') box.hidden = !box.hidden; });

  let floor = 1;
  let showLog = false;
  const render = () => {
    const node = DATA.floors.floor(floor).nodes[0];
    const rows = node.foes.map((f) => `<tr><td>${f.name}</td><td>HP ${f.hp}</td><td>ATK ${f.atk}</td><td>DEF ${f.def}</td><td>${f.abilities.join(',')}</td></tr>`).join('');
    const perkOpts = [...DATA.perks.values()].map((p) => `<option value="${p.id}">${p.name} (${p.rarity[0]})</option>`).join('');
    const itemOpts = [...DATA.items.values()].map((i) => `<option value="${i.id}">${i.name} (${i.rarity[0]})</option>`).join('');
    const m = game.meta;
    box.innerHTML = `
      <b>DEBUG</b> (\` to hide) · screen: ${game.screens.current} · run F${game.run?.floor ?? '-'}<br>
      Floor <input id="dbgF" type="number" min="1" value="${floor}"> <button data-a="go">go to battle</button> <button data-a="end">end floor</button><br>
      Speed ${[1, 3, 10].map((s) => `<button data-s="${s}" class="${game.debug.speed === s ? 'on' : ''}">×${s}</button>`).join('')}
      <button data-a="god" class="${game.debug.god ? 'on' : ''}">god</button><br>
      Keys ${m.keys.copper}/${m.keys.silver}/${m.keys.gold} ◆${m.shards} items ${m.items.length}
      ${KEY_TYPES.map((k) => `<button data-k="${k}">+5 ${k}</button>`).join('')}<button data-a="shards">+20◆</button><br>
      <select id="dbgI">${itemOpts}</select><button data-a="item">give item</button><br>
      <select id="dbgP">${perkOpts}</select><button data-a="perk">add perk to run</button><br>
      <button data-a="log" class="${showLog ? 'on' : ''}">battle log</button> <button data-a="reset">reset save</button>
      ${showLog ? `<pre>${logText()}</pre>` : ''}
      <br><b>Floor ${floor}</b> · ${node.type} · key: ${node.keyDrop}
      <table>${rows}</table>
      ${DATA.errors.length ? `<div class="err"><b>Data errors: ${DATA.errors.length}</b><br>${DATA.errors.join('<br>')}</div>` : '<div>data: ok</div>'}`;
    const input = box.querySelector('#dbgF') as HTMLInputElement;
    input.addEventListener('change', () => { floor = Math.max(1, Math.floor(+input.value || 1)); render(); });
  };

  box.addEventListener('click', (e) => {
    const b = (e.target as HTMLElement).closest('button');
    if (!b) return;
    const m = game.meta;
    if (b.dataset.s) game.debug.speed = +b.dataset.s;
    if (b.dataset.k) m.keys[b.dataset.k as keyof typeof m.keys] += 5;
    switch (b.dataset.a) {
      case 'god': game.debug.god = !game.debug.god; break;
      case 'shards': m.shards += 20; break;
      case 'item': addItem(m, (box.querySelector('#dbgI') as HTMLSelectElement).value); break;
      case 'perk': {
        const run = game.run || newRun();
        pickPerk(run, DATA.perks.get((box.querySelector('#dbgP') as HTMLSelectElement).value)!);
        break;
      }
      case 'log': showLog = !showLog; break;
      case 'reset': resetSave(); game.screens.go('menu'); break;
      case 'end': debugEndFloor(); break;
      case 'go': {
        const run = game.run || newRun();
        run.floor = floor;
        run.dead = false;
        if (run.hp <= 0) run.hp = heroStats(run).maxHp;
        game.screens.go('battle');
        break;
      }
    }
    if (b.dataset.k || ['shards', 'item', 'perk'].includes(b.dataset.a || '')) {
      save();
      if (game.screens.current === 'inventory') game.screens.go('inventory');
    }
    render();
  });
  render();
  return { refresh: render };
}
