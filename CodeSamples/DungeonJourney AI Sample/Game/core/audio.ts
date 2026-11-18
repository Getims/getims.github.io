// Project copy of Modules/audio.ts: a game (not a playable) needs music tracks switched by screen and the
// Music volume changed live from Settings. Differences: music on its own gain node, playMusic() switches with a
// fade, setMusicVolume(), a compressor on the master, per-key voice limit + playback rate, addBuffer() for
// procedural sounds. Decode up front, locked until first gesture; SFX requested while locked/hidden are dropped.

function dataUriToBuffer(uri: string): ArrayBuffer {
  const bin = atob(uri.slice(uri.indexOf(',') + 1));
  const out = new Uint8Array(bin.length);
  for (let i = 0; i < bin.length; i++) out[i] = bin.charCodeAt(i);
  return out.buffer;
}

const FADE = 0.4;

export class AudioSystem {
  private ctx: AudioContext | null = null;
  private master: GainNode | null = null;
  private musicGain: GainNode | null = null;
  private buffers: Record<string, AudioBuffer> = {};
  private music: { key: string; src: AudioBufferSourceNode | null; gain: GainNode | null } | null = null;
  private musicVolume = 1;
  unlocked = false;
  muted = false;
  private hidden = false;

  /** sources: key -> data URI (audio/mpeg etc). Resolves when everything is decoded (failures are skipped). */
  async init(sources: Record<string, string>) {
    const Ctx = window.AudioContext || (window as any).webkitAudioContext;
    if (!Ctx) return;
    this.ctx = new Ctx();
    this.master = this.ctx.createGain();
    // soft limiter: stacked hits (crit + hurt + thorns) must not clip
    const comp = this.ctx.createDynamicsCompressor();
    comp.threshold.value = -12;
    comp.knee.value = 8;
    comp.ratio.value = 6;
    comp.attack.value = 0.003;
    comp.release.value = 0.15;
    this.master.connect(comp).connect(this.ctx.destination);
    this.musicGain = this.ctx.createGain();
    this.musicGain.gain.value = this.musicVolume;
    this.musicGain.connect(this.master);
    await Promise.all(Object.keys(sources).map((k) => new Promise<void>((resolve) => {
      try {
        this.ctx!.decodeAudioData(dataUriToBuffer(sources[k]), (b) => {
          this.buffers[k] = b;
          // the track was requested before it finished decoding: start it now
          if (this.music && this.music.key === k && !this.music.src) this.startTrack(k);
          resolve();
        }, () => resolve());
      } catch { resolve(); }
    })));
  }

  // Must run inside a user gesture.
  // Call on every touch: iOS can leave the context 'suspended'/'interrupted' (call, app switch).
  // Returns true only on the first (unlocking) call.
  unlock() {
    if (!this.ctx) return false;
    if (this.unlocked) {
      if (!this.hidden && this.ctx.state !== 'running') { const p = this.ctx.resume(); if (p) p.catch(() => {}); }
      return false;
    }
    this.ctx.resume();
    const src = this.ctx.createBufferSource();
    src.buffer = this.ctx.createBuffer(1, 1, 22050);
    src.connect(this.ctx.destination);
    src.start(0);
    this.unlocked = true;
    this.applyGain();
    // a track chosen before the first gesture starts now
    if (this.music && !this.music.src) this.startTrack(this.music.key);
    return true;
  }

  private voices: Record<string, number> = {};

  /** Mono samples (-1..1) as a sound key: procedural SFX. */
  addBuffer(key: string, data: Float32Array, rate: number) {
    if (!this.ctx) return;
    const b = this.ctx.createBuffer(1, data.length, rate);
    b.getChannelData(0).set(data);
    this.buffers[key] = b;
  }

  /** false = not played (locked, hidden, no buffer, or maxVoices of this key already sound). */
  play(key: string, volume: number, rate = 1, maxVoices = 4) {
    if (!this.ctx || !this.master || !this.unlocked || this.hidden || volume <= 0) return false;
    const buf = this.buffers[key];
    if (!buf || (this.voices[key] || 0) >= maxVoices) return false;
    const src = this.ctx.createBufferSource();
    src.buffer = buf;
    src.playbackRate.value = rate;
    const g = this.ctx.createGain();
    g.gain.value = volume;
    src.connect(g).connect(this.master);
    this.voices[key] = (this.voices[key] || 0) + 1;
    src.onended = () => { this.voices[key]--; };
    src.start(0);
    return true;
  }

  private fadeIn = FADE;
  private delay = 0;

  /** Looped music; the same key keeps playing, another key cross-fades. null stops the music.
   *  fade: seconds of the fade out / in; delay: silence before the new track. */
  playMusic(key: string | null, fade = FADE, delay = 0) {
    if (this.music?.key === key) return;
    this.fadeOut(fade);
    this.fadeIn = fade;
    this.delay = delay;
    this.music = key ? { key, src: null, gain: null } : null;
    if (key) this.startTrack(key);
  }

  setMusicVolume(v: number) {
    this.musicVolume = v;
    if (this.musicGain && this.ctx) this.musicGain.gain.setTargetAtTime(v, this.ctx.currentTime, 0.05);
  }

  private startTrack(key: string) {
    const buf = this.buffers[key];
    if (!this.ctx || !this.musicGain || !this.unlocked || !buf || !this.music || this.music.key !== key) return;
    const src = this.ctx.createBufferSource();
    src.buffer = buf;
    src.loop = true;
    // skip MP3 encoder/decoder padding so the loop seam has no gap
    const [a, b] = this.loopPoints(key, buf);
    src.loopStart = a;
    src.loopEnd = b;
    const g = this.ctx.createGain();
    const now = this.ctx.currentTime + this.delay;
    this.delay = 0;
    g.gain.setValueAtTime(0, now);
    g.gain.linearRampToValueAtTime(1, now + this.fadeIn);
    src.connect(g).connect(this.musicGain);
    src.start(now, a);
    this.music.src = src;
    this.music.gain = g;
  }

  private fadeOut(fade = FADE) {
    const m = this.music;
    if (!m || !m.src || !m.gain || !this.ctx) return;
    const now = this.ctx.currentTime;
    m.gain.gain.cancelScheduledValues(now);
    m.gain.gain.setValueAtTime(m.gain.gain.value, now);
    m.gain.gain.linearRampToValueAtTime(0, now + fade);
    m.src.stop(now + fade + 0.05);
  }

  private loops: Record<string, [number, number]> = {};

  /** Seconds of the first/last non-silent sample (threshold ~ -60 dB), cached per key. */
  private loopPoints(key: string, buf: AudioBuffer): [number, number] {
    if (this.loops[key]) return this.loops[key];
    const n = buf.length, th = 0.001;
    let first = 0, last = n - 1;
    const loud = (i: number) => { for (let c = 0; c < buf.numberOfChannels; c++) if (Math.abs(buf.getChannelData(c)[i]) > th) return true; return false; };
    while (first < last && !loud(first)) first++;
    while (last > first && !loud(last)) last--;
    const r: [number, number] = last - first < buf.sampleRate * 0.1 ? [0, buf.duration] : [first / buf.sampleRate, (last + 1) / buf.sampleRate];
    return (this.loops[key] = r);
  }

  setMuted(m: boolean) { this.muted = m; this.applyGain(); }

  setHidden(h: boolean) {
    this.hidden = h;
    if (!this.ctx || !this.unlocked) return;
    if (h) this.ctx.suspend();
    else this.ctx.resume();
  }

  private applyGain() {
    if (this.master) this.master.gain.value = this.muted ? 0 : 1;
  }
}

export const audio = new AudioSystem();
