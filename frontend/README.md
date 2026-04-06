Pyodide Canvas Game Demo

This is a minimal static demo showing how to run Python game logic in the browser using Pyodide. The game is simple: move the square to collect orange targets.

Files:
- index.html: demo page and UI
- style.css: minimal styling
- pyodide_loader.js: loads Pyodide, runs game.py and forwards DOM events
- game.py: glue + browser interop (uses game_core)
- game_core.py: pure-Python game logic (unit-testable)

Run locally:
1. Serve the folder over HTTP (browsers restrict file access for modules). Example:
   python3 -m http.server 8000
2. Open http://localhost:8000/frontend in your browser

Run tests (requires CPython + pytest):
1. cd frontend
2. python3 -m venv .venv
3. source .venv/bin/activate
4. pip install pytest
5. pytest -q

Notes:
- Events are forwarded to Python via Pyodide's JS interop.
- High-score is persisted into localStorage under key `highscore`.
- The core game logic lives in game_core.py and is pure Python so it can be tested in CI.
