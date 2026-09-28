import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getDashboard } from '../api';
import { StatusBadge } from '../components/StatusBadge';
import { statusLabels } from '../constants';
import type { DashboardSummary, Session, SummonsStatus } from '../types';

export function DashboardPage({ session }: { session: Session }) {
  const [data, setData] = useState<DashboardSummary | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    void getDashboard()
      .then(setData)
      .catch(e => setError(e instanceof Error ? e.message : 'Не удалось загрузить сводку'));
  }, []);

  if (error) return <div className="error">{error}</div>;
  if (!data) return <div className="state">Загрузка сводки…</div>;

  const statusEntries = Object.entries(data.byStatus) as Array<[SummonsStatus, number]>;
  const observer = session.role === 'Observer';
  const latestSummons = observer ? data.recent[0] : undefined;
  const remainingRecent = observer ? data.recent.slice(1) : data.recent;

  return (
    <>
      <header className="topbar">
        <div>
          <p className="eyebrow">{observer ? 'Личный кабинет' : 'Сводка'}</p>
          <h1>{observer ? 'Моя сводка' : 'Сводка по воинскому учёту'}</h1>
        </div>
        <Link className="primary button-link" to="/summons">
          {observer ? 'Все мои повестки' : 'Открыть реестр'}
        </Link>
      </header>

      {observer && latestSummons && (
        <Link to={`/summons/${latestSummons.id}`} className="latest-summons-hero">
          <div className="latest-summons-hero-head">
            <div>
              <span className="latest-summons-label">Последняя повестка</span>
              <strong>{latestSummons.number}</strong>
            </div>
            <StatusBadge status={latestSummons.status} />
          </div>

          <div className="latest-summons-grid">
            <div>
              <span>Срок явки</span>
              <strong>{new Date(latestSummons.dueAt).toLocaleString('ru-RU')}</strong>
            </div>
            <div>
              <span>Военкомат</span>
              <strong>{latestSummons.office.name}</strong>
            </div>
            <div className="latest-summons-reason">
              <span>Основание</span>
              <strong>{latestSummons.reason ?? '—'}</strong>
            </div>
          </div>

          <div className="latest-summons-open">Открыть повестку →</div>
        </Link>
      )}

      <section className="stats stats-wide">
        <article><span>{observer ? 'Мои повестки' : 'Всего повесток'}</span><strong>{data.total}</strong></article>
        <article><span>Активные</span><strong>{data.active}</strong></article>
        <article><span>Завершенные</span><strong>{data.completed}</strong></article>
        <article><span>Отмененные</span><strong>{data.cancelled}</strong></article>
        <article><span>{observer ? 'Моя карточка' : 'Призывники'}</span><strong>{data.citizens}</strong></article>
        <article><span>{observer ? 'Мои обращения' : 'Обращения'}</span><strong>{data.appeals}</strong></article>
        <article><span>{observer ? 'Мои документы' : 'Документы'}</span><strong>{data.documents}</strong></article>
      </section>

      <div className="dashboard-grid">
        <section className="panel">
          <div className="panel-head">
            <div>
              <h2>По статусам</h2>
              <p>{observer ? 'Распределение только ваших повесток.' : 'Распределение повесток военного учёта по статусам.'}</p>
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
              <h2>{observer ? 'Предыдущие повестки' : 'Последние повестки'}</h2>
              <p>
                {observer
                  ? 'Последняя повестка вынесена в крупную карточку выше.'
                  : 'Пять последних повесток по дате формирования.'}
              </p>
            </div>
          </div>
          <div className="compact-list">
            {remainingRecent.map(item => (
              <Link key={item.id} to={`/summons/${item.id}`} className="compact-row">
                <div>
                  <strong>{item.number}</strong>
                  <small>{item.citizen.lastName} {item.citizen.firstName}</small>
                </div>
                <StatusBadge status={item.status} />
              </Link>
            ))}
            {!remainingRecent.length && (
              <div className="state">
                {observer && latestSummons ? 'Других повесток пока нет' : 'Повесток пока нет'}
              </div>
            )}
          </div>
        </section>
      </div>
    </>
  );
}
