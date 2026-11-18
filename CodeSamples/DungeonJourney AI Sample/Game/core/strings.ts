// All UI text goes through t(): strings.en.json now, more languages later.
import en from '../data/strings.en.json';

const table: Record<string, string> = en;

export function t(key: string, vars?: Record<string, string | number>): string {
  let s = table[key];
  if (s === undefined) { console.warn(`missing string: ${key}`); return key; }
  if (vars) for (const k in vars) s = s.split(`{${k}}`).join(String(vars[k]));
  return s;
}
