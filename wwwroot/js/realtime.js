(() => {
 const mode = document.getElementById('mode'); if (!mode) return;
 const status = document.getElementById('connection'), metrics = document.getElementById('metrics'), list = document.getElementById('events');
 let stopped = false, abort = new AbortController(), socket, version = 0, requests = 0, bytes = 0, lastAt = 0;
 const pause = ms => new Promise(resolve => setTimeout(resolve, ms));
 function display() { metrics.textContent = `Запитів/повідомлень: ${requests} · отримано ~${bytes} байтів`; }
 function receive(data, size) {
   requests++; bytes += size; display();
   version = Math.max(version, data.version || 0);
   for (const event of data.events || []) {
     const item = document.createElement('li'); item.className = 'list-group-item';
     item.textContent = `Подія #${event.id} · ${new Date(event.atUtc).toLocaleString('uk-UA')} · ${event.kind}`;
     list.prepend(item); while (list.children.length > 30) list.lastElementChild.remove();
   }
   return (data.events || []).length;
 }
 async function pollLoop(longPoll) {
   let interval = 1500;
   while (!stopped) {
     try {
       const endpoint = longPoll ? '/api/realtime/long-poll' : '/api/realtime/poll';
       const response = await fetch(`${endpoint}?after=${version}`, { signal: abort.signal });
       if (!response.ok) throw Error(`HTTP ${response.status}`);
       const text = await response.text();
       const count = receive(JSON.parse(text), new TextEncoder().encode(text).length);
       status.textContent = longPoll ? 'Long Polling: підключено' : 'Adaptive Polling: підключено';
       if (!longPoll) { interval = count ? 1500 : Math.min(interval * 1.5, 10000); await pause(interval); }
     } catch (err) { if (!stopped) { status.textContent = 'Помилка: ' + err.message; await pause(3000); } }
   }
 }
 function start() {
   stopped = false; abort = new AbortController(); version = 0; requests = 0; bytes = 0; list.replaceChildren(); display();
   if (mode.value === 'ws') {
     const proto = location.protocol === 'https:' ? 'wss:' : 'ws:';
     socket = new WebSocket(`${proto}//${location.host}/ws/library`);
     socket.onopen = () => status.textContent = 'WebSocket: підключено';
     socket.onmessage = e => receive(JSON.parse(e.data), new TextEncoder().encode(e.data).length);
     socket.onerror = () => status.textContent = 'WebSocket: помилка';
     socket.onclose = () => { if (!stopped) status.textContent = 'WebSocket: закрито'; };
   } else pollLoop(mode.value === 'long');
 }
 mode.addEventListener('change', () => { stopped = true; abort.abort(); socket?.close(); socket = null; start(); });
 window.addEventListener('pagehide', () => { stopped = true; abort.abort(); socket?.close(); });
 start();
})();
