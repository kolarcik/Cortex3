import pytest
from game_core import GameCore

def test_collision_increases_score():
    g = GameCore(200,200)
    # place player on target
    g.player.x = g.target.x
    g.player.y = g.target.y
    before = g.score
    g._check_collision()
    assert g.score == before + 1

def test_restart_resets_state():
    g = GameCore(300,300)
    g.score = 5
    g.player.x = 10
    g.restart()
    assert g.score == 0
    assert g.player.x == g.w/2

def test_update_moves_player():
    g = GameCore(400,400)
    g.keys.add('ArrowRight')
    x0 = g.player.x
    g.update(0.1)
    assert g.player.x > x0
