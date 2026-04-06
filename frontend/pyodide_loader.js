// Loads Pyodide, loads game_core.py and game.py, wires DOM events to Python handlers

async function main(){
  const pyodide = await loadPyodide({indexURL: 'https://cdn.jsdelivr.net/pyodide/v0.23.4/full/'});
  // fetch core first then game
  const coreResp = await fetch('game_core.py');
  const coreCode = await coreResp.text();
  await pyodide.runPythonAsync(coreCode);

  const resp = await fetch('game.py');
  const code = await resp.text();
  await pyodide.runPythonAsync(code);

  const canvas = document.getElementById('gameCanvas');
  const init = pyodide.globals.get('init_game');
  init(canvas);

  const handle_event = pyodide.globals.get('handle_event');
  document.addEventListener('keydown', e => handle_event(e));
  document.addEventListener('keyup', e => handle_event(e));
  canvas.addEventListener('touchstart', e => { e.preventDefault(); handle_event(e); }, {passive:false});
  canvas.addEventListener('touchmove', e => { e.preventDefault(); handle_event(e); }, {passive:false});
  canvas.addEventListener('touchend', e => { e.preventDefault(); handle_event(e); }, {passive:false});

  document.getElementById('pauseBtn').addEventListener('click', () => pyodide.globals.get('toggle_pause')());
  document.getElementById('restartBtn').addEventListener('click', () => pyodide.globals.get('restart_game')());

  if(!window._scoreObserver){
    window._scoreObserver = true;
    window.updateScore = (s) => { document.getElementById('score').textContent = `Score: ${s}`; }
  }

  // resize handling: adjust canvas size and notify python
  function fitCanvas(){
    const rect = canvas.getBoundingClientRect();
    const w = Math.min(1024, Math.floor(rect.width));
    const h = Math.max(200, Math.floor(rect.height));
    canvas.width = w;
    canvas.height = h;
    // notify python
    try{
      pyodide.globals.get('on_resize')(w, h);
    }catch(e){/* ignore */}
  }
  window.addEventListener('resize', fitCanvas);
  fitCanvas();
}

main().catch(err => { console.error(err); alert('Failed to start Pyodide: ' + err); });
