import { useEffect, useMemo, useState } from 'react';
import { getSummons } from './api';
import type { SummonsListItem } from './types';

function StatusBadge({ status }: { status: string }) {
  return <span className={`badge badge-${status.toLowerCase()}`}>{status}</span>;
}

export default function App() {
  const [items, setItems] = useState<SummonsListItem[]>([]);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  async function load(value = search) {
    setLoading(true);
    setError('');
    try {
      setItems(await getSummons(value));
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Ошибка загрузки');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { void load(''); }, []);

  const stats = useMemo(() => ({
    total: items.length,
    active: items.filter(x => ['Issued', 'Delivered', 'Acknowledged'].includes(x.status)).length,
    completed: items.filter(x => x.status === 'Completed').length
  }), [items]);

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">ЦИПСО</div>
        <div className="brand-subtitle">Реестр повесток</div>
        <nav>
          <a className="nav-item active" href="#registry">Реестр</a>
          <a className="nav-item" href="#analytics">Сводка</a>
          <a className="nav-item" href="/swagger" target="_blank">API</a>
        </nav>
        <div className="demo-note">Учебный контур · синтетические данные</div>
      </aside>

      <main className="content">
        <header className="topbar">
          <div>
            <p className="eyebrow">Централизованная информационная платформа списков оповещения</p>
            <h1>Реестр электронных повесток</h1>
          </div>
          <span className="environment">DEMO</span>
        </header>

        <section className="stats" id="analytics">
          <article><span>Всего записей</span><strong>{stats.total}</strong></article>
          <article><span>Активные</span><strong>{stats.active}</strong></article>
          <article><span>Завершенные</span><strong>{stats.completed}</strong></article>
        </section>

        <section className="panel" id="registry">
          <div className="panel-head">
            <div>
              <h2>Реестр</h2>
              <p>Поиск по номеру повестки, реестровому номеру или фамилии</p>
            </div>
            <form onSubmit={(e) => { e.preventDefault(); void load(); }} className="search-form">
              <input value={search} onChange={e => setSearch(e.target.value)} placeholder="Например, CIPSO-2026-0001" />
              <button type="submit">Найти</button>
            </form>
          </div>

          {error && <div className="error">{error}</div>}
          {loading ? <div className="state">Загрузка…</div> : (
            <div className="table-wrap">
              <table>
                <thead><tr><th>Номер</th><th>Адресат</th><th>Подразделение</th><th>Дата выпуска</th><th>Срок</th><th>Статус</th></tr></thead>
                <tbody>
                  {items.map(item => (
                    <tr key={item.id}>
                      <td><strong>{item.number}</strong><small>{item.reason}</small></td>
                      <td>{item.citizen.lastName} {item.citizen.firstName}<small>{item.citizen.registryNumber}</small></td>
                      <td>{item.office.name}<small>{item.office.code}</small></td>
                      <td>{new Date(item.issuedAt).toLocaleDateString('ru-RU')}</td>
                      <td>{new Date(item.dueAt).toLocaleString('ru-RU')}</td>
                      <td><StatusBadge status={item.status} /></td>
                    </tr>
                  ))}
                  {!items.length && <tr><td colSpan={6} className="state">Ничего не найдено</td></tr>}
                </tbody>
              </table>
            </div>
          )}
        </section>
      </main>
    </div>
  );
}
