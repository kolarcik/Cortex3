# Adapter module: uses core logic and JS interop for rendering and storage
import json
from core import Game
from js import render, localStorage

# global game instance
game = Game()

def _save_highscore_if_needed():
    try:
        key = 'pygame_highscore'
        prev = localStorage.getItem(key)
        prev_score = int(prev) if prev is not None else 0
        if int(game.score) > prev_score:
            localStorage.setItem(key, str(int(game.score)))
    except Exception as e:
        # ignore storage errors
        pass

def tick():
    game.update()
    _save_highscore_if_needed()
    render.draw(json.dumps(game.to_state()))

def handle_key(evt_type, key):
    if evt_type == 'keydown':
        game.input.add(key)
    elif evt_type == 'keyup':
        try:
            game.input.discard(key)
        except:
            pass

def handle_touch(evt_type, side):
    if evt_type == 'start':
        if side == 'left':
            game.input.add('left')
        elif side == 'right':
            game.input.add('right')
    else:
        game.input.discard('left')
        game.input.discard('right')

def start():
    game.running = True

def pause():
    game.running = False

def restart():
    global game
    game = Game()

def set_size(w, h):
    # called from JS when canvas resized; update game logical size
    try:
        game.width = int(w)
        game.height = int(h)
        # re-center player within new bounds
        game.player.x = max(0, min(game.player.x, game.width - game.player.w))
        game.player.y = max(0, min(game.player.y, game.height - game.player.h))
    except Exception:
        pass

# helper to expose highscore to JS if needed
def get_highscore():
    try:
        key = 'pygame_highscore'
        prev = localStorage.getItem(key)
        return int(prev) if prev is not None else 0
    except Exception:
        return 0

print('game adapter loaded')