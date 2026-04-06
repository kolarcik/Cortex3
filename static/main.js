let pyodide = null;
const container = document.querySelector('.container');
const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');
const scoreEl = document.getElementById('score');

// pixel ratio handling
function resizeCanvas(){
  const dpr = window.devicePixelRatio || 1;
  const cssWidth = Math.min(container.clientWidth, 900);
  const aspect = 600/800; // original
  const cssHeight = Math.round(cssWidth * aspect);
  canvas.style.width = cssWidth + 'px';
  canvas.style.height = cssHeight + 'px';
  canvas.width = Math.round(cssWidth * dpr);
  canvas.height = Math.round(cssHeight * dpr);
  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  // inform Python about logical size (CSS pixels)
  if (pyodide) pyodide.runPythonAsync(`set_size(${cssWidth}, ${cssHeight})`).catch(console.error);
}

// Expose render API for Python: render.draw(jsonString)
window.render = {
  draw: (jsonStr) => {
    const state = JSON.parse(jsonStr);
    // clear (use CSS pixel coords)
    ctx.clearRect(0,0,canvas.width,canvas.height);
    // draw player
    const p = state.player;
    ctx.fillStyle = '#0ff';
    ctx.fillRect(p.x, p.y, p.w, p.h);
    // draw obstacles
    ctx.fillStyle = '#f55';
    for (const o of state.obstacles) {
      ctx.fillRect(o.x, o.y, o.w, o.h);
    }
    // score
    scoreEl.textContent = 'Score: ' + (state.score|0);
  }
};

async function startPyodide() {
  pyodide = await loadPyodide({indexURL: 'https://cdn.jsdelivr.net/pyodide/v0.23.4/full/'});
  // load python files
  const corePy = await fetch('py/core.py').then(r => r.text());
  const gamePy = await fetch('py/game.py').then(r => r.text());
  const mainPy = await fetch('py/main.py').then(r => r.text());
  await pyodide.runPythonAsync(corePy + '\n' + gamePy + '\n' + mainPy);
  // set initial size
  resizeCanvas();
  // start game (Python start function)
  await pyodide.runPythonAsync('start()');
  requestAnimationFrame(loop);
}

async function loop(){
  if (pyodide) {
    try {
      await pyodide.runPythonAsync('tick()');
    } catch(e){
      console.error('tick error', e);
    }
  }
  requestAnimationFrame(loop);
}

// forward events to python handlers
function forwardKey(type, key){
  if (!pyodide) return;
  const safe = JSON.stringify(key);
  pyodide.runPythonAsync(`handle_key(${JSON.stringify(type)}, ${safe})`).catch(console.error);
}

window.addEventListener('keydown', (e)=>{ forwardKey('keydown', e.key); });
window.addEventListener('keyup', (e)=>{ forwardKey('keyup', e.key); });
window.addEventListener('resize', resizeCanvas);

// improved touch support: touchmove + touchstart
canvas.addEventListener('touchstart', (ev)=>{
  const t = ev.touches[0];
  const rect = canvas.getBoundingClientRect();
  const x = t.clientX - rect.left;
  const y = t.clientY - rect.top;
  const side = x < rect.width/2 ? 'left' : 'right';
  if (pyodide) pyodide.runPythonAsync(`handle_touch("start","${side}")`).catch(console.error);
  ev.preventDefault();
});
canvas.addEventListener('touchmove', (ev)=>{
  // allow slide up/down to move
  const t = ev.touches[0];
  const rect = canvas.getBoundingClientRect();
  const y = t.clientY - rect.top;
  const h = rect.height;
  // if touch is upper third, treat as 'up', lower third as 'down'
  if (pyodide) {
    if (y < h/3) pyodide.runPythonAsync(`handle_key('keydown','ArrowUp')`).catch(console.error);
    else if (y > h*2/3) pyodide.runPythonAsync(`handle_key('keydown','ArrowDown')`).catch(console.error);
  }
  ev.preventDefault();
});
canvas.addEventListener('touchend', (ev)=>{ if (pyodide) pyodide.runPythonAsync(`handle_touch("end","")`).catch(console.error); ev.preventDefault(); });

// buttons
document.getElementById('btn-pause').addEventListener('click', ()=>{ if (!pyodide) return; pyodide.runPythonAsync('pause()'); });
document.getElementById('btn-restart').addEventListener('click', ()=>{ if (!pyodide) return; pyodide.runPythonAsync('restart()'); });

startPyodide().catch(err => console.error(err));