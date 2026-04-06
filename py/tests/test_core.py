from core import Game

def test_player_moves_left():
    g = Game(width=200, height=200)
    g.player.x = 100
    g.input.add('left')
    g.update(dt=1.0)
    assert g.player.x < 100

def test_collision_resets_position():
    g = Game(width=200, height=200)
    # place an obstacle overlapping player
    g.player.x = 50
    g.player.y = 50
    g.obstacles = [{'x': 45, 'y':45, 'w':20, 'h':20}]
    prev_score = g.score
    g.update(dt=1.0)
    # after collision, player should be moved to center-ish (width//2)
    assert g.player.x == g.width//2
    assert g.score <= prev_score + 0.1
