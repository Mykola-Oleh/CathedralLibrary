(() => {
  'use strict';
  const key = 'cathedral-library-notes-v1';
  const host = document.querySelector('#blocks');
  if (!host) return;
  const parser = typeof window.markdownit === 'function' ? window.markdownit({ html: false, linkify: false }) : null;
  let blocks;
  try { const saved = JSON.parse(localStorage.getItem(key) || 'null'); blocks = Array.isArray(saved) ? saved.filter(x => x && typeof x.text === 'string').slice(0, 200) : null; } catch { blocks = null; }
  blocks ??= [{ id: crypto.randomUUID(), text: '# Нотатки бібліотеки' }, { id: crypto.randomUUID(), text: '- Підготувати каталог\n- Перевірити примірники' }];
  let saveTimer;
  function persist() {
    clearTimeout(saveTimer);
    saveTimer = setTimeout(() => {
      try { localStorage.setItem(key, JSON.stringify(blocks)); document.querySelector('#save-state').textContent = 'Збережено локально'; }
      catch { document.querySelector('#save-state').textContent = 'Не вдалося зберегти: перевірте місце у браузері'; }
    }, 350);
  }
  function render() {
    host.replaceChildren();
    blocks.forEach((block, index) => {
      const card = document.createElement('div'); card.className = 'card p-3 note-block'; card.draggable = true; card.dataset.id = block.id;
      const top = document.createElement('div'); top.className = 'd-flex justify-content-between mb-2';
      const hint = document.createElement('span'); hint.textContent = '↕ Перетягніть блок'; hint.className = 'small text-secondary';
      const remove = document.createElement('button'); remove.type = 'button'; remove.className = 'btn btn-sm btn-outline-danger'; remove.textContent = 'Видалити';
      remove.addEventListener('click', () => { blocks.splice(index, 1); render(); persist(); });
      top.append(hint, remove);
      const editor = document.createElement('textarea'); editor.className = 'form-control'; editor.rows = 3; editor.maxLength = 15000; editor.value = block.text; editor.setAttribute('aria-label', 'Markdown блок ' + (index + 1));
      const preview = document.createElement('div'); preview.className = 'note-preview border rounded p-3 mt-2';
      const update = () => {
        if (parser) preview.innerHTML = parser.render(editor.value);
        else preview.textContent = editor.value;
      };
      editor.addEventListener('input', () => { block.text = editor.value; update(); persist(); }); update();
      card.addEventListener('dragstart', e => { e.dataTransfer.setData('text/plain', block.id); e.dataTransfer.effectAllowed = 'move'; });
      card.addEventListener('dragover', e => e.preventDefault());
      card.addEventListener('drop', e => {
        e.preventDefault(); const draggedId = e.dataTransfer.getData('text/plain');
        const from = blocks.findIndex(x => x.id === draggedId); const to = blocks.findIndex(x => x.id === block.id);
        if (from < 0 || to < 0 || from === to) return;
        const [item] = blocks.splice(from, 1); blocks.splice(to, 0, item); render(); persist();
      });
      card.append(top, editor, preview); host.append(card);
    });
  }
  document.querySelector('#add-block').addEventListener('click', () => { blocks.push({ id: crypto.randomUUID(), text: '' }); render(); persist(); host.lastElementChild?.querySelector('textarea')?.focus(); });
  render();
})();
