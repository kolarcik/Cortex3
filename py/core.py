import random

class Player:
    def __init__(self, w=32, h=32, x=100, y=100):
        self.w = w
        self.h = h
        self.x = x
        self.y = y
        self.vx = 0
        self.vy = 0

class Game:
    def __init__(self, width=800, height=600):
        self.width = width
        self.height = height
        self.player = Player(x=width//2, y=height-80)
        self.obstacles = []
        self._gen_obstacles()
        self.input = set()
        self.score = 0.0
        self.running = True

    def _gen_obstacles(self):
        self.obstacles = []
        for i in range(6):
            w = random.randint(40, 120)
            h = random.randint(20, 40)
            x = random.randint(0, max(0, self.width - w))
            y = random.randint(20, max(20, self.height//2))
            self.obstacles.append({'x': x, 'y': y, 'w': w, 'h': h})

    def update(self, dt=1/60.0):
        if not self.running:
            return
        speed = 220 * dt
        if 'ArrowLeft' in self.input or 'a' in self.input or 'left' in self.input:
            self.player.x -= speed
        if 'ArrowRight' in self.input or 'd' in self.input or 'right' in self.input:
            self.player.x += speed
        if 'ArrowUp' in self.input or 'w' in self.input or 'up' in self.input:
            self.player.y -= speed
        if 'ArrowDown' in self.input or 's' in self.input or 'down' in self.input:
            self.player.y += speed
        # bounds
        self.player.x = max(0, min(self.player.x, self.width - self.player.w))
        self.player.y = max(0, min(self.player.y, self.height - self.player.h))
        # collision detection
        px1 = self.player.x; py1 = self.player.y; px2 = px1 + self.player.w; py2 = py1 + self.player.h
        for o in self.obstacles:
            ox1 = o['x']; oy1 = o['y']; ox2 = ox1 + o['w']; oy2 = oy1 + o['h']
            if not (px2 < ox1 or px1 > ox2 or py2 < oy1 or py1 > oy2):
                # collision
                self.score = max(0, self.score - 10)
                # knock back to center
                self.player.x = self.width//2
                self.player.y = self.height-80
        # reward for survival
        self.score += 0.1

    def to_state(self):
        return {
            'player': {'x': self.player.x, 'y': self.player.y, 'w': self.player.w, 'h': self.player.h},
            'obstacles': self.obstacles,
            'score': int(self.score)
        }
