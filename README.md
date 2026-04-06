Pyodide Game Demo - client-side game in Python

Co je implementováno:
- Statická stránka (index.html) s canvas a ovládacími prvky
- JS loader (static/main.js) který načte Pyodide a spustí Python moduly
- Python modul split: py/core.py (čistá logika) + py/game.py (adapter s JS interop)
- Rendering provádí JS (window.render.draw) volaný z Pythonu přes Pyodide
- Responsive canvas, touch ovládání a high-score uložené v localStorage
- Jednoduché pytest testy pro herní logiku (py/tests/test_core.py)

Jak spustit lokálně:
1) Spusť jednoduchý HTTP server v kořenovém adresáři projektu:
   python3 -m http.server 8000
2) Otevři v prohlížeči: http://localhost:8000

Spuštění testů (lokálně, bez Pyodide):
1) Nainstaluj pytest: pip install pytest
2) Spusť: pytest -q py/tests

Poznámky:
- Kandidát pro zlepšení: přepnout na volání native requestAnimationFrame z Pythonu (async loop) nebo optimalizovat interop pro vysoké FPS.
- Použité CDN: Pyodide v0.23.4

Licence: MIT
