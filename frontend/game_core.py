from dataclasses import dataclass, field
import math
import random
from typing import Set, Dict

@dataclass
class Player:
    x: float
    y: float
    size: int = 28
    speed: float = 220.0

@dataclass
class Target:
    x: int
    y: int
    r: int = 14

@dataclass
class GameCore:
    w: int
    h: int
    player: Player = field(init=False)
    target: Target = field(init=False)
    keys: Set[str] = field(default_factory=set)
    score: int = 0
    paused: bool = False

    def __post_init__(self):
        self.player = Player(self.w/2, self.h/2)
        self.target = Target(60, 60)

    def clamp_player(self):
        half = self.player.size / 2
        self.player.x = max(half, min(self.w - half, self.player.x))
        self.player.y = max(half, min(self.h - half, self.player.y))

    def update(self, dt: float):
        if self.paused:
            return False
        vx = 0
        vy = 0
        if 'ArrowLeft' in self.keys or 'a' in self.keys:
            vx -= 1
        if 'ArrowRight' in self.keys or 'd' in self.keys:
            vx += 1
        if 'ArrowUp' in self.keys or 'w' in self.keys:
            vy -= 1
        if 'ArrowDown' in self.keys or 's' in self.keys:
            vy += 1
        if vx != 0 or vy != 0:
            l = math.hypot(vx, vy)
            vx /= l; vy /= l
        self.player.x += vx * self.player.speed * dt
        self.player.y += vy * self.player.speed * dt
        self.clamp_player()
        return self._check_collision()

    def _check_collision(self):
        dx = self.player.x - self.target.x
        dy = self.player.y - self.target.y
        dist2 = dx*dx + dy*dy
        rad = (self.player.size/2) + self.target.r
        if dist2 <= rad*rad:
            self.score += 1
            self._move_target()
            return True
        return False

    def _move_target(self):
        self.target.x = random.randint(self.target.r+8, max(self.target.r+8, self.w - self.target.r - 8))
        self.target.y = random.randint(self.target.r+8, max(self.target.r+8, self.h - self.target.r - 8))

    def restart(self):
        self.player = Player(self.w/2, self.h/2)
        self.target = Target(60, 60)
        self.keys.clear()
        self.score = 0
        self.paused = False

    def set_player_pos(self, x: float, y: float):
        self.player.x = x
        self.player.y = y
        self.clamp_player()

    def resize(self, w: int, h: int):
        # preserve proportional player position
        if self.w and self.h:
            px = self.player.x / self.w
            py = self.player.y / self.h
        else:
            px = py = 0.5
        self.w = w; self.h = h
        self.player.x = px * self.w
        self.player.y = py * self.h
        self.clamp_player()
