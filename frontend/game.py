from pyodide.ffi import create_proxy
from js import window, console
from game_core import GameCore

# Glue between pyodide/browser and pure-Python GameCore

core = None
canvas = None
animation_proxy = None
last_ts = None


def init_game(c):
    global core, canvas, animation_proxy, last_ts
    canvas = c
    core = GameCore(int(canvas.width), int(canvas.height))
    last_ts = None
    animation_proxy = create_proxy(_game_loop)
    # restore high score (if exists) and notify
    try:
        hs = window.localStorage.getItem('highscore')
        if hs is not None:
            window.updateScore(int(hs))
    except Exception:
        pass
    window.requestAnimationFrame(animation_proxy)


def _game_loop(ts):
    global last_ts
    if last_ts is None:
        last_ts = ts
    dt = (ts - last_ts) / 1000.0
    last_ts = ts
    if core and not core.paused:
        collided = core.update(dt)
        if collided:
            try:
                window.updateScore(core.score)
                hs = window.localStorage.getItem('highscore')
                if hs is None or core.score > int(hs):
                    window.localStorage.setItem('highscore', str(core.score))
            except Exception:
                pass
    _render()
    window.requestAnimationFrame(animation_proxy)


def _render():
    if core is None or canvas is None:
        return
    ctx = canvas.getContext('2d')
    w, h = core.w, core.h
    ctx.clearRect(0,0,w,h)
    ctx.fillStyle = '#071029'
    ctx.fillRect(0,0,w,h)
    # draw target
    tx = core.target
    ctx.beginPath()
    ctx.fillStyle = '#f97316'
    ctx.arc(tx.x, tx.y, tx.r, 0, 6.283)
    ctx.fill()
    # draw player
    p = core.player
    ctx.fillStyle = '#60a5fa'
    ctx.beginPath()
    ctx.rect(p.x - p.size/2, p.y - p.size/2, p.size, p.size)
    ctx.fill()
    # HUD
    ctx.fillStyle = '#e6eef8'
    ctx.font = '14px sans-serif'
    ctx.fillText(f'Score: {core.score}', 10, 20)
    if core.paused:
        ctx.fillStyle = 'rgba(0,0,0,0.5)'
        ctx.fillRect(w/2 - 80, h/2 - 30, 160, 60)
        ctx.fillStyle = '#fff'
        ctx.fillText('PAUSED', w/2 - 26, h/2 + 6)


def handle_event(ev):
    # events proxied from JS
    t = ev.type
    if t == 'keydown':
        core.keys.add(ev.key)
    elif t == 'keyup':
        k = ev.key
        if k in core.keys:
            core.keys.remove(k)
    elif t.startswith('touch'):
        touches = ev.touches if hasattr(ev, 'touches') else None
        if touches and len(touches) > 0:
            tp = touches.item(0)
            rect = canvas.getBoundingClientRect()
            x = tp.clientX - rect.left
            y = tp.clientY - rect.top
            core.set_player_pos(x, y)
    return None


def toggle_pause():
    core.paused = not core.paused


def restart_game():
    core.restart()
    # check highscore reset not needed
    try:
        window.updateScore(core.score)
    except Exception:
        pass


def on_resize(w, h):
    # called from JS when canvas size changes
    if core:
        core.resize(int(w), int(h))

# expose
init_game = init_game
handle_event = handle_event
toggle_pause = toggle_pause
restart_game = restart_game
on_resize = on_resize
