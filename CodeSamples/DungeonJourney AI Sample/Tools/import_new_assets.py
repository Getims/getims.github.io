# Imports the customer's asset pack (Reference/New) into Assets/: renames, trims, splits hero sheets, copies sounds.
# Run from the project folder: python Tools/import_new_assets.py
# Mapping old -> new and the reasons: Docs/ASSETS_SWAP_REPORT.md.
import os
import re
import array
import shutil
import wave
from PIL import Image

SRC = 'Reference/New/PhaserTextures'
SND = 'Reference/New/PhaserSounds'
DST = 'Assets'
# pack sounds recorded far too quiet (click peaks at ~1% of full scale): normalized to this peak
NORMALIZE = {'click.wav', 'equip.wav', 'reward.wav'}
PEAK = 0.9
HERO_SCALE = 1.45  # new hero frames are smaller than the old cuts (idle 184 px vs 269 px tall)

# GUID-named files: baked text from the original (leaderboard / counters) -> reference only, not in the build
GUID_NAMES = {
    '1dcb0519-91db-4688-9a39-f8897c57b1c6': 'text_digit0_white',
    'a0e38720-422a-4a36-b1c6-45c2363ea6f0': 'text_digit0_gold',
    'b43e379d-65c5-4612-bb9f-cafb3b7cc0ff': 'text_digit0_copper',
    '21037e37-149d-478d-939f-cbfa00c67192': 'lb_rank_line',
    '48738698-9f2a-4792-90c0-3eab3e519ea0': 'lb_your_best',
    '5c2a8224-8c3b-4e20-9a36-a010af719cb4': 'lb_season_live',
    '9b7e7e9e-3992-4f3f-9ef7-2cc29789a8c0': 'lb_season',
    'c1f95aab-f80a-4512-9fa3-15e4fd6f0bab': 'lb_entry_1',
    'fb548a9b-1d9d-4ad2-b189-be16287630e7': 'lb_entry_2',
    'e3bdf25f-f522-474f-9e97-f0ff9cc3ddfe': 'lb_entry_3',
    'c4db4391-5d4e-438a-a70d-c8a3fa553b98': 'lb_loadout_title',
    'd5a96385-a265-49d0-a417-2e2e80ad2969': 'lb_reward_note',
    'dd216745-73de-45c2-86ff-d18acd0aadb5': 'lb_tagline',
    'f65d00c3-649f-454f-9ef7-34884ebbc599': 'lb_title',
}

# straight copies: new file -> Assets path
COPY = {
    'background': 'backgrounds/bg_battle',
    'menu_bg': 'backgrounds/bg_menu',
    'died_bg': 'backgrounds/bg_fail',
    'menu_hero': 'hero/hero_menu',
    'slash': 'fx/fx_slash',
    'flame': 'fx/fx_flame',
    'glow': 'fx/fx_glow',
    'spark': 'fx/fx_spark',
    'key_copper': 'items/key_copper',
    'key_silver': 'items/key_silver',
    'key_gold': 'items/key_gold',
    'key': 'ui/icon_key',
    'enemy_fallback': 'enemies/enemy_fallback',
    'menu_title': 'ui/logo_title',
    'btn_play': 'ui/btn_play',
    'btn_inventory': 'ui/btn_inventory',
    'btn_continue': 'ui/btn_continue',
    'btn_settings': 'ui/btn_settings',
    'btn_leaderboard': 'ui/btn_leaderboard',
    'btn_back': 'ui/btn_back',
    'btn_quit': 'ui/btn_quit',
    'btn_inventory_tab': 'ui/btn_tab',
    'btn_open_copper': 'ui/btn_open_copper',
    'btn_open_silver': 'ui/btn_open_silver',
    'btn_open_gold': 'ui/btn_open_gold',
    'btn_open_disabled': 'ui/btn_open_disabled',
    'btn_3to1_gold': 'ui/btn_3to1_gold',
    'btn_3to1_silver': 'ui/btn_3to1_silver',
    'died_btn_revive': 'ui/btn_revive',
    'died_btn_mainmenu': 'ui/btn_mainmenu',
    'died_btn_playagain': 'ui/btn_playagain',
    'died_title': 'ui/text_you_died',
    'died_ornamental': 'ui/orn_fail_divider',
    'victory_top': 'ui/orn_victory_top',
    'victory_bot': 'ui/orn_victory_bottom',
    'victory_side': 'ui/orn_victory_side',
    'fight': 'ui/icon_fight',
    'foe_alive': 'ui/diamond_full',
    'foe_dead': 'ui/diamond_empty',
    'game_bar_hp': 'ui/hpbar_frame',
    'pnl_common': 'ui/card_frame_common',
    'pnl_uncommon': 'ui/card_frame_uncommon',
    'pnl_rare': 'ui/card_frame_rare',
    'pnl_epic': 'ui/card_frame_epic',
    'pnl_legendary': 'ui/card_frame_legendary',
    'pnl_equipment_slot': 'inventory/slot_frame',
    'pnl_inventory_slot': 'inventory/grid_cell',
    'pnl_inventory_keyholder': 'inventory/panel_keys',
}


