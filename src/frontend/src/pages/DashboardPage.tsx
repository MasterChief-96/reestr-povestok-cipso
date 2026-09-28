import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getDashboard } from '../api';
import { StatusBadge } from '../components/StatusBadge';
import { statusLabels } from '../constants';
import type { DashboardSummary, SummonsStatus } from '../types';

export function DashboardPage() {
  const [data, setData] = useState<DashboardSummary | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    void getDashboard()
      .then(setData)
      .catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить сводку'));
  }, []);

  if (error) {
    return <div className="error">{error}</div>;
  }

  if (!data) {
    return <div className="state">Загрузка сводки…</div>;
  }

  const statusEntries = Object.entries(data.byStatus) as Array<[SummonsStatus, number]>;

  return (
    <>
      <header className="topbar">
        <div>
          <p className="eyebrow">Сводка</p>
          <h1>Сводка по воинскому учёту</h1>
        </div>
        <Link className="primary button-link" to="/summons">Открыть реестр</Link>
      </header>

      <section className="stats stats-wide">
        <article><span>Всего повесток</span><strong>{data.total}</strong></article>
        <article><span>Активные</span><strong>{data.active}</strong></article>
        <article><span>Завершенные</span><strong>{data.completed}</strong></article>
        <article><span>Отмененные</span><strong>{data.cancelled}</strong></article>
        <article><span>Призывники</span><strong>{data.citizens}</strong></article>
        <article><span>Обращения</span><strong>{data.appeals}</strong></article>
        <article><span>Документы</span><strong>{data.documents}</strong></article>
      </section>

      <div className="dashboard-grid">
        <section className="panel">
          <div className="panel-head">
            <div>
              <h2>По статусам</h2>
              <p>Распределение повесток военного учёта по статусам.</p>
            </div>
          </div>
          <div className="status-list">
            {statusEntries.map(([status, count]) => (
              <div className="status-row" key={status}>
                <StatusBadge status={status} />
                <span>{statusLabels[status]}</span>
                <strong>{count}</strong>
              </div>
            ))}
          </div>
        </section>

        <section className="panel">
          <div className="panel-head">
            <div>
              <h2>Последние повестки</h2>
              <p>Пять последних повесток по дате формирования.</p>
            </div>
          </div>
          <div className="compact-list">
            {data.recent.map(item => (
              <Link key={item.id} to={`/summons/${item.id}`} className="compact-row">
                <div>
                  <strong>{item.number}</strong>
                  <small>{item.citizen.lastName} {item.citizen.firstName}</small>
                </div>
                <StatusBadge status={item.status} />
              </Link>
            ))}
          </div>
        </section>
      </div>
    </>
  );
}