def save(im, rel):
    path = os.path.join(DST, rel + '.png')
    os.makedirs(os.path.dirname(path), exist_ok=True)
    im.save(path, optimize=True)
    return path


def trim(im):
    return im.crop(im.getbbox())


def feet_centered(frame):
    """Trim, then pad horizontally so the feet (bottom 20 rows) are at the center: bottom-center anchor stays put."""
    im = trim(frame)
    a = im.getchannel('A')
    w, h = im.size
    xs = [x for y in range(max(0, h - 20), h) for x in range(w) if a.getpixel((x, y)) > 40]
    cx = (min(xs) + max(xs)) / 2 if xs else w / 2
    half = int(max(cx, w - cx)) + 1
    out = Image.new('RGBA', (half * 2, h))
    out.paste(im, (int(half - cx), 0))
    return out.resize((round(out.width * HERO_SCALE), round(h * HERO_SCALE)), Image.LANCZOS)


def frames(name, n):
    im = Image.open(os.path.join(SRC, name + '.png')).convert('RGBA')
    fw = im.width // n
    return [im.crop((i * fw, 0, (i + 1) * fw, im.height)) for i in range(n)]


REAL_WORDS = {'bat', 'axe', 'eye', 'cap'}  # 3-letter words that are not the pack's random suffix


def unsuffix(name):
    head, _, last = name.rpartition('_')
    return head if head and len(last) == 3 and last not in REAL_WORDS else name


def normalize(src, dst):
    with wave.open(src) as w:
        params = w.getparams()
        a = array.array('h', w.readframes(w.getnframes()))
    k = PEAK * 32767 / max(1, max(abs(x) for x in a))
    a = array.array('h', (max(-32768, min(32767, round(x * k))) for x in a))
    with wave.open(dst, 'wb') as w:
        w.setparams(params)
        w.writeframes(a.tobytes())


def main():
    done = []
    for name, rel in COPY.items():
        im = Image.open(os.path.join(SRC, name + '.png'))
        done.append(save(im, rel))
    for guid, rel in GUID_NAMES.items():
        done.append(save(Image.open(os.path.join(SRC, guid + '.png')), 'ui/text_ref/' + rel))

    # hero: idle 2 frames (256 px), attack 3 frames (384 px), victory pose
    for i, f in enumerate(frames('player_idle', 2)):
        done.append(save(feet_centered(f), f'hero/hero_idle_{i + 1}'))
    for i, f in enumerate(frames('player_attack', 3)):
        done.append(save(feet_centered(f), f'hero/hero_attack_{i + 1}'))
    done.append(save(feet_centered(Image.open(os.path.join(SRC, 'victory_pose.png')).convert('RGBA')), 'hero/hero_victory'))

    for f in sorted(os.listdir(SRC)):
        base = f[:-4]
        # enemies: drop the random 3-char suffix, trim the empty margin (sprites are bottom-center anchored)
        if base.startswith('enemy_') and base != 'enemy_fallback':
            clean = unsuffix(base)
            if clean == 'enemy_rotting_zombie' and base.endswith('qux'):
                clean = 'enemy_rotting_zombie_pale'
            done.append(save(trim(Image.open(os.path.join(SRC, f)).convert('RGBA')), 'enemies/' + clean))
        # item icons: <slot>_<name>, suffix dropped; acce_ -> acc_
        m = re.match(r'^(acc|acce|arms|body|head|legs|sec|wpn)_(.+)$', base)
        if m:
            slot = 'acc' if m.group(1) == 'acce' else m.group(1)
            nm = m.group(2)
            if base == 'head_storm_circlet_jjs':
                nm = 'storm_circlet_blue'
            elif base == 'head_storm_circlet_nb1':
                nm = 'storm_circlet'
            else:
                nm = unsuffix(nm)
            done.append(save(Image.open(os.path.join(SRC, f)), f'items/{slot}_{nm}'))
        if base.startswith('empty_'):
            done.append(save(Image.open(os.path.join(SRC, f)), 'inventory/slot_' + base[6:]))

    os.makedirs(os.path.join(DST, 'Audio'), exist_ok=True)
    for f in sorted(os.listdir(SND)):
        (normalize if f in NORMALIZE else shutil.copyfile)(os.path.join(SND, f), os.path.join(DST, 'Audio', f))
        done.append(os.path.join(DST, 'Audio', f))
    print(len(done), 'files')


main()
